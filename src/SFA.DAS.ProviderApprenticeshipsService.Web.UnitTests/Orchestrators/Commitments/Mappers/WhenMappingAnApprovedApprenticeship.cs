using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.Apprenticeship.Types;
using SFA.DAS.Commitments.Api.Types.ApprovedApprenticeship;
using SFA.DAS.Commitments.Api.Types.DataLock;
using SFA.DAS.Commitments.Api.Types.DataLock.Types;
using SFA.DAS.HashingService;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.Mappers;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Orchestrators.Commitments.Mappers
{
    [TestFixture]
    public class WhenMappingApprovedApprenticeship
    {
        private ApprovedApprenticeshipMapper _mapper;

        private ApprovedApprenticeship _source;

        private string _expectedHashedApprenticeshipId;
        private decimal _expectedCost;
        private string _expectedStatus;

        private Mock<IPaymentStatusMapper> _mockPaymentStatusMapper;
        private Mock<IHashingService> _mockHashingService;
        private Mock<ICurrentDateTime> _mockCurrentDateTime;

        private ApprovedApprenticeshipViewModel _result;

        [SetUp]
        public void Arrange()
        {
            _mockPaymentStatusMapper = new Mock<IPaymentStatusMapper>();
            _mockHashingService = new Mock<IHashingService>();
            _mockCurrentDateTime = new Mock<ICurrentDateTime>();

            _expectedHashedApprenticeshipId = "hashone";
            _expectedCost = 112;
            _expectedStatus = "test status";

            _mockHashingService.Setup(x => x.HashValue(It.IsAny<long>())).Returns(_expectedHashedApprenticeshipId);
            _mockPaymentStatusMapper.Setup(x => x.Map(It.IsAny<PaymentStatus>(), It.IsAny<DateTime?>())).Returns(_expectedStatus);

            _mapper = new ApprovedApprenticeshipMapper(_mockPaymentStatusMapper.Object, _mockHashingService.Object, _mockCurrentDateTime.Object);

            _source = new ApprovedApprenticeship
            {
                Id = 1,
                EndpointAssessorName = "EPA",
                HasHadDataLockSuccess = false,
                AccountLegalEntityPublicHashedId = "AGREEMENT_ID",
                LegalEntityName = "TEST_LEGAL_ENTITY_NAME",
                LegalEntityId = "TEST_LEGAL_ENTITY_ID",
                ProviderId = 3,
                ProviderName = "TEST_PROVIDER_NAME",
                UpdateOriginator = Originator.Provider,
                PaymentOrder = 2,
                ProviderRef = "PROVIDER_REF",
                EmployerRef = "EMPLOYER_REF",
                PaymentStatus = PaymentStatus.Active,
                StopDate = null,
                PauseDate = null,
                StartDate = new DateTime(2018, 1, 1),
                EndDate = new DateTime(2020, 12, 31),
                TrainingName = "TRAINING_NAME",
                TrainingCode = "TRAINING_CODE",
                TrainingType = TrainingType.Framework,
                ULN = "ULN",
                DateOfBirth = new DateTime(2000, 1, 1),
                LastName = "LAST_NAME",
                FirstName = "FIRST_NAME",
                TransferSenderId = null,
                EmployerAccountId = 4,
                CohortReference = "COHORT_REF",
                PriceEpisodes = new List<PriceHistory> { new PriceHistory { Cost = _expectedCost } },
                DataLocks = new List<DataLockStatus>
                {
                    new DataLockStatus{ ErrorCode = DataLockErrorCode.Dlock03, TriageStatus = TriageStatus.Unknown },
                    new DataLockStatus{ ErrorCode = DataLockErrorCode.Dlock07, TriageStatus = TriageStatus.Unknown },
                    new DataLockStatus{ ErrorCode = DataLockErrorCode.Dlock03, TriageStatus = TriageStatus.Restart },
                    new DataLockStatus{ ErrorCode = DataLockErrorCode.Dlock03, TriageStatus = TriageStatus.Change },
                    new DataLockStatus{ ErrorCode = DataLockErrorCode.Dlock07, TriageStatus = TriageStatus.Change }
                }
            };

            _result = _mapper.Map(_source);
        }

        [Test]
        public void ThenHashedApprenticeshipIdIsMappedCorrectly()
        {
            Assert.That(_result.HashedApprenticeshipId, Is.EqualTo(_expectedHashedApprenticeshipId));
        }

        [Test]
        public void ThenFirstNameIsMappedCorrectly()
        {
            Assert.That(_result.FirstName, Is.EqualTo(_source.FirstName));
        }

        [Test]
        public void ThenLastNameIsMappedCorrectly()
        {
            Assert.That(_result.LastName, Is.EqualTo(_source.LastName));
        }

        [Test]
        public void ThenDateOfBirthIsMappedCorrectly()
        {
            Assert.That(_result.DateOfBirth, Is.EqualTo(_source.DateOfBirth));
        }

        [Test]
        public void ThenUlnIsMappedCorrectly()
        {
            Assert.That(_result.Uln, Is.EqualTo(_source.ULN));
        }

        [Test]
        public void ThenStartDateIsMappedCorrectly()
        {
            Assert.That(_result.StartDate, Is.EqualTo(_source.StartDate));
        }

        [Test]
        public void ThenEndDateIsMappedCorrectly()
        {
            Assert.That(_result.EndDate, Is.EqualTo(_source.EndDate));
        }

        [Test]
        public void ThenStopDateIsMappedCorrectly()
        {
            Assert.That(_result.StopDate, Is.EqualTo(_source.StopDate));
        }

        [Test]
        public void ThenTrainingNameIsMappedCorrectly()
        {
            Assert.That(_result.TrainingName, Is.EqualTo(_source.TrainingName));
        }

        [Test]
        public void ThenEmployerNameIsMappedCorrectly()
        {
            Assert.That(_result.EmployerName, Is.EqualTo(_source.LegalEntityName));
        }

        [Test]
        public void ThenCohortReferenceIsMappedCorrectly()
        {
            Assert.That(_result.CohortReference, Is.EqualTo(_source.CohortReference));
        }

        [Test]
        public void ThenAccountLegalEntityPublicHashedIdIsMappedCorrectly()
        {
            Assert.That(_result.AccountLegalEntityPublicHashedId, Is.EqualTo(_source.AccountLegalEntityPublicHashedId));
        }

        [Test]
        public void ThenProviderReferenceIsMappedCorrectly()
        {
            Assert.That(_result.ProviderReference, Is.EqualTo(_source.ProviderRef));
        }

        [Test]
        public void ThenHasHadDataLockSuccessIsMappedCorrectly()
        {
            Assert.That(_result.HasHadDataLockSuccess, Is.EqualTo(_source.HasHadDataLockSuccess));
        }

        [Test]
        public void ThenCurrentCostIsMappedCorrectly()
        {
            Assert.That(_result.CurrentCost, Is.EqualTo(_expectedCost));
        }

        [Test]
        public void ThenStatusIsMappedCorrectly()
        {
            Assert.That(_result.Status, Is.EqualTo(_expectedStatus));
        }

        [Test]
        public void ThenPendingChangeIsMappedCorrectly()
        {
            Assert.That(_result.PendingChanges, Is.EqualTo(PendingChanges.WaitingForEmployer));
        }

        [Test]
        public void ThenEnableEditIsMappedCorrectly()
        {
            Assert.That(_result.EnableEdit, Is.False);
        }

        [Test]
        public void ThenDataLockCourseIsMappedCorrectly()
        {
            Assert.That(_result.DataLockCourse, Is.True);
        }

        [Test]
        public void ThenDataLockPriceIsMappedCorrectly()
        {
            Assert.That(_result.DataLockCourse, Is.True);
        }

        [Test]
        public void ThenDataLockCourseTriagedIsMappedCorrectly()
        {
            Assert.That(_result.DataLockCourse, Is.True);
        }

        [Test]
        public void ThenDataLockCourseChangeTriagedIsMappedCorrectly()
        {
            Assert.That(_result.DataLockCourse, Is.True);
        }

        [Test]
        public void ThenDataLockPriceTriagedIsMappedCorrectly()
        {
            Assert.That(_result.DataLockCourse, Is.True);
        }
    }
}