using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces.Services;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models.TrainingProvider;
using SFA.DAS.ProviderApprenticeshipsService.Web.Authentication;
using SFA.DAS.ProviderApprenticeshipsService.Web.Authorization;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Authentication
{
    public class TrainingProviderAuthorizationHandlerTest
    {
        [Test, MoqAutoData]
        public async Task Then_The_ProviderStatus_Is_Valid_And_True_Returned(
            long ukprn,
            GetProviderSummaryResult apiResponse,
            [Frozen] Mock<ITrainingProviderApiClient> trainingProviderApiClient,
            TrainingProviderAllRolesRequirement requirement,
            TrainingProviderAuthorizationHandler handler)
        {
            //Arrange
            apiResponse.CanAccessApprenticeshipService = true;
            var claims = new List<Claim>()
            {
                new Claim(DasClaimTypes.Ukprn, ukprn.ToString()),
            };
            var identity = new ClaimsPrincipal(new[] {new ClaimsIdentity(claims, "TestAuthType")});
            var context = new AuthorizationHandlerContext(new[] { requirement }, identity, null);
            trainingProviderApiClient.Setup(x => x.GetProviderDetails(ukprn)).ReturnsAsync(apiResponse);

            //Act
            var actual = await handler.IsProviderAuthorized(context, true);

            //Assert
            actual.Should().BeTrue();
        }
        [Test, MoqAutoData]
        public async Task Then_If_The_Ukprn_Is_Not_Valid_Then_False_Returned(
            [Frozen] Mock<ITrainingProviderApiClient> trainingProviderApiClient,
            TrainingProviderAllRolesRequirement requirement,
            TrainingProviderAuthorizationHandler handler)
        {
            //Arrange
            var claim = new Claim(DasClaimTypes.Ukprn, "test");
            var claimsPrinciple = new ClaimsPrincipal(new[] { new ClaimsIdentity(new[] { claim }) });
            var context = new AuthorizationHandlerContext(new[] { requirement }, claimsPrinciple, null);
            
            //Act
            var actual = await handler.IsProviderAuthorized(context, true);

            //Assert
            actual.Should().BeFalse();
        }

        [Test, MoqAutoData]
        public async Task Then_The_ProviderDetails_Is_InValid_And_False_Returned(
            long ukprn,
            GetProviderSummaryResult apiResponse,
            [Frozen] Mock<ITrainingProviderApiClient> trainingProviderApiClient,
            TrainingProviderAllRolesRequirement requirement,
            TrainingProviderAuthorizationHandler handler)
        {
            //Arrange
            apiResponse.CanAccessApprenticeshipService = false;
            var claims = new List<Claim>()
            {
                new Claim(DasClaimTypes.Ukprn, ukprn.ToString()),
            };
            var identity = new ClaimsPrincipal(new[] {new ClaimsIdentity(claims, "TestAuthType")});
            var context = new AuthorizationHandlerContext(new[] { requirement }, identity, null);
            trainingProviderApiClient.Setup(x => x.GetProviderDetails(ukprn)).ReturnsAsync(apiResponse);

            //Act
            var actual = await handler.IsProviderAuthorized(context, true);

            //Assert
            actual.Should().BeFalse();
        }

        [Test, MoqAutoData]
        public async Task Then_The_ProviderDetails_Is_Null_And_False_Returned(
            long ukprn,
            [Frozen] Mock<ITrainingProviderApiClient> trainingProviderApiClient,
            TrainingProviderAllRolesRequirement requirement,
            TrainingProviderAuthorizationHandler handler)
        {
            //Arrange
            var claims = new List<Claim>()
            {
                new Claim(DasClaimTypes.Ukprn, ukprn.ToString()),
            };
            var identity = new ClaimsPrincipal(new[] {new ClaimsIdentity(claims, "TestAuthType")});
            var context = new AuthorizationHandlerContext(new[] { requirement }, identity, null);
            trainingProviderApiClient.Setup(x => x.GetProviderDetails(ukprn)).ReturnsAsync((GetProviderSummaryResult)null);

            //Act
            var actual = await handler.IsProviderAuthorized(context, true);

            //Assert
            actual.Should().BeFalse();
        }
    }
}
