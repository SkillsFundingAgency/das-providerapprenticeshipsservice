using System;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Types;
using SFA.DAS.ProviderApprenticeshipsService.Web.Validation;
using SFA.DAS.ProviderApprenticeshipsService.Web.Validation.Text;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Validation.ApprenticeshipCreateOrEdit
{
    [TestFixture]
    public class WhenValidatingTrainingCode
    {
        private readonly ApprenticeshipViewModelValidator _validator = new ApprenticeshipViewModelValidator(new WebApprenticeshipValidationText());
        private ApprenticeshipViewModel _validModel;

        [SetUp]
        public void Setup()
        {
            _validModel = new ApprenticeshipViewModel { ULN = "1001234567", FirstName = "TestFirstName", LastName = "TestLastName" };
        }


    }
}
