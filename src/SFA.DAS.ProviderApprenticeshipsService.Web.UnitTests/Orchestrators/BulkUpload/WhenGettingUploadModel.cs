using System;
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

        [SetUp]
        public void Arrange()
        {
            _commitmentView = new CommitmentView
            {
                EditStatus = EditStatus.ProviderOnly,
                AgreementStatus = AgreementStatus.NotAgreed
            };

            _mediator = new Mock<IMediator>();
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetCommitmentQueryRequest>()))
                .ReturnsAsync(() => new GetCommitmentQueryResponse
                {
                    Commitment = _commitmentView
                });

            var bulkUploader = new BulkUploader(_mediator.Object, Mock.Of<IBulkUploadValidator>(),
                Mock.Of<IBulkUploadFileParser>(), Mock.Of<IProviderCommitmentsLogger>());

            _bulkUploadOrchestrator = new BulkUploadOrchestrator(
                _mediator.Object,
                bulkUploader,
                Mock.Of<IHashingService>(),
                new BulkUploadMapper(_mediator.Object),
                Mock.Of<IProviderCommitmentsLogger>(),
                Mock.Of<IBulkUploadFileParser>());
        }


        [Test]
        public void ThenIfCohortIsPaidForByATransferThenAnExceptionIsThrown()
        {
            _commitmentView.TransferSender = new TransferSender {Id = 123L};
            Assert.ThrowsAsync<InvalidOperationException>(() => _bulkUploadOrchestrator.GetUploadModel(123L, "HashedCmtId"));
        }

        [Test]
        public void ThenIfCohortIsNotPaidForByATransferThenAnExceptionIsNotThrown()
        {
            _commitmentView.TransferSender = null;
            Assert.DoesNotThrowAsync(() => _bulkUploadOrchestrator.GetUploadModel(123L, "HashedCmtId"));
        }


    }
}
