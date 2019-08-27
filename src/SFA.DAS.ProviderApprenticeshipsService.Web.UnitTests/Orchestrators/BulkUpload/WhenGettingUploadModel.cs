using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.Commitments.Api.Types.Commitment.Types;
using SFA.DAS.HashingService;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetCommitment;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.BulkUpload;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.Mappers;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Orchestrators.BulkUpload
{
    [TestFixture]
    public class WhenGettingUploadModel
    {
        private BulkUploadOrchestrator _bulkUploadOrchestrator;

        private Mock<IMediator> _mediator;
        private CommitmentView _commitmentView;
        private Mock<IReservationsService> _reservationsService;

        [SetUp]
        public void Arrange()
        {
            _commitmentView = new CommitmentView
            {
                EditStatus = EditStatus.ProviderOnly,
                AgreementStatus = AgreementStatus.NotAgreed
            };

            _mediator = new Mock<IMediator>();
            _mediator.Setup(x => x.Send(It.IsAny<GetCommitmentQueryRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => new GetCommitmentQueryResponse
                {
                    Commitment = _commitmentView
                });

            _reservationsService = new Mock<IReservationsService>();
            _reservationsService.Setup(rs => rs.IsAutoReservationEnabled(It.IsAny<long>())).ReturnsAsync(true);

            var bulkUploader = new BulkUploader(_mediator.Object, Mock.Of<IBulkUploadValidator>(),
                Mock.Of<IBulkUploadFileParser>(), Mock.Of<IProviderCommitmentsLogger>());

            _bulkUploadOrchestrator = new BulkUploadOrchestrator(
                _mediator.Object,
                bulkUploader,
                Mock.Of<IHashingService>(),
                new BulkUploadMapper(_mediator.Object),
                Mock.Of<IProviderCommitmentsLogger>(),
                Mock.Of<IBulkUploadFileParser>(),
                _reservationsService.Object
                );
        }

        [Test]
        public void ThenIfCohortIsNotPaidForByATransferThenAnExceptionIsNotThrown()
        {
            _commitmentView.TransferSender = null;
            Assert.DoesNotThrowAsync(() => _bulkUploadOrchestrator.GetUploadModel(123L, "HashedCmtId"));
        }

        [Test, AutoData]
        public async Task AndHasTransferSender_ThenIsPaidByTransferIsTrue(TransferSender transferSender)
        {
            _commitmentView.TransferSender = transferSender;
            var result = await _bulkUploadOrchestrator.GetUploadModel(123L, "HashedCmtId");
            result.IsPaidByTransfer.Should().BeTrue();
        }

        [Test]
        public async Task AndNullTransferSender_ThenIsPaidByTransferIsFalse()
        {
            _commitmentView.TransferSender = null;
            var result = await _bulkUploadOrchestrator.GetUploadModel(123L, "HashedCmtId");
            result.IsPaidByTransfer.Should().BeFalse();
        }
    }
}
