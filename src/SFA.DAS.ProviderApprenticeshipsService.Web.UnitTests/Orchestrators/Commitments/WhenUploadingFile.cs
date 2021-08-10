using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.Commitment.Types;
using SFA.DAS.Commitments.Api.Types.TrainingProgramme;
using SFA.DAS.Commitments.Api.Types.Validation;
using SFA.DAS.Commitments.Api.Types.Validation.Types;
using SFA.DAS.ProviderApprenticeshipsService.Application.Commands.BulkUploadApprenticeships;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetCommitment;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetOverlappingApprenticeships;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.BulkUpload;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.Mappers;
using CommitmentView = SFA.DAS.Commitments.Api.Types.Commitment.CommitmentView;
using SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Orchestrators.BulkUpload;
using SFA.DAS.HashingService;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetTrainingProgrammes;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models.ApprenticeshipCourse;
using static System.Text.Encoding;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetEmailOverlapingApprenticeships;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Orchestrators.Commitments
{
    [TestFixture]
    public sealed class WhenUploadingFile : BulkUploadTestBase
    {
        private const string HeaderLine = @"CohortRef,GivenNames,FamilyName,DateOfBirth,StdCode,StartDate,EndDate,TotalPrice,EPAOrgId,EmpRef,ProviderRef,ULN,EmailAddress,AgreementId";
        private BulkUploadOrchestrator _sut;
        private Mock<HttpPostedFileBase> _file;
        private Mock<IMediator> _mockMediator;
        private Mock<IReservationsService> _mockReservationsService;
        private Mock<IProviderCommitmentsLogger> _logger;

        [SetUp]
        public void Setup()
        {
            var mockHashingService = new Mock<IHashingService>();
            mockHashingService.Setup(x => x.DecodeValue(It.IsAny<string>())).Returns(123L);

            _file = new Mock<HttpPostedFileBase>();
            _file.Setup(m => m.FileName).Returns("APPDATA-20051030-213855.csv");
            _file.Setup(m => m.ContentLength).Returns(400);

            _mockMediator = new Mock<IMediator>();

            _mockMediator.Setup(m => m.Send(It.IsAny<GetTrainingProgrammesQueryRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetTrainingProgrammesQueryResponse { TrainingProgrammes = new List<TrainingProgramme>
                {
                    {
                        new TrainingProgramme {CourseCode = "2", Name = "Hej" }
                    },
                    {
                        new TrainingProgramme { CourseCode = "1-2-3" }
                    }
                } });

            _mockMediator.Setup(m => m.Send(It.IsAny<GetOverlappingApprenticeshipsQueryRequest>(), It.IsAny<CancellationToken>()))
                .Returns(
                    Task.Run(() => new GetOverlappingApprenticeshipsQueryResponse
                            {
                                Overlaps = new List<ApprenticeshipOverlapValidationResult>()
                            }));

            _mockReservationsService = new Mock<IReservationsService>();
            _mockReservationsService.Setup(rs => rs.IsAutoReservationEnabled(It.IsAny<long>(), It.IsAny<long?>())).ReturnsAsync(true);

            var uploadValidator = BulkUploadTestHelper.GetBulkUploadValidator(512);

            _logger = new Mock<IProviderCommitmentsLogger>();
            _logger.Setup(x => x.Info(It.IsAny<string>(), It.IsAny<long?>(), It.IsAny<long?>(), It.IsAny<long?>())).Verifiable();
            _logger.Setup(x => x.Error(It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<long?>(), It.IsAny<long?>(), It.IsAny<long?>())).Verifiable();

            var uploadFileParser = new BulkUploadFileParser(_logger.Object);
            var bulkUploader = new BulkUploader(_mockMediator.Object, uploadValidator, uploadFileParser, Mock.Of<IProviderCommitmentsLogger>());
            var bulkUploadMapper = new BulkUploadMapper(_mockMediator.Object);

            _sut = new BulkUploadOrchestrator(_mockMediator.Object, bulkUploader, mockHashingService.Object,
                bulkUploadMapper, Mock.Of<IProviderCommitmentsLogger>(), Mock.Of<IBulkUploadFileParser>(), _mockReservationsService.Object);
        }

        [Test]
        public async Task TestPerformance()
        {
            string HeaderLine = @"CohortRef,GivenNames,FamilyName,DateOfBirth,StdCode,StartDate,EndDate,TotalPrice,EPAOrgId,EmpRef,ProviderRef,AgreementId,EmailAddress,ULN";
            _mockMediator.Setup(m => m.Send(It.IsAny<GetCommitmentQueryRequest>(), It.IsAny<CancellationToken>()))
                    .Returns(Task.FromResult(new GetCommitmentQueryResponse
                    {
                        Commitment = new CommitmentView
                        {
                            AgreementStatus = AgreementStatus.NotAgreed,
                            EditStatus = EditStatus.ProviderOnly
                        }
                    }));
            
            const int upper = 40 * 1000;
            var testData = new List<string>();
            for (int i = 0; i < upper; i++)
            {
                var uln = (1000000000 + i).ToString();
                var email = $"apprentice{i}@test.com";
                testData.Add("\n\r ABBA123,Chris,Froberg,1998-12-08,0,2020-08-01,2025-08,1500,,Employer ref,Provider ref,XYZUR," + email + ',' + uln);
            }
            var str = HeaderLine + string.Join("", testData);

            var textStream = new MemoryStream(UTF8.GetBytes(str));
            _file.Setup(m => m.InputStream).Returns(textStream);

            var model = new UploadApprenticeshipsViewModel { Attachment = _file.Object, HashedCommitmentId = "ABBA123", ProviderId = 1234L, AccountLegalEntityPublicHashedId = "XYZUR" };
            var stopwatch = Stopwatch.StartNew();
            var r1 = await _sut.UploadFile("user123", model, new SignInUserModel());
            stopwatch.Stop(); Console.WriteLine($"Time TOTAL: {stopwatch.Elapsed.Seconds}");
            r1.RowLevelErrors.Count().Should().Be(80 * 1000);
            stopwatch.Elapsed.Seconds.Should().BeLessThan(7);
        }

        [Test]
        public async Task ShouldCallMediatorPassingInMappedApprenticeships()
        {
            const string dataLine = "\n\r ABBA123,Chris,Froberg,1998-12-08,2,2020-08-01,2025-08,1500,,Employer ref,Provider ref,1113335559,apprentice1@test.com,XYZUR";
          
            const string fileContents = HeaderLine + dataLine;
            var textStream = new MemoryStream(UTF8.GetBytes(fileContents));
            _file.Setup(m => m.InputStream).Returns(textStream);

            BulkUploadApprenticeshipsCommand commandArgument = null;
            _mockMediator.Setup(x => x.Send(It.IsAny<BulkUploadApprenticeshipsCommand>(), new CancellationToken()))
                .ReturnsAsync(new Unit())
                .Callback<IRequest<Unit>, CancellationToken>((command, token) => commandArgument = command.As<BulkUploadApprenticeshipsCommand>());

            _mockMediator.Setup(m => m.Send(It.IsAny<GetCommitmentQueryRequest>(), new CancellationToken()))
                .Returns(Task.FromResult(new GetCommitmentQueryResponse
                {
                    Commitment = new CommitmentView
                    {
                        AgreementStatus = AgreementStatus.NotAgreed,
                        EditStatus = EditStatus.ProviderOnly
                    }
                }));

            var model = new UploadApprenticeshipsViewModel { Attachment = _file.Object, HashedCommitmentId = "ABBA123", ProviderId = 111, AccountLegalEntityPublicHashedId = "XYZUR" };
            var signinUser = new SignInUserModel { DisplayName = "Bob", Email = "test@email.com" };

            await _sut.UploadFile("user123", model, signinUser);

            _mockMediator.Verify(x => x.Send(It.IsAny<BulkUploadApprenticeshipsCommand>(), new CancellationToken()), Times.Once);

            commandArgument.ProviderId.Should().Be(111);
            commandArgument.CommitmentId.Should().Be(123);

            commandArgument.Apprenticeships.Should().NotBeEmpty();
            commandArgument.Apprenticeships.ToList()[0].FirstName.Should().Be("Chris");
            commandArgument.Apprenticeships.ToList()[0].LastName.Should().Be("Froberg");
            commandArgument.Apprenticeships.ToList()[0].DateOfBirth.Should().Be(new DateTime(1998, 12, 8));
            commandArgument.Apprenticeships.ToList()[0].TrainingType.Should().Be(DAS.Commitments.Api.Types.Apprenticeship.Types.TrainingType.Standard);
            commandArgument.Apprenticeships.ToList()[0].TrainingCode.Should().Be("2");
            commandArgument.Apprenticeships.ToList()[0].StartDate.Should().Be(new DateTime(2020, 8, 1));
            commandArgument.Apprenticeships.ToList()[0].EndDate.Should().Be(new DateTime(2025, 8, 1));
            commandArgument.Apprenticeships.ToList()[0].Cost.Should().Be(1500);
            commandArgument.Apprenticeships.ToList()[0].ProviderRef.Should().Be("Provider ref");
            commandArgument.Apprenticeships.ToList()[0].ULN.Should().Be("1113335559");
            commandArgument.UserEmailAddress.Should().Be(signinUser.Email);
            commandArgument.UserDisplayName.Should().Be(signinUser.DisplayName);
            commandArgument.UserId.Should().Be("user123");
        }

        [Test]
        public async Task ThenIfAnyRecordsOverlapWithActiveApprenticeshipsThenReturnError()
        {
            const string dataLine = "\n\r ABBA123,Chris,Froberg,1998-12-08,2,2020-08,2025-08,1500,,Employer ref,Provider ref,1113335559, apprentice1@test.com,XYZUR";
            const string fileContents = HeaderLine + dataLine;
            var textStream = new MemoryStream(UTF8.GetBytes(fileContents));
            _file.Setup(m => m.InputStream).Returns(textStream);

            BulkUploadApprenticeshipsCommand commandArgument = null;
            _mockMediator.Setup(x => x.Send(It.IsAny<BulkUploadApprenticeshipsCommand>(), new CancellationToken()))
                .ReturnsAsync(new Unit())
                .Callback((IRequest<Unit> x, CancellationToken c) => commandArgument = x.As<BulkUploadApprenticeshipsCommand>());

            _mockMediator.Setup(m => m.Send(It.IsAny<GetCommitmentQueryRequest>(), new CancellationToken()))
                .Returns(Task.FromResult(new GetCommitmentQueryResponse
                {
                    Commitment = new CommitmentView
                    {
                        AgreementStatus = AgreementStatus.NotAgreed,
                        EditStatus = EditStatus.ProviderOnly
                    }
                }));

            _mockMediator.Setup(m => m.Send(It.IsAny<GetOverlappingApprenticeshipsQueryRequest>(), It.IsAny<CancellationToken>()))
                .Returns(
                    Task.Run(() => new GetOverlappingApprenticeshipsQueryResponse
                    {
                        Overlaps = new List<ApprenticeshipOverlapValidationResult>
                        {
                            new ApprenticeshipOverlapValidationResult
                            {
                                OverlappingApprenticeships = new List<OverlappingApprenticeship>
                                {
                                    new OverlappingApprenticeship
                                    {
                                        Apprenticeship = new Apprenticeship {ULN = "1113335559"},
                                        ValidationFailReason = ValidationFailReason.DateEmbrace
                                    }
                                }
                            }
                        }
                    }));

            var model = new UploadApprenticeshipsViewModel { Attachment = _file.Object, HashedCommitmentId = "ABBA123", ProviderId = 111, AccountLegalEntityPublicHashedId = "XYZUR" };
            var file = await _sut.UploadFile("user123", model, new SignInUserModel());

            //Assert
            Assert.IsTrue(file.HasRowLevelErrors);
        }

        [Test]
        public async Task ThenIfAnyRecordsEmailOverlapWithActiveApprenticeshipsThenReturnError()
        {
            const string dataLine = "\n\r ABBA123,Chris,Froberg,1998-12-08,2,2020-08,2025-08,1500,,Employer ref,Provider ref,1113335559,apprentice1@test.com,XYZUR";
            const string fileContents = HeaderLine + dataLine;
            var textStream = new MemoryStream(UTF8.GetBytes(fileContents));
            _file.Setup(m => m.InputStream).Returns(textStream);

            BulkUploadApprenticeshipsCommand commandArgument = null;
            _mockMediator.Setup(x => x.Send(It.IsAny<BulkUploadApprenticeshipsCommand>(), new CancellationToken()))
                .ReturnsAsync(new Unit())
                .Callback((IRequest<Unit> x, CancellationToken c) => commandArgument = x.As<BulkUploadApprenticeshipsCommand>());

            _mockMediator.Setup(m => m.Send(It.IsAny<GetCommitmentQueryRequest>(), new CancellationToken()))
                .Returns(Task.FromResult(new GetCommitmentQueryResponse
                {
                    Commitment = new CommitmentView
                    {
                        AgreementStatus = AgreementStatus.NotAgreed,
                        EditStatus = EditStatus.ProviderOnly
                    }
                }));

            _mockMediator.Setup(m => m.Send(It.IsAny<GetEmailOverlappingApprenticeshipsQueryRequest>(), It.IsAny<CancellationToken>()))
                .Returns(
                    Task.Run(() => new GetEmailOverlappingApprenticeshipsQueryResponse
                    {
                        Overlaps = new List<ApprenticeshipEmailOverlapValidationResult>
                        {
                            new ApprenticeshipEmailOverlapValidationResult
                            {
                                OverlappingApprenticeships = new List<OverlappingApprenticeship>
                                {
                                    new OverlappingApprenticeship
                                    {
                                        Apprenticeship = new Apprenticeship {Email = "apprentice1@test.com"},
                                        ValidationFailReason = ValidationFailReason.DateEmbrace
                                    }
                                }
                            }
                        }
                    }));

            var model = new UploadApprenticeshipsViewModel { Attachment = _file.Object, HashedCommitmentId = "ABBA123", ProviderId = 111, AccountLegalEntityPublicHashedId = "XYZUR" };
            
            //Act
            var file = await _sut.UploadFile("user123", model, new SignInUserModel());

            //Assert
            Assert.IsTrue(file.HasRowLevelErrors);
        }

        [Test]
        public async Task ThenIfFileIsEmptyReturnError()
        {
            var textStream = new MemoryStream(UTF8.GetBytes(""));
            _file.Setup(m => m.InputStream).Returns(textStream);

            BulkUploadApprenticeshipsCommand commandArgument = null;
            _mockMediator.Setup(x => x.Send(It.IsAny<BulkUploadApprenticeshipsCommand>(), new CancellationToken()))
                .ReturnsAsync(new Unit())
                .Callback((IRequest<Unit> x, CancellationToken c) => commandArgument = x.As<BulkUploadApprenticeshipsCommand>());
          

            _mockMediator.Setup(m => m.Send(It.IsAny<GetCommitmentQueryRequest>(), new CancellationToken()))
                .Returns(Task.FromResult(new GetCommitmentQueryResponse
                {
                    Commitment = new CommitmentView
                    {
                        AgreementStatus = AgreementStatus.NotAgreed,
                        EditStatus = EditStatus.ProviderOnly
                    }
                }));

            _mockMediator.Setup(m => m.Send(It.IsAny<GetOverlappingApprenticeshipsQueryRequest>(), new CancellationToken()))
                .Returns(
                    Task.Run(() => new GetOverlappingApprenticeshipsQueryResponse
                    {
                        Overlaps = new List<ApprenticeshipOverlapValidationResult>
                        {
                            new ApprenticeshipOverlapValidationResult
                            {
                                OverlappingApprenticeships = new List<OverlappingApprenticeship>
                                {
                                    new OverlappingApprenticeship
                                    {
                                        Apprenticeship = new Apprenticeship {ULN = "1113335559"},
                                        ValidationFailReason = ValidationFailReason.DateEmbrace
                                    }
                                }
                            }
                        }
                    }));

            var model = new UploadApprenticeshipsViewModel { Attachment = _file.Object, HashedCommitmentId = "ABBA123", ProviderId = 111, AccountLegalEntityPublicHashedId = "XYZUR" };
            var file = await _sut.UploadFile("user123", model, new SignInUserModel());

            //Assert
            Assert.IsTrue(file.HasFileLevelErrors);

            _logger.Verify(x => x.Info(It.IsAny<string>(), It.IsAny<long?>(), It.IsAny<long?>(), It.IsAny<long?>()), Times.Once);
            _logger.Verify(x => x.Error(It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<long?>(), It.IsAny<long?>(), It.IsAny<long?>()), Times.Never);

        }

        [Test]
        [TestCaseSource(nameof(GetInvalidColumnHeaders))]
        public async Task ThenIfFileHasMissingFieldsReturnError(string header)
        {
            var inputData = $"{header}" +
                            @"
                            Abba123,1113335559,Froberg,Chris,1998-12-08,SE123321C,25,,,2,2120-08,2125-08,1500,,Employer ref,Provider ref,apprentice1@test.com,XYZUR";

            var textStream = new MemoryStream(UTF8.GetBytes(inputData));
            _file.Setup(m => m.InputStream).Returns(textStream);

            BulkUploadApprenticeshipsCommand commandArgument = null;
            _mockMediator.Setup(x => x.Send(It.IsAny<BulkUploadApprenticeshipsCommand>(), new CancellationToken()))
                .ReturnsAsync(new Unit())
                .Callback((IRequest<Unit> x, CancellationToken c) => commandArgument = x.As<BulkUploadApprenticeshipsCommand>());

            _mockMediator.Setup(m => m.Send(It.IsAny<GetCommitmentQueryRequest>(), new CancellationToken()))
                .Returns(Task.FromResult(new GetCommitmentQueryResponse
                {
                    Commitment = new CommitmentView
                    {
                        AgreementStatus = AgreementStatus.NotAgreed,
                        EditStatus = EditStatus.ProviderOnly
                    }
                }));

            _mockMediator.Setup(m => m.Send(It.IsAny<GetOverlappingApprenticeshipsQueryRequest>(), It.IsAny<CancellationToken>()))
                .Returns(
                    Task.Run(() => new GetOverlappingApprenticeshipsQueryResponse
                    {
                        Overlaps = new List<ApprenticeshipOverlapValidationResult>
                        {
                            new ApprenticeshipOverlapValidationResult
                            {
                                OverlappingApprenticeships = new List<OverlappingApprenticeship>
                                {
                                    new OverlappingApprenticeship
                                    {
                                        Apprenticeship = new Apprenticeship {ULN = "1113335559"},
                                        ValidationFailReason = ValidationFailReason.DateEmbrace
                                    }
                                }
                            }
                        }
                    }));

            var model = new UploadApprenticeshipsViewModel { Attachment = _file.Object, HashedCommitmentId = "ABBA123", ProviderId = 111, AccountLegalEntityPublicHashedId = "XYZUR" };
            var file = await _sut.UploadFile("user123", model, new SignInUserModel());

            //Assert
            Assert.IsTrue(file.HasFileLevelErrors);

            _logger.Verify(x => x.Info(It.IsAny<string>(), It.IsAny<long?>(), It.IsAny<long?>(), It.IsAny<long?>()), Times.Once);
            _logger.Verify(x => x.Error(It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<long?>(), It.IsAny<long?>(), It.IsAny<long?>()), Times.Never);
        }

    }
}
