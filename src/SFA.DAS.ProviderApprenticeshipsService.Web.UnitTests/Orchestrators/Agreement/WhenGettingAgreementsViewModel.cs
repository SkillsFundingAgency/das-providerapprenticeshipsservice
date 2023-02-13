﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.HashingService;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetCommitmentAgreements;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.Mappers;
using SFA.DAS.CommitmentsV2.Api.Types;
using SFA.DAS.CommitmentsV2.Types;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Orchestrators.Agreement
{
    [TestFixture]
    public class WhenGettingAgreementsViewModel
    {
        private AgreementOrchestrator _orchestrator;
        private Mock<IMediator> _mediator;
        private Mock<IAgreementMapper> _agreementMapper;
        private const long providerId = 54321L;

        [SetUp]
        public void Setup()
        {
            _mediator = new Mock<IMediator>();
            _mediator.Setup(m => m.Send(It.IsAny<GetCommitmentAgreementsQueryRequest>(), new CancellationToken()))
            .ReturnsAsync(new GetCommitmentAgreementsQueryResponse
            {
                CommitmentAgreements = new List<ProviderCommitmentAgreement>
                {
                        new ProviderCommitmentAgreement { AccountLegalEntityPublicHashedId = "1" , LegalEntityName = "A"},
                        new ProviderCommitmentAgreement { AccountLegalEntityPublicHashedId = "2" , LegalEntityName = "B"},
                        new ProviderCommitmentAgreement { AccountLegalEntityPublicHashedId = "3" , LegalEntityName = "C"},
                        new ProviderCommitmentAgreement { AccountLegalEntityPublicHashedId = "4" , LegalEntityName = "D"},
                }
            });

            _agreementMapper = new Mock<IAgreementMapper>();

            _orchestrator = new AgreementOrchestrator(_mediator.Object,
                Mock.Of<IProviderCommitmentsLogger>(),
                _agreementMapper.Object);
        }

        [Test]
        public async Task TheCommitmentAgreementsReturnedFromHandlerAreMapped()
        {
            //Arrange
            _mediator.Setup(m => m.Send(It.IsAny<GetCommitmentAgreementsQueryRequest>(), new CancellationToken()))
                .ReturnsAsync(new GetCommitmentAgreementsQueryResponse
                {
                    CommitmentAgreements = new List<ProviderCommitmentAgreement>
                    {
                        new ProviderCommitmentAgreement()
                    }
                });

            var mappedCommitmentAgreement = new Web.Models.Agreement.CommitmentAgreement
            {
                AgreementID = "agree",
                OrganisationName = "org"
            };

            _agreementMapper.Setup(m => m.Map(It.IsAny<ProviderCommitmentAgreement>()))
                .Returns(mappedCommitmentAgreement);

            //Act
            var result = await _orchestrator.GetAgreementsViewModel(providerId, string.Empty);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(TestHelper.EnumerablesAreEqual(new[] { mappedCommitmentAgreement }, result.CommitmentAgreements));
        }

        [Test]
        public async Task TheCommitmentAgreementsReturnedFromHandlerAreMappedAndOrdered()
        {
            //Arrange
            SetOrganisations();

            //Act
            var result = await _orchestrator.GetAgreementsViewModel(providerId, string.Empty);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(TestHelper.EnumerablesAreEqual(new[]
            {
                new Web.Models.Agreement.CommitmentAgreement { OrganisationName = "A" , AgreementID = "1" },
                new Web.Models.Agreement.CommitmentAgreement { OrganisationName = "B" , AgreementID = "2" },
                new Web.Models.Agreement.CommitmentAgreement { OrganisationName = "C" , AgreementID = "3" },
                new Web.Models.Agreement.CommitmentAgreement { OrganisationName = "D" , AgreementID = "4" }
            }, result.CommitmentAgreements));
        }

        [Test]
        public async Task TheCommitmentAgreementsReturnedFromHandlerAreMappedAndOrderedBySearchText()
        {
            //Arrange
            SetOrganisations();

            //Act
            var result = await _orchestrator.GetAgreementsViewModel(providerId, "A");

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(TestHelper.EnumerablesAreEqual(new[]
            {
                new Web.Models.Agreement.CommitmentAgreement { OrganisationName = "A", AgreementID = "1"}
            }, result.CommitmentAgreements));
        }

        [Test]
        public async Task TheCommitmentAgreementsReturnedFromHandlerAreMappedAndOrderedBySearchTextWithNoResults()
        {
            //Arrange
            SetOrganisations();

            //Act
            var result = await _orchestrator.GetAgreementsViewModel(providerId, "V");

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(result.CommitmentAgreements.Count(), 0);
        }

        [Test]
        public async Task TheCommitmentAgreementsReturnedFromHandlerAreMappedAndOrderedBySearchTextWithoutDuplicateResults()
        {
            //Arrange
            SetOrganisations();

            //Act
            var result = await _orchestrator.GetAgreementsViewModel(providerId, "D");

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(result.CommitmentAgreements.Count(), 1);
        }

        [Test]
        public async Task ThenAllOrganisationNamesAreReturnedWithFilteredResults()
        {
            //Arrange
            SetOrganisations();

            //Act
            var result = await _orchestrator.GetAgreementsViewModel(providerId, "D");

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(result.CommitmentAgreements.Count(), 1);
            Assert.AreEqual(result.AllProviderOrganisationNames.Count(), 4);
        }

        private void SetOrganisations()
        {
            SetupMapping("1", "A");
            SetupMapping("2", "B");
            SetupMapping("3", "C");
            SetupMapping("4", "D");
            SetupMapping("4", "D");
        }

        private void SetupMapping(string publicHashedId, string outOrganisationName)
        {
            _agreementMapper
                .Setup(m => m.Map(It.Is<ProviderCommitmentAgreement>(ca => ca.AccountLegalEntityPublicHashedId == publicHashedId)))
                .Returns(new Web.Models.Agreement.CommitmentAgreement { OrganisationName = outOrganisationName, AgreementID = publicHashedId });
        }
    }
}