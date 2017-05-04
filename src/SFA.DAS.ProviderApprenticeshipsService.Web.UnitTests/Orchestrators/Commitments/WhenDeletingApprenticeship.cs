using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;

using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.ProviderApprenticeshipsService.Application.Commands.DeleteApprenticeship;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetApprenticeship;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Services;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators;
using SFA.DAS.ProviderApprenticeshipsService.Web.Validation;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.Mappers;
using SFA.DAS.ProviderApprenticeshipsService.Web.Validation.Text;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Orchestrators.Commitments
{
    [TestFixture]
    public sealed class WhenDeletingApprenticeship : ApprenticeshipValidationTestBase
    {
        private CommitmentOrchestrator _orchestrator;
        private Mock<IMediator> _mockMediator;
        

        [SetUp]
        public void Setup()
        {
            _mockMediator = new Mock<IMediator>();

            var mockHashingService = new Mock<IHashingService>();
            mockHashingService.Setup(m => m.DecodeValue("ABBA99")).Returns(123L);
            mockHashingService.Setup(m => m.DecodeValue("ABBA66")).Returns(321L);

            _orchestrator = new CommitmentOrchestrator(
                _mockMediator.Object, 
                Mock.Of<ICommitmentStatusCalculator>(), 
                mockHashingService.Object,
                Mock.Of<IProviderCommitmentsLogger>(),
                Mock.Of<ApprenticeshipViewModelUniqueUlnValidator>(),
                Mock.Of<ProviderApprenticeshipsServiceConfiguration>(),
                Mock.Of<IApprenticeshipMapper>(),
                Validator);
        }

        [Test]
        public async Task ShouldCallMediatorToDelete()
        {
            DeleteApprenticeshipCommand arg = null;

            _mockMediator.Setup(x => x.SendAsync(It.IsAny<DeleteApprenticeshipCommand>()))
                .ReturnsAsync(new Unit()).Callback<DeleteApprenticeshipCommand>(x => arg = x);

            _mockMediator.Setup(x => x.SendAsync(It.IsAny<GetApprenticeshipQueryRequest>()))
                .ReturnsAsync(new GetApprenticeshipQueryResponse { Apprenticeship = new Apprenticeship() });

            await _orchestrator.DeleteApprenticeship("user123", new Web.Models.DeleteConfirmationViewModel
            {
                ProviderId = 123L,
                HashedCommitmentId = "ABBA99",
                HashedApprenticeshipId = "ABBA66"
            });

            _mockMediator.Verify(x => x.SendAsync(It.IsAny<DeleteApprenticeshipCommand>()), Times.Once);
            arg.ProviderId.Should().Be(123);
            arg.ApprenticeshipId.Should().Be(321);
        }
    }
}
