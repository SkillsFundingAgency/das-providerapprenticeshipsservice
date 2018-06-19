using System;
using Moq;
using NUnit.Framework;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Services;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Validation;
using SFA.DAS.ProviderApprenticeshipsService.Web.Validation.Text;
using SFA.DAS.Learners.Validators;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Validation.ApprenticeshipCreateOrEdit
{
    public abstract class ApprenticeshipValidationTestBase
    {
        protected readonly Mock<ICurrentDateTime> CurrentDateTime = new Mock<ICurrentDateTime>();
        protected Mock<IUlnValidator> MockUlnValidator = new Mock<IUlnValidator>();
        protected ApprenticeshipViewModelValidator Validator;
        protected ApprenticeshipViewModel ValidModel;

        [SetUp]
        public void BaseSetup()
        {
            CurrentDateTime.Setup(x => x.Now).Returns(DateTime.Now.AddMonths(6));
            Validator = new ApprenticeshipViewModelValidator(
                //todo: mock the interface
                new WebApprenticeshipValidationText(new AcademicYearDateProvider(CurrentDateTime.Object)),
                CurrentDateTime.Object,
                new AcademicYearDateProvider(CurrentDateTime.Object),
                MockUlnValidator.Object);
            ValidModel = new ApprenticeshipViewModel { ULN = "1001234567", FirstName = "TestFirstName", LastName = "TestLastName" };
        }
    }
}
