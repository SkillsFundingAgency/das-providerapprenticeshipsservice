using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types.Validation;
using SFA.DAS.HashingService;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetOverlappingApprenticeships;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.ApprenticeshipUpdate;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.Mappers;
using SFA.DAS.ProviderApprenticeshipsService.Web.Validation;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Orchestrators.ManageApprentices
{
    [TestFixture]
    public class WhenValidatingApprenticeshipUpdate
    {
        private ManageApprenticesOrchestrator _orchestrator;
        private Mock<IMediator> _mockMediator;
        private Mock<IApprovedApprenticeshipValidator> _mockValidator;
        private Mock<IApprenticeshipMapper> _mockMapper;

        [SetUp]
        public void SetUp()
        {
            _mockMediator = new Mock<IMediator>();

            _mockValidator = new Mock<IApprovedApprenticeshipValidator>();
            _mockValidator.Setup(m => m.MapOverlappingErrors(It.IsAny<GetOverlappingApprenticeshipsQueryResponse>()))
                .Returns(new Dictionary<string, string>());
            _mockMapper = new Mock<IApprenticeshipMapper>();

            _mockValidator.Setup(m => m.ValidateToDictionary(It.IsAny<ApprenticeshipViewModel>()))
                .Returns(new Dictionary<string, string>());

            _mockValidator.Setup(m => m.ValidateAcademicYear(It.IsAny<CreateApprenticeshipUpdateViewModel>()))
                .Returns(new Dictionary<string, string>());

            _mockValidator.Setup(m => m.ValidateApprovedEndDate(It.IsAny<CreateApprenticeshipUpdateViewModel>()))
                .Returns(new Dictionary<string, string>());

            _orchestrator = new ManageApprenticesOrchestrator(
                _mockMediator.Object,
                Mock.Of<IHashingService>(),
                Mock.Of<IProviderCommitmentsLogger>(),
                _mockMapper.Object,
                _mockValidator.Object,
                Mock.Of<IApprenticeshipFiltersMapper>(),
                Mock.Of<IDataLockMapper>());
        }

        [Test]
        public async Task ShouldValidate()
        {
            var viewModel = new ApprenticeshipViewModel();
            var updateModel = new CreateApprenticeshipUpdateViewModel();

            _mockMediator.Setup(m => m.SendAsync(It.IsAny<GetOverlappingApprenticeshipsQueryRequest>()))
                .ReturnsAsync(new GetOverlappingApprenticeshipsQueryResponse { Overlaps = new List<ApprenticeshipOverlapValidationResult>() });

            await _orchestrator.ValidateEditApprenticeship(viewModel, updateModel);

            _mockMediator.Verify(m => m.SendAsync(It.IsAny<GetOverlappingApprenticeshipsQueryRequest>()), Times.Once, failMessage: "Should call");
            _mockValidator.Verify(m => m.MapOverlappingErrors(It.IsAny<GetOverlappingApprenticeshipsQueryResponse>()), Times.Once, failMessage: "Should verify overlapping apprenticeship");
            _mockValidator.Verify(m => m.ValidateToDictionary(It.IsAny<ApprenticeshipViewModel>()), Times.Once, failMessage: "Should validate apprenticeship");
            _mockValidator.Verify(m => m.ValidateAcademicYear(It.IsAny<CreateApprenticeshipUpdateViewModel>()), Times.Once, failMessage: "Should validate academic year");
            _mockValidator.Verify(m => m.ValidateApprovedEndDate(It.IsAny<CreateApprenticeshipUpdateViewModel>()), Times.Once, failMessage: "Should validate end date");
        }
    }
}
