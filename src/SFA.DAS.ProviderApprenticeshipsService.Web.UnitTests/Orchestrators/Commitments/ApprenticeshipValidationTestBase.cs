using FeatureToggle;
using FluentValidation.Results;

using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.HashingService;
using SFA.DAS.Learners.Validators;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models.FeatureToggles;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Services;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.Mappers;
using SFA.DAS.ProviderApprenticeshipsService.Web.Validation;
using SFA.DAS.ProviderApprenticeshipsService.Web.Validation.Text;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Orchestrators.Commitments
{
    public abstract class ApprenticeshipValidationTestBase
    {
        protected ICurrentDateTime _currentDateTime;

        protected ApprenticeshipViewModel ValidModel;
        protected CommitmentOrchestrator _orchestrator;
        protected Mock<IMediator> _mockMediator = new Mock<IMediator>();
        protected Mock<IHashingService> _mockHashingService = new Mock<IHashingService>();
        protected Mock<IApprenticeshipMapper> _mockMapper = new Mock<IApprenticeshipMapper>();
        protected Mock<ICommitmentStatusCalculator> _mockCalculator = new Mock<ICommitmentStatusCalculator>();
        protected Mock<IFeatureToggleService> MockFeatureToggleService;
        protected Mock<IFeatureToggle> MockFeatureToggleOn;

        private ApprenticeshipViewModelValidator _validator;
        private Mock<ApprenticeshipViewModelUniqueUlnValidator> _ulnValidator;

        [SetUp]
        public virtual void SetUp()
        {
            ValidModel = new ApprenticeshipViewModel { ULN = "1001234567", FirstName = "TestFirstName", LastName = "TestLastName" };
            _currentDateTime = _currentDateTime ?? new CurrentDateTime();

            _validator = new ApprenticeshipViewModelValidator(
                new WebApprenticeshipValidationText(new AcademicYearDateProvider(_currentDateTime)),
                _currentDateTime,
                new AcademicYearDateProvider(_currentDateTime),
                new UlnValidator(),
                new AcademicYearValidator(_currentDateTime, new AcademicYearDateProvider(_currentDateTime)));

            _ulnValidator = new Mock<ApprenticeshipViewModelUniqueUlnValidator>();
            _ulnValidator
                .Setup(m => m.ValidateAsyncOverride(It.IsAny<ApprenticeshipViewModel>()))
                .ReturnsAsync(new ValidationResult());

            MockFeatureToggleOn = new Mock<IFeatureToggle>();
            MockFeatureToggleOn.Setup(x => x.FeatureEnabled).Returns(true);
            MockFeatureToggleService = new Mock<IFeatureToggleService>();
            MockFeatureToggleService.Setup(x => x.Get<Transfers>()).Returns(MockFeatureToggleOn.Object);

            SetUpOrchestrator();
        }

        protected void SetUpOrchestrator()
        {
            _orchestrator = new CommitmentOrchestrator(
                       _mockMediator.Object,
                       _mockCalculator.Object,
                       _mockHashingService.Object,
                       Mock.Of<IProviderCommitmentsLogger>(),
                       _ulnValidator.Object,
                       Mock.Of<ProviderApprenticeshipsServiceConfiguration>(),
                       _mockMapper.Object,
                       _validator,
                       Mock.Of<IAcademicYearDateProvider>(),
                       MockFeatureToggleService.Object);
        }

        protected void SetUpOrchestrator(ICommitmentStatusCalculator commitmentStatusCalculator)
        {
            _orchestrator = new CommitmentOrchestrator(
                       _mockMediator.Object,
                       commitmentStatusCalculator,
                       _mockHashingService.Object,
                       Mock.Of<IProviderCommitmentsLogger>(),
                       Mock.Of<ApprenticeshipViewModelUniqueUlnValidator>(),
                       Mock.Of<ProviderApprenticeshipsServiceConfiguration>(),
                       _mockMapper.Object,
                       _validator,
                       Mock.Of<IAcademicYearDateProvider>(),
                       MockFeatureToggleService.Object);
        }
    }
}