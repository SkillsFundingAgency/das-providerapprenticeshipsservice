using System;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
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
    public class WhenCallingValidateApprovedEndDate
    {
        private IApprovedApprenticeshipValidator _validator;
        private Mock<ICurrentDateTime> _currentDateTime;
        private Mock<IAcademicYearValidator> _mockAcademicYearValidator;
        private CreateApprenticeshipUpdateViewModel _createApprenticeshipUpdateViewModel;

        private const string FieldName = "EndDate";

        [SetUp]
        public void Setup()
        {
            _mockAcademicYearValidator = new Mock<IAcademicYearValidator>();

            _currentDateTime = new Mock<ICurrentDateTime>();
            _currentDateTime
                .Setup(x => x.Now)
                .Returns(new DateTime(2018, 11, 5));

            var academicYearProvider = new AcademicYearDateProvider(_currentDateTime.Object);

            _createApprenticeshipUpdateViewModel = new CreateApprenticeshipUpdateViewModel();

            _validator = new ApprovedApprenticeshipValidator(
                new WebApprenticeshipValidationText(academicYearProvider),
                _currentDateTime.Object,
                academicYearProvider,
                _mockAcademicYearValidator.Object,
                new Mock<IUlnValidator>().Object,
                Mock.Of<IMediator>());
        }

        /// <remarks>
        /// Through the UI, with the other end date validation rules in place, validation would fail if no end date was supplied.
        /// Passing validation here refers to the validation in the ValidateApprovedEndDate method only!
        /// </remarks>
        [TestCase(true)]
        [TestCase(false)]
        public void ShouldPassValidationWhenNoEndDateSupplied(bool hasHadDataLockSuccess)
        {
            _currentDateTime.Setup(x => x.Now).Returns(new DateTime(2019, 1, 1));
            _createApprenticeshipUpdateViewModel.OriginalApprenticeship = new Apprenticeship { HasHadDataLockSuccess = hasHadDataLockSuccess };
            _createApprenticeshipUpdateViewModel.EndDate = new DateTimeViewModel();

            var result = _validator.ValidateApprovedEndDate(_createApprenticeshipUpdateViewModel);

            Assert.IsFalse(result.ContainsKey(FieldName));
        }

        /// <remarks>
        /// Passing validation here refers to the validation in the ValidateApprovedEndDate method only!
        /// </remarks>
        [TestCase(true)]
        [TestCase(false)]
        public void ShouldPassValidationWhenEndDateHasntChanged(bool hasHadDataLockSuccess)
        {
            _currentDateTime.Setup(x => x.Now).Returns(new DateTime(2019, 1, 1));
            _createApprenticeshipUpdateViewModel.OriginalApprenticeship = new Apprenticeship { HasHadDataLockSuccess = hasHadDataLockSuccess };
            _createApprenticeshipUpdateViewModel.EndDate = null;

            var result = _validator.ValidateApprovedEndDate(_createApprenticeshipUpdateViewModel);

            Assert.IsFalse(result.ContainsKey(FieldName));
        }

        [TestCase(1, 6, 2019, 1, 7, 2019)]
        [TestCase(1, 6, 2019, 1, 8, 2019)]
        public void AndHasHadDataLockSuccessShouldFailValidationWhenEndDateMonthInFuture(
            int nowDay, int nowMonth, int nowYear,
            int? endDay, int? endMonth, int? endYear)
        {
            const string expected = "The end date must not be in the future";

            _currentDateTime.Setup(x => x.Now).Returns(new DateTime(nowYear, nowMonth, nowDay));
            _createApprenticeshipUpdateViewModel.OriginalApprenticeship = new Apprenticeship {HasHadDataLockSuccess = true};
            _createApprenticeshipUpdateViewModel.EndDate = new DateTimeViewModel(endDay, endMonth, endYear);

            var result = _validator.ValidateApprovedEndDate(_createApprenticeshipUpdateViewModel);

            Assert.IsTrue(result.ContainsKey(FieldName));
            Assert.AreEqual(expected, result[FieldName]);
        }

        [TestCase(1, 6, 2019, 1, 6, 2019)]
        [TestCase(1, 6, 2019, 1, 5, 2019)]
        [TestCase(15, 6, 2019, 1, 6, 2019)]
        [TestCase(1, 6, 2019, 15, 6, 2019)]
        public void AndHasHadDataLockSuccessShouldPassValidationWhenEndDateIsCurrentMonthOrInPast(
            int nowDay, int nowMonth, int nowYear,
            int? endDay, int? endMonth, int? endYear)
        {
            _currentDateTime.Setup(x => x.Now).Returns(new DateTime(nowYear, nowMonth, nowDay));
            _createApprenticeshipUpdateViewModel.OriginalApprenticeship = new Apprenticeship { HasHadDataLockSuccess = true };
            _createApprenticeshipUpdateViewModel.EndDate = new DateTimeViewModel(endDay, endMonth, endYear);

            var result = _validator.ValidateApprovedEndDate(_createApprenticeshipUpdateViewModel);

            Assert.IsFalse(result.ContainsKey(FieldName));
        }

        [TestCase(1, 6, 2019, 1, 7, 2019)]
        [TestCase(1, 6, 2019, 1, 8, 2019)]
        public void AndHasNotHadDataLockSuccessShouldPassValidationWhenEndDateMonthInFuture(
            int nowDay, int nowMonth, int nowYear,
            int? endDay, int? endMonth, int? endYear)
        {
            _currentDateTime.Setup(x => x.Now).Returns(new DateTime(nowYear, nowMonth, nowDay));
            _createApprenticeshipUpdateViewModel.OriginalApprenticeship = new Apprenticeship { HasHadDataLockSuccess = false };
            _createApprenticeshipUpdateViewModel.EndDate = new DateTimeViewModel(endDay, endMonth, endYear);

            var result = _validator.ValidateApprovedEndDate(_createApprenticeshipUpdateViewModel);

            Assert.IsFalse(result.ContainsKey(FieldName));
        }
    }
}
