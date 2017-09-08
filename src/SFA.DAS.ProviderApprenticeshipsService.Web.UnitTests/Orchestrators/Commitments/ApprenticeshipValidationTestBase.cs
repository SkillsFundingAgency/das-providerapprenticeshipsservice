using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Learners.Validators;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
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
        protected readonly ApprenticeshipViewModelValidator Validator = new ApprenticeshipViewModelValidator(
                                                                        new WebApprenticeshipValidationText(new Infrastructure.Services.AcademicYearDateProvider(new CurrentDateTime())),
                                                                        new CurrentDateTime(),
                                                                        new Infrastructure.Services.AcademicYearDateProvider(new CurrentDateTime()),
                                                                        new UlnValidator(),
                                                                        new AcademicYearValidator(new CurrentDateTime(), new Infrastructure.Services.AcademicYearDateProvider(new CurrentDateTime())));

        protected ApprenticeshipViewModel ValidModel;
        protected CommitmentOrchestrator _orchestrator;
        protected Mock<IMediator> _mockMediator = new Mock<IMediator>();
        protected Mock<IHashingService> _mockHashingService = new Mock<IHashingService>();
        protected Mock<IApprenticeshipMapper> _mockMapper = new Mock<IApprenticeshipMapper>();
        protected Mock<ICommitmentStatusCalculator> _mockCalculator = new Mock<ICommitmentStatusCalculator>();


        [SetUp]
        public void BaseSetup()
        {
            ValidModel = new ApprenticeshipViewModel { ULN = "1001234567", FirstName = "TestFirstName", LastName = "TestLastName" };
            SetUp();
            SetUpOrchestrator();
        }

        protected virtual void SetUp()
        {
        }

        protected void SetUpOrchestrator()
        {
            _orchestrator = new CommitmentOrchestrator(
                       _mockMediator.Object,
                       _mockCalculator.Object,
                       _mockHashingService.Object,
                       Mock.Of<IProviderCommitmentsLogger>(),
                       Mock.Of<ApprenticeshipViewModelUniqueUlnValidator>(),
                       Mock.Of<ProviderApprenticeshipsServiceConfiguration>(),
                       _mockMapper.Object,
                       Validator,
                       Mock.Of<IAcademicYearValidator>(),
                       Mock.Of<IAcademicYearDateProvider>());
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
                       Validator,
                       Mock.Of<IAcademicYearValidator>(),
                       Mock.Of<IAcademicYearDateProvider>());
        }
    }
}