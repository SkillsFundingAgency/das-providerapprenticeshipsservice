﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.Commitments.Api.Types.Commitment.Types;
using SFA.DAS.Commitments.Api.Types.TrainingProgramme;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetApprenticeship;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetCommitment;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetTrainingProgrammes;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models.ApprenticeshipCourse;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Orchestrators.Commitments
{
    [TestFixture]
    public class WhenGettingApprenticeship : ApprenticeshipValidationTestBase
    {
        private CommitmentView _commitment;

        public override void SetUp()
        {
            base.SetUp();

            _commitment = new CommitmentView
            {
                AgreementStatus = AgreementStatus.ProviderAgreed,
                EditStatus = EditStatus.ProviderOnly,
                Apprenticeships = new List<Apprenticeship>(),
                Messages = new List<MessageView>(),
                TransferSender = new TransferSender { Id = 99, Name = "Transfer Sender Org" }
            };

            _mockMediator = new Mock<IMediator>();
            _mockMediator.Setup(x => x.Send(It.IsAny<GetApprenticeshipQueryRequest>(), new CancellationToken()))
                .ReturnsAsync(() => new GetApprenticeshipQueryResponse
                {
                    Apprenticeship = new Apprenticeship()
                });

            _mockMediator.Setup(x => x.Send(It.IsAny<GetCommitmentQueryRequest>(), new CancellationToken()))
                .ReturnsAsync(() => new GetCommitmentQueryResponse
                {
                    Commitment = _commitment
                });

            _mockMediator.Setup(m => m.Send(It.IsAny<GetTrainingProgrammesQueryRequest>(), new CancellationToken()))
                .ReturnsAsync(new GetTrainingProgrammesQueryResponse { TrainingProgrammes = new List<TrainingProgramme>() });

            _mockMapper.Setup(x => x.MapApprenticeship(It.IsAny<Apprenticeship>(), It.IsAny<CommitmentView>()))
                .Returns(() => new ApprenticeshipViewModel());

            SetUpOrchestrator();
        }


        [Test]
        public async Task ThenFrameworksAreNotRetrievedForCohortsFundedByTransfer()
        {
            _commitment.TransferSender = new TransferSender{ Id = 99, Name = "Transfer Sender Org"};

            await _orchestrator.GetApprenticeship(1L, "HashedCmtId", "HashedAppId");

            _mockMediator.Verify(x => x.Send(It.Is<GetTrainingProgrammesQueryRequest>(r => !r.IncludeFrameworks), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task ThenFrameworksAreRetrievedForCohortsNotFundedByTransfer()
        {
            _commitment.TransferSender = null;

            await _orchestrator.GetApprenticeship(1L, "HashedCmtId", "HashedAppId");

            _mockMediator.Verify(x => x.Send(It.Is<GetTrainingProgrammesQueryRequest>(r => r.IncludeFrameworks), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
