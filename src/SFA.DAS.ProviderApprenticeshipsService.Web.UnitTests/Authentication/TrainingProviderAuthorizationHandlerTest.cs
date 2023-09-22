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
            GetProviderStatusResult apiResponse,
            [Frozen] Mock<ITrainingProviderApiClient> trainingProviderApiClient,
            [Frozen] Mock<IHttpContextAccessor> httpContextAccessor,
            AuthorizationHandlerContext context,
            TrainingProviderAuthorizationHandler handler)
        {
            //Arrange
            apiResponse.IsValidProvider = true;
            var claims = new List<Claim>()
            {
                new Claim(DasClaimTypes.Ukprn, ukprn.ToString()),
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            httpContextAccessor.Setup(x => x.HttpContext.User.Identity).Returns(identity);
            trainingProviderApiClient.Setup(x => x.GetProviderStatus(ukprn)).ReturnsAsync(apiResponse);

            //Act
            var actual = await handler.IsProviderAuthorized(context, true);

            //Assert
            actual.Should().BeTrue();
        }

        [Test, MoqAutoData]
        public async Task Then_The_ProviderStatus_Is_InValid_And_False_Returned(
            long ukprn,
            GetProviderStatusResult apiResponse,
            [Frozen] Mock<ITrainingProviderApiClient> trainingProviderApiClient,
            [Frozen] Mock<IHttpContextAccessor> httpContextAccessor,
            AuthorizationHandlerContext context,
            TrainingProviderAuthorizationHandler handler)
        {
            //Arrange
            apiResponse.IsValidProvider = false;
            var claims = new List<Claim>()
            {
                new Claim(DasClaimTypes.Ukprn, ukprn.ToString()),
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            httpContextAccessor.Setup(x => x.HttpContext.User.Identity).Returns(identity);
            trainingProviderApiClient.Setup(x => x.GetProviderStatus(ukprn)).ReturnsAsync(apiResponse);

            //Act
            var actual = await handler.IsProviderAuthorized(context, true);

            //Assert
            actual.Should().BeFalse();
        }

        [Test, MoqAutoData]
        public async Task Then_The_ProviderStatus_Is_Null_And_False_Returned(
            long ukprn,
            GetProviderStatusResult apiResponse,
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
            trainingProviderApiClient.Setup(x => x.GetProviderStatus(ukprn)).ReturnsAsync((GetProviderStatusResult)null);

            //Act
            var actual = await handler.IsProviderAuthorized(context, true);

            //Assert
            actual.Should().BeFalse();
        }
    }
}
