using FeatureToggle;
using FluentValidation.Results;

using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.HashingService;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models.FeatureToggles;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Services;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.Mappers;
using SFA.DAS.ProviderApprenticeshipsService.Web.Validation;

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
        protected Mock<IFeatureToggleService> MockFeatureToggleService;
        protected Mock<IFeatureToggle> MockFeatureToggleOn;

        private Mock<ApprenticeshipViewModelUniqueUlnValidator> _ulnValidator;

        [SetUp]
        public virtual void SetUp()
        {
            ValidModel = new ApprenticeshipViewModel { ULN = "1001234567", FirstName = "TestFirstName", LastName = "TestLastName" };
            _currentDateTime = _currentDateTime ?? new CurrentDateTime();

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
                       _mockHashingService.Object,
                       Mock.Of<IProviderCommitmentsLogger>(),
                       _ulnValidator.Object,
                       Mock.Of<ProviderApprenticeshipsServiceConfiguration>(),
                       _mockMapper.Object,
                       MockFeatureToggleService.Object);
        }
    }
}