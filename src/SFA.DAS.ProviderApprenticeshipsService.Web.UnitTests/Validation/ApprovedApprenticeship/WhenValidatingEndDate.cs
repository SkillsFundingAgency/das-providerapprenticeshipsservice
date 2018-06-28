using System;
using System.Linq;
using Moq;
using NUnit.Framework;
using SFA.DAS.Learners.Validators;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Services;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.ApprenticeshipUpdate;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Types;
using SFA.DAS.ProviderApprenticeshipsService.Web.Validation;
using SFA.DAS.ProviderApprenticeshipsService.Web.Validation.Text;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Validation.ApprovedApprenticeship
{
    [TestFixture]
    public class WhenValidatingEndDate
    {
        private IApprovedApprenticeshipValidator _validator;
        private Mock<ICurrentDateTime> _currentDateTime;
        private Mock<IAcademicYearValidator> _mockAcademicYearValidator;
        private CreateApprenticeshipUpdateViewModel _createApprenticeshipUpdateViewModel;

        private const string FieldName = "EndDate";

        [SetUp]
        public void BaseSetup()
        {
            _mockAcademicYearValidator = new Mock<IAcademicYearValidator>();

            _currentDateTime = new Mock<ICurrentDateTime>();
            var academicYearProvider = new AcademicYearDateProvider(_currentDateTime.Object);

            _createApprenticeshipUpdateViewModel = new CreateApprenticeshipUpdateViewModel();

            _validator = new ApprovedApprenticeshipValidator(
                new WebApprenticeshipValidationText(academicYearProvider),
                _currentDateTime.Object,
                academicYearProvider,
                _mockAcademicYearValidator.Object,
                new Mock<IUlnValidator>().Object);
        }

        [TestCase(1, 6, 2019, 1, 7, 2019)]
        [TestCase(1, 6, 2019, 1, 8, 2019)]
        public void ShouldFailValidationWhenEndDateMonthInFuture(
            int nowDay, int nowMonth, int nowYear,
            int? endDay, int? endMonth, int? endYear)
        {
            const string expected = "The end date must not be in the future";

            _currentDateTime.Setup(x => x.Now).Returns(new DateTime(nowYear, nowMonth, nowDay));
            _createApprenticeshipUpdateViewModel.EndDate = new DateTimeViewModel(endDay, endMonth, endYear);

            var result = _validator.ValidateApprovedEndDate(_createApprenticeshipUpdateViewModel);

            Assert.IsTrue(result.ContainsKey(FieldName));
            Assert.AreEqual(expected, result[FieldName]);
        }

        [TestCase(1, 6, 2019, 1, 6, 2019)]
        [TestCase(1, 6, 2019, 1, 5, 2019)]
        [TestCase(15, 6, 2019, 1, 6, 2019)]
        [TestCase(1, 6, 2019, 15, 6, 2019)]
        public void ShouldPassValidationWhenEndDateIsCurrentMonthOrInPast(
            int nowDay, int nowMonth, int nowYear,
            int? endDay, int? endMonth, int? endYear)
        {
            _currentDateTime.Setup(x => x.Now).Returns(new DateTime(nowYear, nowMonth, nowDay));
            _createApprenticeshipUpdateViewModel.EndDate = new DateTimeViewModel(endDay, endMonth, endYear);

            var result = _validator.ValidateApprovedEndDate(_createApprenticeshipUpdateViewModel);

            Assert.IsFalse(result.ContainsKey(FieldName));
        }
    }
}
