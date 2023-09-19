using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Enums;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces.Services;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models.TrainingProvider;
using SFA.DAS.ProviderApprenticeshipsService.Web.Authentication;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Authentication
{
    public class TrainingProviderAuthorizationHandlerTest
    {
        [Test, MoqAutoData]
        public async Task Then_The_Provider_Is_Valid_And_True_Returned(
            long ukprn,
            GetProviderResponseItem apiResponse,
            [Frozen] Mock<ITrainingProviderService> trainingProviderService,
            [Frozen] Mock<IHttpContextAccessor> httpContextAccessor,
            AuthorizationHandlerContext context,
            TrainingProviderAuthorizationHandler handler)
        {
            //Arrange
            apiResponse.ProviderTypeId = (int) ProviderTypeIdentifier.MainProvider;
            apiResponse.StatusId = (int) ProviderStatusType.Active;
            var claims = new List<Claim>()
            {
                new Claim(DasClaimTypes.Ukprn, ukprn.ToString()),
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            httpContextAccessor.Setup(x => x.HttpContext.User.Identity).Returns(identity);
            trainingProviderService
                .Setup(x => x.GetProviderDetails(ukprn)).ReturnsAsync(apiResponse);

            //Act
            var actual = await handler.IsProviderAuthorized(context, true);

            //Assert
            actual.Should().BeTrue();
        }

        [Test, MoqAutoData]
        public async Task Then_The_Provider_Is_InValid_And_False_Returned(
            long ukprn,
            GetProviderResponseItem apiResponse,
            [Frozen] Mock<ITrainingProviderService> trainingProviderService,
            [Frozen] Mock<IHttpContextAccessor> httpContextAccessor,
            AuthorizationHandlerContext context,
            TrainingProviderAuthorizationHandler handler)
        {
            //Arrange
            apiResponse.ProviderTypeId = (int)ProviderTypeIdentifier.EPAO;
            apiResponse.StatusId = (int)ProviderStatusType.ActiveButNotTakingOnApprentices;
            var claims = new List<Claim>()
            {
                new Claim(DasClaimTypes.Ukprn, ukprn.ToString()),
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            httpContextAccessor.Setup(x => x.HttpContext.User.Identity).Returns(identity);
            trainingProviderService
                .Setup(x => x.GetProviderDetails(ukprn)).ReturnsAsync(apiResponse);

            //Act
            var actual = await handler.IsProviderAuthorized(context, true);

            //Assert
            actual.Should().BeFalse();
        }
    }
}
