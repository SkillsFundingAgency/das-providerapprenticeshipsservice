using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.ProviderApprenticeshipsService.Application.Commands.CreateApprenticeshipUpdate;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.ApprenticeshipUpdate;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.ApprovedApprenticeshipValidation;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.Mappers;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Orchestrators.ManageApprentices
{
    [TestFixture]
    public sealed class WhenCreatingApprenticeshipUpdate
    {
        private ManageApprenticesOrchestrator _orchestrator;
        private Mock<IMediator> _mockMediator;
        private Mock<IApprenticeshipMapper> _mockApprenticeshipMapper;
        
        [SetUp]
        public void Setup()
        {
            _mockMediator = new Mock<IMediator>();
            _mockApprenticeshipMapper = new Mock<IApprenticeshipMapper>();
            
            _orchestrator = new ManageApprenticesOrchestrator(
                _mockMediator.Object, 
                Mock.Of<IHashingService>(),
                Mock.Of<IProviderCommitmentsLogger>(),
                _mockApprenticeshipMapper.Object,
                Mock.Of<IApprovedApprenticeshipValidator>());
        }

        [Test]
        public async Task ShouldCallMediatorToCreate()
        {
            var providerId = 123;
            string userId = "ABC";
            var expectedApprenticeship = new ApprenticeshipUpdate();
            var viewModel = new CreateApprenticeshipUpdateViewModel();
            var signedInUser = new SignInUserModel() { DisplayName = "Bob", Email = "bob@test.com" };
            _mockApprenticeshipMapper.Setup(x => x.MapFrom(viewModel)).Returns(expectedApprenticeship);

            await _orchestrator.CreateApprenticeshipUpdate(viewModel, providerId, userId, signedInUser);

            _mockMediator.Verify(
                x =>
                    x.SendAsync(
                        It.Is<CreateApprenticeshipUpdateCommand>(
                            c =>
                                c.ProviderId == providerId && c.UserId == userId && c.ApprenticeshipUpdate == expectedApprenticeship && c.UserDisplayName == signedInUser.DisplayName &&
                                c.UserEmailAddress == signedInUser.Email)), Times.Once);
        }
    }
}
