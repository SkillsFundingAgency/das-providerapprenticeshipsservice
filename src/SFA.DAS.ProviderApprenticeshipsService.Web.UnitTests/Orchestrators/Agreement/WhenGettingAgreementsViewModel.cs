﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.HashingService;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetCommitmentAgreements;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.Formatters;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.Mappers;

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
                CommitmentAgreements = new List<CommitmentAgreement>
                {
                        new CommitmentAgreement { Reference = "1" },
                        new CommitmentAgreement { Reference = "2" },
                        new CommitmentAgreement { Reference = "3" },
                        new CommitmentAgreement { Reference = "4" },
                }
            });

            _agreementMapper = new Mock<IAgreementMapper>();

            _orchestrator = new AgreementOrchestrator(_mediator.Object,
                Mock.Of<IHashingService>(),
                Mock.Of<IProviderCommitmentsLogger>(),
                _agreementMapper.Object,
                Mock.Of<ICsvFormatter>(),
                Mock.Of<IExcelFormatter>());
        }

        [Test]
        public async Task TheCommitmentAgreementsReturnedFromHandlerAreMapped()
        {
            //Arrange
            _mediator.Setup(m => m.Send(It.IsAny<GetCommitmentAgreementsQueryRequest>(), new CancellationToken()))
                .ReturnsAsync(new GetCommitmentAgreementsQueryResponse
                {
                    CommitmentAgreements = new List<CommitmentAgreement>
                    {
                        new CommitmentAgreement()
                    }
                });

            var mappedCommitmentAgreement = new Web.Models.Agreement.CommitmentAgreement
            {
                AgreementID = "agree",               
                OrganisationName = "org"
            };

            _agreementMapper.Setup(m => m.Map(It.IsAny<CommitmentAgreement>()))
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
                new Web.Models.Agreement.CommitmentAgreement { OrganisationName = "A" },
                new Web.Models.Agreement.CommitmentAgreement { OrganisationName = "B" },
                new Web.Models.Agreement.CommitmentAgreement { OrganisationName = "C" },
                new Web.Models.Agreement.CommitmentAgreement { OrganisationName = "C" }
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
                new Web.Models.Agreement.CommitmentAgreement { OrganisationName = "A"}
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

        private void SetOrganisations()
        {
            SetupMapping("1", "C" );
            SetupMapping("2", "B" );
            SetupMapping("3", "A" );
            SetupMapping("4", "C" );
        }

        private void SetupMapping(string inReference, string outOrganisationName)
        {
            _agreementMapper.Setup(m => m.Map(It.Is<CommitmentAgreement>(ca => ca.Reference == inReference)))
                .Returns(new Web.Models.Agreement.CommitmentAgreement { OrganisationName = outOrganisationName });
        }
    }
}
