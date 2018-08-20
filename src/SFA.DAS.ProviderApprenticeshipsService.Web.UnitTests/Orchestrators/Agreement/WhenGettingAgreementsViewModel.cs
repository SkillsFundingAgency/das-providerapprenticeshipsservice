using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.HashingService;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetCommitmentAgreements;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.Mappers;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Orchestrators.Agreement
{
    [TestFixture]
    public class WhenGettingAgreementsViewModel
    {
        private AgreementOrchestrator _orchestrator;
        private Mock<IMediator> _mediator;
        private Mock<IAgreementMapper> _agreementMapper;

        [SetUp]
        public void Setup()
        {
            _mediator = new Mock<IMediator>();

            _agreementMapper = new Mock<IAgreementMapper>();

            _orchestrator = new AgreementOrchestrator(_mediator.Object,
                Mock.Of<IHashingService>(),
                Mock.Of<IProviderCommitmentsLogger>(),
                _agreementMapper.Object);
        }

        [Test]
        public async Task TheCommitmentAgreementsReturnedFromHandlerAreMapped()
        {
            const long providerId = 54321L;

            _mediator.Setup(m => m.SendAsync(It.IsAny<GetCommitmentAgreementsQueryRequest>()))
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
                CohortID = "cohort",
                OrganisationName = "org"
            };

            _agreementMapper.Setup(m => m.Map(It.IsAny<CommitmentAgreement>()))
                .Returns(mappedCommitmentAgreement);

            var result = await _orchestrator.GetAgreementsViewModel(providerId);

            Assert.IsNotNull(result);
            Assert.IsTrue(TestHelper.EnumerablesAreEqual(new[] { mappedCommitmentAgreement }, result.CommitmentAgreements));
        }

        [Test]
        public async Task TheCommitmentAgreementsReturnedFromHandlerAreMappedAndOrdered()
        {
            const long providerId = 54321L;

            _mediator.Setup(m => m.SendAsync(It.IsAny<GetCommitmentAgreementsQueryRequest>()))
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

            SetupMapping("1", "C", "2");
            SetupMapping("2", "B", "X");
            SetupMapping("3", "A", "X");
            SetupMapping("4", "C", "1");

            var result = await _orchestrator.GetAgreementsViewModel(providerId);

            Assert.IsNotNull(result);
            Assert.IsTrue(TestHelper.EnumerablesAreEqual(new[]
            {
                new Web.Models.Agreement.CommitmentAgreement { OrganisationName = "A", CohortID = "X" },
                new Web.Models.Agreement.CommitmentAgreement { OrganisationName = "B", CohortID = "X" },
                new Web.Models.Agreement.CommitmentAgreement { OrganisationName = "C", CohortID = "1" },
                new Web.Models.Agreement.CommitmentAgreement { OrganisationName = "C", CohortID = "2" }
            }, result.CommitmentAgreements));
        }

        private void SetupMapping(string inReference, string outOrganisationName, string outCohortId)
        {
            _agreementMapper.Setup(m => m.Map(It.Is<CommitmentAgreement>(ca => ca.Reference == inReference)))
                .Returns(new Web.Models.Agreement.CommitmentAgreement { OrganisationName = outOrganisationName, CohortID = outCohortId });
        }
    }
}
