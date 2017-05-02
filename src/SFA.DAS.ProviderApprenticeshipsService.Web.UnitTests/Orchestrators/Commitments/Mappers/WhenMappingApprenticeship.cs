using System;

using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;

using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.Apprenticeship.Types;
using SFA.DAS.Commitments.Api.Types.DataLock.Types;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.Mappers;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Orchestrators.Commitments.Mappers
{
    [TestFixture]
    public class WhenMappingApprenticeship
    {
        private Mock<IHashingService> _hashingService;

        private ApprenticeshipMapper _mapper;

        private Apprenticeship _model;

        [SetUp]
        public void Arrange()
        {
            _hashingService = new Mock<IHashingService>();
            _hashingService.Setup(x => x.HashValue(It.IsAny<long>())).Returns("hashed");

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
                             EndDate = DateTime.Now.AddMonths(26),
                             TrainingCode = "code-training",
                             TrainingName = "Training name",
                             TrainingType = TrainingType.Framework,
                             ULN = "1112223301"
                         };
            _mapper = new ApprenticeshipMapper(_hashingService.Object, Mock.Of<IMediator>());
        }

        [Test]
        public void ShouldMapToViewModel()
        {
            var viewModel = _mapper.MapFrom(_model);

            viewModel.HashedApprenticeshipId.Should().Be("hashed");
            viewModel.FirstName.Should().Be("First name");
            viewModel.LastName.Should().Be("Last name");

            viewModel.DateOfBirth.Should().Be(new DateTime(1998, 12, 08));
            viewModel.Uln.Should().Be("1112223301");
            viewModel.StartDate.Should().BeCloseTo(DateTime.Now.AddMonths(2), 10 * 1000);

            viewModel.EndDate.Should().BeCloseTo(DateTime.Now.AddMonths(26), 10 * 1000);
            viewModel.TrainingName.Should().Be("Training name");
            viewModel.Cost.Should().Be(1700);

            viewModel.EmployerName.Should().Be("LegalEntityName");
            viewModel.CohortReference.Should().Be("hashed");
            viewModel.ProviderReference.Should().Be("Provider ref");
        }

        [Test]
        public void ShouldNotEnableEditWhenStartDateInPast()
        {
            _model.StartDate = DateTime.Now.AddMonths(-5);
            _model.PaymentStatus = PaymentStatus.Active;

            var viewModel = _mapper.MapFrom(_model);

            viewModel.EnableEdit.Should().BeFalse();
        }

        [Test]
        public void ShouldNotEnableEditWhenCanceled()
        {
            _model.PaymentStatus = PaymentStatus.Withdrawn;
            var viewModel = _mapper.MapFrom(_model);

            viewModel.EnableEdit.Should().BeFalse();
        }

        [Test]
        public void ShouldHaveUpdatesToReview()
        {
            _model.PendingUpdateOriginator = Originator.Employer;
            var viewModel = _mapper.MapFrom(_model);

            viewModel.EnableEdit.Should().BeFalse();
            viewModel.PendingChanges.Should().Be(PendingChanges.ReadyForApproval);
        }

        [Test]
        public void ShouldHaveNotUpdatesToReviewWithCanceled()
        {
            _model.PendingUpdateOriginator = Originator.Employer;
            _model.PaymentStatus = PaymentStatus.Withdrawn;

            var viewModel = _mapper.MapFrom(_model);

            viewModel.PendingChanges.Should().Be(PendingChanges.ReadyForApproval);
        }

        [Test]
        public void ShouldHaveUpdatesWaitingForEmployer()
        {
            _model.PendingUpdateOriginator = Originator.Provider;
            var viewModel = _mapper.MapFrom(_model);

            viewModel.EnableEdit.Should().BeFalse();
            viewModel.PendingChanges.Should().Be(PendingChanges.WaitingForEmployer);
        }

        [Test]
        public void ShouldHaveUpdatesWaitingForEmployerEvenIfCanceled()
        {
            _model.PendingUpdateOriginator = Originator.Provider;
            _model.PaymentStatus = PaymentStatus.Withdrawn;

            var viewModel = _mapper.MapFrom(_model);

            viewModel.EnableEdit.Should().BeFalse();
            viewModel.PendingChanges.Should().Be(PendingChanges.WaitingForEmployer);
        }

        [Test]
        public void ShouldHaveStatusTextForFututreStart()
        {
            var viewModel = _mapper.MapFrom(_model);

            viewModel.Status.Should().Be("Waiting to start");
        }

        [Test]
        public void ShouldHaveStatusTextWhenStarted()
        {
            _model.PaymentStatus = PaymentStatus.Active;
            _model.StartDate = DateTime.Now.AddMonths(-5);
            var viewModel = _mapper.MapFrom(_model);

            viewModel.Status.Should().Be("On programme");
        }

        [Test]
        public void ShouldHaveStatusTextWhenCanceled()
        {
            _model.PaymentStatus = PaymentStatus.Withdrawn;
            _model.StartDate = DateTime.Now.AddMonths(-5);
            var viewModel = _mapper.MapFrom(_model);

            viewModel.Status.Should().Be("Stopped");
        }

        [Test]
        public void ShouldHaveStatusTextWhenPaused()
        {
            _model.PaymentStatus = PaymentStatus.Paused;
            _model.StartDate = DateTime.Now.AddMonths(-5);
            var viewModel = _mapper.MapFrom(_model);

            viewModel.Status.Should().Be("Paused");
        }

        [Test]
        public void ShouldHaveStatusTextWhenCompleted()
        {
            _model.PaymentStatus = PaymentStatus.Completed;
            _model.StartDate = DateTime.Now.AddMonths(-5);
            var viewModel = _mapper.MapFrom(_model);

            viewModel.Status.Should().Be("Completed");
        }

        [Test]
        public void ShouldNotHaveRecordStatus()
        {
            _model.PendingUpdateOriginator = null;
            var viewModel = _mapper.MapFrom(_model);

            viewModel.RecordStatus.Should().Be(string.Empty);
        }

        [Test]
        public void ShouldHaveRecordStatusFromEmployer()
        {
            _model.PendingUpdateOriginator = Originator.Employer;
            var viewModel = _mapper.MapFrom(_model);

            viewModel.RecordStatus.Should().Be("Changes for review");
        }

        [Test]
        public void ShouldHaveRecordStatusFromProvider()
        {
            _model.PendingUpdateOriginator = Originator.Provider;
            var viewModel = _mapper.MapFrom(_model);

            viewModel.RecordStatus.Should().Be("Changes pending");
        }

        [TestCase(TriageStatus.Change)]
        [TestCase(TriageStatus.FixIlr)]
        [TestCase(TriageStatus.Restart)]
        [TestCase(TriageStatus.Unknown)]
        [TestCase(null, true)]
        public void ShouldDisableEditIfDataLock(TriageStatus? triageStatus, bool expectedEnabled = false)
        {
            _model.PendingUpdateOriginator = null;
            _model.PaymentStatus = PaymentStatus.Active;
            _model.DataLockTriageStatus = triageStatus;

            var viewModel = _mapper.MapFrom(_model);
            viewModel.EnableEdit.Should().Be(expectedEnabled);
        }

        [TestCase(4)]
        [TestCase(8)]
        [TestCase(12)]
        [TestCase(16)]
        [TestCase(32)]
        public void ShouldMapErrorCodeToRestart(int dataLockErrorCode)
        {
            var result = _mapper.MapErrorType((DataLockErrorCode)dataLockErrorCode);
            result.Should().Be(DataLockErrorType.RestartRequire);
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
            result.Should().Be(DataLockErrorType.RestartRequire);
        }

        public void ShouldMapErrorCodeToNoneIfEmppty()
        {
            var result = _mapper.MapErrorType(0);
            result.Should().Be(DataLockErrorType.None);
        }
    }
}