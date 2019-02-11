using NUnit.Framework;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Moq;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.Commitments.Api.Types.Commitment.Types;
using SFA.DAS.HashingService;
using SFA.DAS.ProviderApprenticeshipsService.Application.Domain.Commitment;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetCommitments;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetProviderAgreement;
using SFA.DAS.ProviderApprenticeshipsService.Domain;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.Mappers;
using SFA.DAS.ProviderApprenticeshipsService.Web.Validation;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Orchestrators.Commitments
{
    [TestFixture]
    public class WhenGettingDraftCohorts
    {
        private CommitmentOrchestrator _orchestrator;
        private Mock<IMediator> _mediator;
             
        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _mediator.Setup(x => x.Send(It.IsAny<GetCommitmentsQueryRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => new GetCommitmentsQueryResponse
                {
                    Commitments = new List<CommitmentListItem>
                    {
                        new CommitmentListItem
                        {
                            Id = 1,
                            EditStatus = EditStatus.ProviderOnly,
                            LastAction = LastAction.None
                        },
                        new CommitmentListItem
                        {
                            Id = 2,
                            EditStatus = EditStatus.EmployerOnly,
                            LastAction = LastAction.None
                        },
                        new CommitmentListItem
                        {
                            Id = 3,
                            EditStatus = EditStatus.ProviderOnly,
                            LastAction = LastAction.Amend
                        }
                    }
                });
            _mediator.Setup(x => x.Send(It.IsAny<GetProviderAgreementQueryRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => new GetProviderAgreementQueryResponse
                {
                    HasAgreement = ProviderAgreementStatus.Agreed
                });
                

            _orchestrator = new CommitmentOrchestrator(
                _mediator.Object,
                Mock.Of<IHashingService>(),
                Mock.Of<IProviderCommitmentsLogger>(),
                Mock.Of<ApprenticeshipViewModelUniqueUlnValidator>(),
                Mock.Of<ProviderApprenticeshipsServiceConfiguration>(),
                Mock.Of<IApprenticeshipCoreValidator>(),
                Mock.Of<IApprenticeshipMapper>()
            );
        }

        [Test]
        public async Task ThenAllCohortsForTheProviderAreRetrieved()
        {
            await _orchestrator.GetAllDrafts(999);
            _mediator.Verify(x => x.Send(It.Is<GetCommitmentsQueryRequest>(r => r.ProviderId == 999), It.IsAny<CancellationToken>()));
        }

        [Test]
        public async Task ThenOnlyCohortsWithStatusOfNewRequestAreReturned()
        {
            var result = await _orchestrator.GetAllDrafts(999);
            Assert.AreEqual(1, result.Commitments.Count());
            Assert.AreEqual(RequestStatus.NewRequest, result.Commitments.First().Status);
        }
    }
}
