using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;

using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.Apprenticeship.Types;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.Commitments.Api.Types.DataLock.Types;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Types;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.Mappers;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Services;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;

using TrainingType = SFA.DAS.Commitments.Api.Types.Apprenticeship.Types.TrainingType;
using SFA.DAS.NLog.Logger;
using SFA.DAS.HashingService;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetTrainingProgrammes;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models.AcademicYear;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models.ApprenticeshipCourse;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Orchestrators.Commitments.Mappers
{
    [TestFixture]
    public class WhenMappingApprenticeship
    {
        private Mock<IHashingService> _hashingService;
        private Apprenticeship _model;
        private CommitmentView _commitment;
        private ApprenticeshipMapper _mapper;
        private Mock<IAcademicYearValidator> _mockAcademicYearValidator;

        private DateTime _now;

        [SetUp]
        public void Arrange()
        {
            _now = new DateTime(DateTime.Now.Year, 11, 01);
            _hashingService = new Mock<IHashingService>();
            var mockMediator = new Mock<IMediator>();
            _mockAcademicYearValidator = new Mock<IAcademicYearValidator>();

            _hashingService.Setup(x => x.HashValue(It.IsAny<long>())).Returns("hashed");
            _hashingService.Setup(x => x.DecodeValue("hashed")).Returns(1998);

            _commitment = new CommitmentView();

            _model = new Apprenticeship

            {
                AgreementStatus = AgreementStatus.BothAgreed,
                CanBeApproved = false,
                CommitmentId = 222,
                Cost = 1700,
                DateOfBirth = new DateTime(1998, 12, 08),
                EmployerAccountId = 555,
                EmployerRef = "",
                FirstName = "First name",
                Id = 1,
                LastName = "Last name",
                LegalEntityName = "LegalEntityName",
                NINumber = "SE4445566O",
                PaymentStatus = PaymentStatus.Active,
                PendingUpdateOriginator = Originator.Provider,
                ProviderId = 666,
                ProviderName = "Provider name",
                ProviderRef = "Provider ref",
                Reference = "ABBA12",
                StartDate = DateTime.Now.AddMonths(2),
                StopDate = DateTime.Now.AddMonths(6),
                EndDate = DateTime.Now.AddMonths(26),
                TrainingCode = "code-training",
                TrainingName = "Training name",
                TrainingType = TrainingType.Framework,
                ULN = "1112223301"
            };

            mockMediator.Setup(m => m.SendAsync(It.IsAny<GetTrainingProgrammesQueryRequest>()))
                .ReturnsAsync(new GetTrainingProgrammesQueryResponse { TrainingProgrammes = new List<ITrainingProgramme>
                {
                    new Standard
                    {
                        Duration = 12,
                        Id = "code-training",
                        Level = 3,
                        Title = "Fake training"
                    }
                }
                });

            _mapper = new ApprenticeshipMapper(
                _hashingService.Object, 
                mockMediator.Object,
                new CurrentDateTime(_now),
                Mock.Of<ILog>(),
                _mockAcademicYearValidator.Object
                );
        }

        [Test]
        public void ShouldMapToApprenticeshipViewModel()
        {
            var viewModel = _mapper.MapApprenticeship(_model, _commitment);

            viewModel.HashedApprenticeshipId.Should().Be("hashed");
            viewModel.FirstName.Should().Be("First name");
            viewModel.LastName.Should().Be("Last name");

            viewModel.DateOfBirth.DateTime.Should().Be(new DateTime(1998, 12, 08));
            viewModel.ULN.Should().Be("1112223301");
            viewModel.StartDate.DateTime.Should().BeCloseTo(new DateTimeViewModel(DateTime.Now.AddMonths(2)).DateTime.Value, 100 * 1000);
            viewModel.StopDate.DateTime.Should().BeCloseTo(new DateTimeViewModel(DateTime.Now.AddMonths(6)).DateTime.Value, 100 * 1000);
            viewModel.EndDate.DateTime.Should().BeCloseTo(new DateTimeViewModel(DateTime.Now.AddMonths(26)).DateTime.Value, 10 * 1000);
            viewModel.TrainingName.Should().Be("Training name");
            viewModel.Cost.Should().Be("1700");

            viewModel.ProviderRef.Should().Be("Provider ref");
        }

        [Test]
        public async Task ShouldMapToApprenticeship()
        {
            var viewModel = _mapper.MapApprenticeship(_model, _commitment);
            var model = await _mapper.MapApprenticeship(viewModel);

            model.Id.Should().Be(1998);
            model.FirstName.Should().Be("First name");
            model.LastName.Should().Be("Last name");

            model.DateOfBirth.Should().Be(new DateTime(1998, 12, 08));
            model.ULN.Should().Be("1112223301");
            model.StartDate.Should().BeCloseTo(new DateTimeViewModel(DateTime.Now.AddMonths(2)).DateTime.Value, 10 * 1000);
            model.EndDate.Should().BeCloseTo(new DateTimeViewModel(DateTime.Now.AddMonths(26)).DateTime.Value, 10 * 1000);
            model.TrainingName.Should().Be("Fake training");
            model.Cost.Should().Be(1700);

            model.ProviderRef.Should().Be("Provider ref");
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("%�$^")]
        [TestCase("79228162514264337593543950336", Description = "Decimal max val + 1")]
        public async Task ShouldMapToApprenticeshipCostToNullIfNullOrEmpty(string cost)
        {
            var viewModel = _mapper.MapApprenticeship(_model, new CommitmentView());
            viewModel.Cost = cost;
            var model = await _mapper.MapApprenticeship(viewModel);

            model.Cost.Should().Be(null);
        }

        [Test]
        public void ShouldDisableEditIfDataLockCourse()
        {
            _model.PendingUpdateOriginator = null;
            _model.PaymentStatus = PaymentStatus.Active;
            _model.DataLockCourse = true;

            var viewModel = _mapper.MapApprenticeshipDetails(_model);
            viewModel.EnableEdit.Should().Be(false);
        }

        [Test]
        public void ShouldDisableEditIfDataLockCourseTriaged()
        {
            _model.PendingUpdateOriginator = null;
            _model.PaymentStatus = PaymentStatus.Active;
            _model.DataLockCourseTriaged = true;

            var viewModel = _mapper.MapApprenticeshipDetails(_model);
            viewModel.EnableEdit.Should().Be(false);
        }

        [Test]
        public void ShouldDisableEditIfDataLockPrice()
        {
            _model.PendingUpdateOriginator = null;
            _model.PaymentStatus = PaymentStatus.Active;
            _model.DataLockPrice = true;

            var viewModel = _mapper.MapApprenticeshipDetails(_model);
            viewModel.EnableEdit.Should().Be(false);
        }

        [Test]
        public void ShouldDisableEditIfDataLockPriceTriaged()
        {
            _model.PendingUpdateOriginator = null;
            _model.PaymentStatus = PaymentStatus.Active;
            _model.DataLockPriceTriaged = true;

            var viewModel = _mapper.MapApprenticeshipDetails(_model);
            viewModel.EnableEdit.Should().Be(false);
        }

        [TestCase(4)]
        [TestCase(8)]
        [TestCase(12)]
        [TestCase(16)]
        [TestCase(32)]
        public void ShouldMapErrorCodeToRestart(int dataLockErrorCode)
        {
            var result = _mapper.MapErrorType((DataLockErrorCode)dataLockErrorCode);
            result.Should().Be(DataLockErrorType.RestartRequired);
        }

        [TestCase(64, Description = "DLock_07")]
        [TestCase(256, Description = "DLock_09")]
        [TestCase(320, Description = "DLock_07 and DLock_09")]
        public void ShouldMapErrorCodeToUpdateNeeded(int dataLockErrorCode)
        {
            var result = _mapper.MapErrorType((DataLockErrorCode)dataLockErrorCode);
            result.Should().Be(DataLockErrorType.UpdateNeeded);
        }

        [Test(Description = "When DLock 07 and 03 should only return restart")]
        public void ShouldMapErrorCodeToRestartIfBothRestartAndUpdateNeededInErrorCode()
        {
            var result = _mapper.MapErrorType((DataLockErrorCode)68);
            result.Should().Be(DataLockErrorType.RestartRequired);
        }

        [Test]
        public void ShouldMapErrorCodeToNoneIfEmppty()
        {
            var result = _mapper.MapErrorType(0);
            result.Should().Be(DataLockErrorType.None);
        }

        // ---------

        [Test]
        public void ShouldNotHaveLockedStatusIfNoDataLockSuccessFound()
        {
            var apprenticeship = new Apprenticeship { StartDate = _now.AddMonths(-1), HasHadDataLockSuccess = false};
            var viewModel = _mapper.MapApprenticeship(apprenticeship, _commitment);

            viewModel.IsLockedForUpdate.Should().BeFalse();
            viewModel.IsEndDateLockedForUpdate.Should().BeFalse();
            viewModel.HasStarted.Should().BeTrue();
        }

        [Test]
        public void ShouldHaveLockedStatusIfDataLocksSuccesFound()
        {
            var apprenticeship = new Apprenticeship { StartDate = _now.AddMonths(-1), HasHadDataLockSuccess = true, PaymentStatus = PaymentStatus.Active };
            var viewModel = _mapper.MapApprenticeship(apprenticeship, _commitment);

            viewModel.IsLockedForUpdate.Should().BeTrue();
            viewModel.IsEndDateLockedForUpdate.Should().BeTrue();
            viewModel.HasStarted.Should().BeTrue();
        }

        [Test]
        public void ShouldHaveLockedStatusIfPastCutOffDate()
        {
            _mockAcademicYearValidator.Setup(m => m.IsAfterLastAcademicYearFundingPeriod).Returns(true);
            _mockAcademicYearValidator.Setup(m => m.Validate(It.IsAny<DateTime>())).Returns(AcademicYearValidationResult.NotWithinFundingPeriod);

            var apprenticeship = new Apprenticeship { StartDate = _now.AddMonths(-5), HasHadDataLockSuccess = false, PaymentStatus = PaymentStatus.Active };
            var viewModel = _mapper.MapApprenticeship(apprenticeship, _commitment);

            viewModel.IsLockedForUpdate.Should().BeTrue();
            viewModel.IsEndDateLockedForUpdate.Should().BeTrue();
            viewModel.HasStarted.Should().BeTrue();
        }

        [Test]
        public void ShouldHaveLockedStatusIfApprovedTransferFundedWithSuccessfulIlrSubmissionAndCourseNotYetStarted()
        {
            var apprenticeship = new Apprenticeship { StartDate = _now.AddMonths(3), HasHadDataLockSuccess = true, PaymentStatus = PaymentStatus.Active };
            var commitment = new CommitmentView { TransferSender = new TransferSender { TransferApprovalStatus = TransferApprovalStatus.Approved } };

            var viewModel = _mapper.MapApprenticeship(apprenticeship, commitment);

            viewModel.IsLockedForUpdate.Should().BeTrue();
            viewModel.IsEndDateLockedForUpdate.Should().BeTrue();
        }

        [Test]
        public void ShouldHaveTransferFlagSetIfCommitmentHasTransferSender()
        {
            var apprenticeship = new Apprenticeship { StartDate = _now.AddMonths(-5), HasHadDataLockSuccess = false };
            _commitment.TransferSender = new TransferSender{ Id = 123L };

            var viewModel = _mapper.MapApprenticeship(apprenticeship, _commitment);

            viewModel.IsPaidForByTransfer.Should().BeTrue();
        }

        [TestCase(true, false, true, TransferApprovalStatus.Approved)]
        [TestCase(false, true, true, TransferApprovalStatus.Approved)]
        [TestCase(false, false, true, TransferApprovalStatus.Pending)]
        [TestCase(false, true, true, TransferApprovalStatus.Pending)]
        [TestCase(false, false, true, TransferApprovalStatus.Rejected)]
        [TestCase(false, true, true, TransferApprovalStatus.Rejected)]
        [TestCase(false, false, false, null)]
        [TestCase(false, true, false, null)]
        public void ThenIsUpdateLockedForStartDateAndCourseShouldBeSetCorrectly(bool expected, bool dataLockSuccess, bool transferSender, TransferApprovalStatus? transferApprovalStatus)
        {
            var apprenticeship = new Apprenticeship { HasHadDataLockSuccess = dataLockSuccess, PaymentStatus = PaymentStatus.Active };
            var commitment = new CommitmentView();

            if (transferSender)
            {
                commitment.TransferSender = new TransferSender { TransferApprovalStatus = transferApprovalStatus };
            }

            var viewModel = _mapper.MapApprenticeship(apprenticeship, commitment);

            Assert.AreEqual(expected, viewModel.IsUpdateLockedForStartDateAndCourse);
        }

        [TestCase(true, false, true, true, true, AcademicYearValidationResult.NotWithinFundingPeriod, Description = "No valid change, must be locked (locked=true)")]
        [TestCase(false, false, true, false, true, AcademicYearValidationResult.NotWithinFundingPeriod, Description = "Should override to enabled (lockdown=false)")]
        [TestCase(false, true, false, true, true, AcademicYearValidationResult.NotWithinFundingPeriod, Description = "Same lockdown status as other lockable fields (lockdown=isLockedForUpdate)")]
        [TestCase(true, true, false, false, true, AcademicYearValidationResult.NotWithinFundingPeriod, Description = "Same lockdown status as other lockable fields (lockdown=isLockedForUpdate)")]
        [TestCase(true, false, true, true, false, AcademicYearValidationResult.NotWithinFundingPeriod, Description = "No valid change, must be locked (locked=true)")]
        [TestCase(false, false, true, false, false, AcademicYearValidationResult.NotWithinFundingPeriod, Description = "Should override to enabled (lockdown=false)")]
        [TestCase(false, true, false, true, false, AcademicYearValidationResult.NotWithinFundingPeriod, Description = "Same lockdown status as other lockable fields (lockdown=isLockedForUpdate)")]
        [TestCase(false, true, false, false, false, AcademicYearValidationResult.NotWithinFundingPeriod, Description = "Same lockdown status as other lockable fields (lockdown=isLockedForUpdate)")]
        [TestCase(true, false, true, true, true, AcademicYearValidationResult.Success, Description = "No valid change, must be locked (locked=true)")]
        [TestCase(false, false, true, false, true, AcademicYearValidationResult.Success, Description = "Should override to enabled (lockdown=false)")]
        [TestCase(false, true, false, true, true, AcademicYearValidationResult.Success, Description = "Same lockdown status as other lockable fields (lockdown=isLockedForUpdate)")]
        [TestCase(false, true, false, false, true, AcademicYearValidationResult.Success, Description = "Same lockdown status as other lockable fields (lockdown=isLockedForUpdate)")]
        [TestCase(true, false, true, true, false, AcademicYearValidationResult.Success, Description = "No valid change, must be locked (locked=true)")]
        [TestCase(false, false, true, false, false, AcademicYearValidationResult.Success, Description = "Should override to enabled (lockdown=false)")]
        [TestCase(false, true, false, true, false, AcademicYearValidationResult.Success, Description = "Same lockdown status as other lockable fields (lockdown=isLockedForUpdate)")]
        [TestCase(false, true, false, false, false, AcademicYearValidationResult.Success, Description = "Same lockdown status as other lockable fields (lockdown=isLockedForUpdate)")]
        public void AndApprenticeshipApprovedThenIsEndDateLockedForUpdateShouldBeSetCorrectly(bool expected, bool unchanged,
            bool dataLockSuccess, bool isStartDateInFuture, bool isAfterLastAcademicYearFundingPeriod, AcademicYearValidationResult academicYearValidationResult)
        {
            _mockAcademicYearValidator.Setup(m => m.IsAfterLastAcademicYearFundingPeriod).Returns(isAfterLastAcademicYearFundingPeriod);
            _mockAcademicYearValidator.Setup(m => m.Validate(It.IsAny<DateTime>())).Returns(academicYearValidationResult);

            var apprenticeship = new Apprenticeship
            {
                HasHadDataLockSuccess = dataLockSuccess,
                StartDate = _now.AddMonths(isStartDateInFuture ? 1 : -1),
                PaymentStatus = PaymentStatus.Active
            };

            var commitment = new CommitmentView { AgreementStatus = AgreementStatus.BothAgreed };

            var viewModel = _mapper.MapApprenticeship(apprenticeship, commitment);

            Assert.AreEqual(expected, viewModel.IsEndDateLockedForUpdate);
            if (unchanged)
                Assert.AreEqual(viewModel.IsLockedForUpdate, viewModel.IsEndDateLockedForUpdate);
        }
    }
}