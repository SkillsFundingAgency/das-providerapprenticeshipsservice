using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces.Services;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models.TrainingProvider;
using SFA.DAS.ProviderApprenticeshipsService.Web.Authentication;
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
            [Frozen] Mock<IHttpContextAccessor> httpContextAccessor,
            AuthorizationHandlerContext context,
            TrainingProviderAuthorizationHandler handler)
        {
            //Arrange
            apiResponse.CanAccessApprenticeshipService = true;
            var claims = new List<Claim>()
            {
                new Claim(DasClaimTypes.Ukprn, ukprn.ToString()),
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            httpContextAccessor.Setup(x => x.HttpContext.User.Identity).Returns(identity);
            trainingProviderApiClient.Setup(x => x.GetProviderDetails(ukprn)).ReturnsAsync(apiResponse);

            //Act
            var actual = await handler.IsProviderAuthorized(context, true);

            //Assert
            actual.Should().BeTrue();
        }

        [Test, MoqAutoData]
        public async Task Then_The_ProviderDetails_Is_InValid_And_False_Returned(
            long ukprn,
            GetProviderSummaryResult apiResponse,
            [Frozen] Mock<ITrainingProviderApiClient> trainingProviderApiClient,
            [Frozen] Mock<IHttpContextAccessor> httpContextAccessor,
            AuthorizationHandlerContext context,
            TrainingProviderAuthorizationHandler handler)
        {
            //Arrange
            apiResponse.CanAccessApprenticeshipService = false;
            var claims = new List<Claim>()
            {
                new Claim(DasClaimTypes.Ukprn, ukprn.ToString()),
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            httpContextAccessor.Setup(x => x.HttpContext.User.Identity).Returns(identity);
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
            [Frozen] Mock<IHttpContextAccessor> httpContextAccessor,
            AuthorizationHandlerContext context,
            TrainingProviderAuthorizationHandler handler)
        {
            //Arrange
            var claims = new List<Claim>()
            {
                new Claim(DasClaimTypes.Ukprn, ukprn.ToString()),
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            httpContextAccessor.Setup(x => x.HttpContext.User.Identity).Returns(identity);
            trainingProviderApiClient.Setup(x => x.GetProviderDetails(ukprn)).ReturnsAsync((GetProviderSummaryResult)null);

            //Act
            var actual = await handler.IsProviderAuthorized(context, true);

            //Assert
            actual.Should().BeFalse();
        }
    }
}
