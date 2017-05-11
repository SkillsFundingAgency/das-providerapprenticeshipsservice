using System;
using Moq;
using NUnit.Framework;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Services;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Validation;
using SFA.DAS.ProviderApprenticeshipsService.Web.Validation.Text;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Validation.ApprenticeshipCreateOrEdit
{
    public abstract class ApprenticeshipValidationTestBase
    {
        protected readonly Mock<ICurrentDateTime> CurrentDateTime = new Mock<ICurrentDateTime>();

        protected ApprenticeshipViewModelValidator Validator;
        protected ApprenticeshipViewModel ValidModel;

        [SetUp]
        public void BaseSetup()
        {
            CurrentDateTime.Setup(x => x.Now).Returns(new DateTime(2018, 5, 1));

            Validator = new ApprenticeshipViewModelValidator(new WebApprenticeshipValidationText(), CurrentDateTime.Object, new Infrastructure.Services.AcademicYear(CurrentDateTime.Object));

            ValidModel = new ApprenticeshipViewModel { ULN = "1001234567", FirstName = "TestFirstName", LastName = "TestLastName" };
        }
    }
}
