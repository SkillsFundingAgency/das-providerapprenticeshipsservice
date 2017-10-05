using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;

using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.ProviderApprenticeshipsService.Application.Commands.DeleteApprenticeship;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetApprenticeship;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Orchestrators.Commitments
{
    [TestFixture]
    public sealed class WhenDeletingApprenticeship : ApprenticeshipValidationTestBase
    {
        [SetUp]
        protected void SetUp()
        {
            _mockHashingService.Setup(m => m.DecodeValue("ABBA99")).Returns(123L);
            _mockHashingService.Setup(m => m.DecodeValue("ABBA66")).Returns(321L);
        }

        [Test]
        public async Task ShouldCallMediatorToDelete()
        {
            DeleteApprenticeshipCommand arg = null;

            _mockMediator.Setup(x => x.SendAsync(It.IsAny<DeleteApprenticeshipCommand>()))
                .ReturnsAsync(new Unit()).Callback<DeleteApprenticeshipCommand>(x => arg = x);

            _mockMediator.Setup(x => x.SendAsync(It.IsAny<GetApprenticeshipQueryRequest>()))
                .ReturnsAsync(new GetApprenticeshipQueryResponse { Apprenticeship = new Apprenticeship() });

            var signInUser = new SignInUserModel { DisplayName = "Bob", Email = "test@email.com" };

            await _orchestrator.DeleteApprenticeship("user123", new Web.Models.DeleteConfirmationViewModel
            {
                ProviderId = 123L,
                HashedCommitmentId = "ABBA99",
                HashedApprenticeshipId = "ABBA66"
            }, signInUser);

            _mockMediator.Verify(x => x.SendAsync(It.IsAny<DeleteApprenticeshipCommand>()), Times.Once);
            arg.ProviderId.Should().Be(123);
            arg.ApprenticeshipId.Should().Be(321);
            arg.UserDisplayName.Should().Be(signInUser.DisplayName);
            arg.UserEmailAddress.Should().Be(signInUser.Email);
        }
    }
}
