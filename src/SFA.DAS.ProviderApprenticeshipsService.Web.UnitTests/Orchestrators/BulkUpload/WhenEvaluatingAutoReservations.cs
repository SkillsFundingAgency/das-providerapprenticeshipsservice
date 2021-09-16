using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.Commitments.Api.Types.Commitment.Types;
using SFA.DAS.Commitments.Api.Types.TrainingProgramme;
using SFA.DAS.Commitments.Api.Types.Validation;
using SFA.DAS.HashingService;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetBulkUploadFile;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetCommitment;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetOverlappingApprenticeships;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetTrainingProgrammes;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models.ApprenticeshipCourse;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.BulkUpload;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.BulkUpload;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.Mappers;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Orchestrators.BulkUpload
{
    [TestFixture]
    public class WhenEvaluatingAutoReservations
    {
        private BulkUploadOrchestrator _bulkUploadOrchestrator;

        private Mock<IMediator> _mediator;
        private Mock<IReservationsService> _reservationsService;
        private Mock<IHashingService> _mockHashingService;
        private Mock<IBulkUploadFileParser> _mockBulkUploadFileParser;

        private const long ProviderId = 789;
        private const long CohortId = 456;
        private const long AccountId = 123;
        private long? TransferSenderId = null;
        private const string HashedCohortId = "ABC456";

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _mediator.Setup(x => x.Send(It.Is<GetCommitmentQueryRequest>(req => req.CommitmentId == CohortId && req.ProviderId == ProviderId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => new GetCommitmentQueryResponse
                {
                    Commitment = new CommitmentView
                    {
                        EmployerAccountId = AccountId,
                        EditStatus = EditStatus.ProviderOnly,
                        AgreementStatus = AgreementStatus.NotAgreed
                    }
                });

            var bulkUploader = new BulkUploader(_mediator.Object, Mock.Of<IBulkUploadValidator>(),
                Mock.Of<IBulkUploadFileParser>(), Mock.Of<IProviderCommitmentsLogger>());

            _reservationsService = new Mock<IReservationsService>();

            _mockHashingService = new Mock<IHashingService>();
            _mockHashingService.Setup(hs => hs.DecodeValue(HashedCohortId)).Returns(CohortId);

            _mockBulkUploadFileParser = new Mock<IBulkUploadFileParser>();
            
            _bulkUploadOrchestrator = new BulkUploadOrchestrator(
                _mediator.Object,
                bulkUploader,
                _mockHashingService.Object,
                new BulkUploadMapper(_mediator.Object),
                Mock.Of<IProviderCommitmentsLogger>(),
                _mockBulkUploadFileParser.Object,
                _reservationsService.Object,
                Mock.Of<ICommitmentsV2Service>());
        }

        [Test]
        public void AndHasAutoReservationsDisabledWhenGettingUploadModel_ThenShouldGetException()
        {
            _reservationsService.Setup(rs => rs.IsAutoReservationEnabled(AccountId, TransferSenderId)).ReturnsAsync(false);

            Assert.ThrowsAsync<HttpException>(() =>_bulkUploadOrchestrator.GetUploadModel(ProviderId, HashedCohortId));
        }

        [Test]
        public void AndHasAutoReservationsEnabledWhenGettingUploadModel_ThenShouldNotGetException()
        {
            _reservationsService.Setup(rs => rs.IsAutoReservationEnabled(AccountId, TransferSenderId)).ReturnsAsync(true);

            Assert.DoesNotThrowAsync(() => _bulkUploadOrchestrator.GetUploadModel(ProviderId, HashedCohortId));
        }

        [Test]
        public void AndHasAutoReservationsDisabledWhenUploadingModel_ThenShouldGetException()
        {
            _reservationsService.Setup(rs => rs.IsAutoReservationEnabled(AccountId, TransferSenderId)).ReturnsAsync(false);

            Assert.ThrowsAsync<HttpException>(CallUploadFile);
        }

        [Test]
        public void AndHasAutoReservationsEnabledWhenUploadingModel_ThenShouldNotGetException()
        {
            _reservationsService.Setup(rs => rs.IsAutoReservationEnabled(AccountId, TransferSenderId)).ReturnsAsync(true);

            Assert.DoesNotThrowAsync(CallUploadFile);
        }


        [Test]
        public void AndHasAutoReservationsDisabledWhenGettingUnsuccessfulUpload_ThenShouldGetException()
        {
            _reservationsService.Setup(rs => rs.IsAutoReservationEnabled(AccountId, TransferSenderId)).ReturnsAsync(false);
            Assert.ThrowsAsync<HttpException>(CallGetUnsuccessfulUpload);
        }

        [Test]
        public void AndHasAutoReservationsEnabledWhenGettingUnsuccessfulUpload_ThenShouldNotGetException()
        {
            _reservationsService.Setup(rs => rs.IsAutoReservationEnabled(AccountId, TransferSenderId)).ReturnsAsync(true);
            Assert.DoesNotThrowAsync(CallGetUnsuccessfulUpload);
        }

        private Task CallUploadFile()
        {
            _mediator
                .Setup(m => m.Send(It.IsAny<GetBulkUploadFileQueryRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => new GetBulkUploadFileQueryResponse {FileContent = "FileContent"});

            return _bulkUploadOrchestrator.UploadFile("", new UploadApprenticeshipsViewModel{HashedCommitmentId = HashedCohortId, ProviderId = ProviderId}, new SignInUserModel());
        }

        private Task CallGetUnsuccessfulUpload()
        {
            _mediator
                .Setup(m => m.Send(It.IsAny<GetBulkUploadFileQueryRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => new GetBulkUploadFileQueryResponse { FileContent = "FileContent" });

            _mediator
                .Setup(m => m.Send(It.IsAny<GetTrainingProgrammesQueryRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => new GetTrainingProgrammesQueryResponse { TrainingProgrammes = new List<TrainingProgramme>() });

            _mediator
                .Setup(m => m.Send(It.IsAny<GetOverlappingApprenticeshipsQueryRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => new GetOverlappingApprenticeshipsQueryResponse { Overlaps = new ApprenticeshipOverlapValidationResult[0]});
            
            _mockBulkUploadFileParser
                .Setup(bufp => bufp.CreateViewModels(ProviderId,
                    It.Is<CommitmentView>(commitment => commitment.EmployerAccountId == AccountId), It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(new BulkUploadResult {Data = new ApprenticeshipUploadModel[0]});
                    
            return _bulkUploadOrchestrator.GetUnsuccessfulUpload(ProviderId, HashedCohortId	, "ABCDEF", false);
        }
    }
}
