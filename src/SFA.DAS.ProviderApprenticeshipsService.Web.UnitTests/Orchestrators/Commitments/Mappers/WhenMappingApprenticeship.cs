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
using SFA.DAS.Commitments.Api.Types.DataLock;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetFrameworks;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetStandards;
using SFA.DAS.ProviderApprenticeshipsService.Domain;
using SFA.DAS.Commitments.Api.Types.DataLock.Types;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Types;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.Mappers;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Services;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;

using TrainingType = SFA.DAS.Commitments.Api.Types.Apprenticeship.Types.TrainingType;
using SFA.DAS.NLog.Logger;
using SFA.DAS.HashingService;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Orchestrators.Commitments.Mappers
{
    [TestFixture]
    public class WhenMappingApprenticeship
    {
        private Mock<IHashingService> _hashingService;
        private Apprenticeship _model;
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

            mockMediator.Setup(m => m.SendAsync(It.IsAny<GetStandardsQueryRequest>()))
                .ReturnsAsync(new GetStandardsQueryResponse
                {
                    Standards = new List<Standard>
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
            mockMediator.Setup(m => m.SendAsync(It.IsAny<GetFrameworksQueryRequest>()))
                .ReturnsAsync(new GetFrameworksQueryResponse { Frameworks = new List<Framework>() });

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
            var viewModel = _mapper.MapApprenticeship(_model);

            viewModel.HashedApprenticeshipId.Should().Be("hashed");
            viewModel.FirstName.Should().Be("First name");
            viewModel.LastName.Should().Be("Last name");

            viewModel.DateOfBirth.DateTime.Should().Be(new DateTime(1998, 12, 08));
            viewModel.ULN.Should().Be("1112223301");
            viewModel.StartDate.DateTime.Should().BeCloseTo(new DateTimeViewModel(DateTime.Now.AddMonths(2)).DateTime.Value, 100 * 1000);

            viewModel.EndDate.DateTime.Should().BeCloseTo(new DateTimeViewModel(DateTime.Now.AddMonths(26)).DateTime.Value, 10 * 1000);
            viewModel.TrainingName.Should().Be("Training name");
            viewModel.Cost.Should().Be("1700");

            viewModel.ProviderRef.Should().Be("Provider ref");
        }

        [Test]
        public async Task ShouldMapToApprenticeship()
        {
            var viewModel = _mapper.MapApprenticeship(_model);
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
            var viewModel = _mapper.MapApprenticeship(_model);
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
            var viewModel = _mapper.MapApprenticeship(apprenticeship);

            viewModel.IsLockedForUpdate.Should().BeFalse();
            viewModel.HasStarted.Should().BeTrue();
        }

        [Test]
        public void ShouldHaveLockedStatusIfDataLocksSuccesFound()
        {
            var apprenticeship = new Apprenticeship { StartDate = _now.AddMonths(-1), HasHadDataLockSuccess = true };
            var viewModel = _mapper.MapApprenticeship(apprenticeship);

            viewModel.IsLockedForUpdate.Should().BeTrue();
            viewModel.HasStarted.Should().BeTrue();
        }

        [Test]
        public void ShouldHaveLockedStatusIfPastCutOffDate()
        {
            _mockAcademicYearValidator.Setup(m => m.IsAfterLastAcademicYearFundingPeriod).Returns(true);
            _mockAcademicYearValidator.Setup(m => m.Validate(It.IsAny<DateTime>())).Returns(AcademicYearValidationResult.NotWithinFundingPeriod);

            var apprenticeship = new Apprenticeship { StartDate = _now.AddMonths(-5), HasHadDataLockSuccess = false };
            var viewModel = _mapper.MapApprenticeship(apprenticeship);

            viewModel.IsLockedForUpdate.Should().BeTrue();
            viewModel.HasStarted.Should().BeTrue();
        }
    }
}