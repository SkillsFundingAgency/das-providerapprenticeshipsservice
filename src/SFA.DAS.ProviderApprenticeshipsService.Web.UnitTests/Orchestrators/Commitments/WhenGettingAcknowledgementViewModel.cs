using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetCommitment;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Types;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Orchestrators.Commitments
{
    [TestFixture]
    public class WhenGettingAcknowledgementViewModel: ApprenticeshipValidationTestBase
    {
        private CommitmentView _commitment;

        public override void SetUp()
        {
            _commitment = new CommitmentView
            {
                Messages = new List<MessageView>()
            };

            _mockMediator.Setup(x => x.SendAsync(It.IsAny<GetCommitmentQueryRequest>()))
                .ReturnsAsync(() => new GetCommitmentQueryResponse
                {
                    Commitment = _commitment
                });

            base.SetUp();
        }

        [Test]
        public async Task ThenARequestForTheCommitmentDataIsMade()
        {
            await _orchestrator.GetAcknowledgementViewModel(1, "Hashed-Id", SaveStatus.ApproveAndSend);

            _mockMediator.Verify(x => x.SendAsync(It.IsAny<GetCommitmentQueryRequest>()), Times.Once);
        }

        public enum ExpectedWhatHappensNextType
        {
            TransferFirstApproval,
            EmployerWillReview,
            UpdatedCohort
        }

        //todo: check when set to approve. handle same as approveandsend??
        [TestCase(true, AgreementStatus.ProviderAgreed, SaveStatus.ApproveAndSend, ExpectedWhatHappensNextType.TransferFirstApproval)]
        [TestCase(true, AgreementStatus.BothAgreed, SaveStatus.ApproveAndSend, ExpectedWhatHappensNextType.EmployerWillReview)]
        [TestCase(true, AgreementStatus.EmployerAgreed, SaveStatus.ApproveAndSend, ExpectedWhatHappensNextType.EmployerWillReview)]
        [TestCase(true, AgreementStatus.NotAgreed, SaveStatus.ApproveAndSend, ExpectedWhatHappensNextType.EmployerWillReview)]
        [TestCase(true, AgreementStatus.ProviderAgreed, SaveStatus.AmendAndSend, ExpectedWhatHappensNextType.UpdatedCohort)]
        [TestCase(true, AgreementStatus.BothAgreed, SaveStatus.AmendAndSend, ExpectedWhatHappensNextType.UpdatedCohort)]
        [TestCase(true, AgreementStatus.EmployerAgreed, SaveStatus.AmendAndSend, ExpectedWhatHappensNextType.UpdatedCohort)]
        [TestCase(true, AgreementStatus.NotAgreed, SaveStatus.AmendAndSend, ExpectedWhatHappensNextType.UpdatedCohort)]
        [TestCase(true, AgreementStatus.ProviderAgreed, SaveStatus.Approve, ExpectedWhatHappensNextType.UpdatedCohort)]
        [TestCase(true, AgreementStatus.BothAgreed, SaveStatus.Approve, ExpectedWhatHappensNextType.UpdatedCohort)]
        [TestCase(true, AgreementStatus.EmployerAgreed, SaveStatus.Approve, ExpectedWhatHappensNextType.UpdatedCohort)]
        [TestCase(true, AgreementStatus.NotAgreed, SaveStatus.Approve, ExpectedWhatHappensNextType.UpdatedCohort)]
        [TestCase(true, AgreementStatus.ProviderAgreed, SaveStatus.Save, ExpectedWhatHappensNextType.UpdatedCohort)]
        [TestCase(true, AgreementStatus.BothAgreed, SaveStatus.Save, ExpectedWhatHappensNextType.UpdatedCohort)]
        [TestCase(true, AgreementStatus.EmployerAgreed, SaveStatus.Save, ExpectedWhatHappensNextType.UpdatedCohort)]
        [TestCase(true, AgreementStatus.NotAgreed, SaveStatus.Save, ExpectedWhatHappensNextType.UpdatedCohort)]
        [TestCase(false, AgreementStatus.ProviderAgreed, SaveStatus.ApproveAndSend, ExpectedWhatHappensNextType.EmployerWillReview)]
        [TestCase(false, AgreementStatus.BothAgreed, SaveStatus.ApproveAndSend, ExpectedWhatHappensNextType.EmployerWillReview)]
        [TestCase(false, AgreementStatus.EmployerAgreed, SaveStatus.ApproveAndSend, ExpectedWhatHappensNextType.EmployerWillReview)]
        [TestCase(false, AgreementStatus.NotAgreed, SaveStatus.ApproveAndSend, ExpectedWhatHappensNextType.EmployerWillReview)]
        [TestCase(false, AgreementStatus.ProviderAgreed, SaveStatus.AmendAndSend, ExpectedWhatHappensNextType.UpdatedCohort)]
        [TestCase(false, AgreementStatus.BothAgreed, SaveStatus.AmendAndSend, ExpectedWhatHappensNextType.UpdatedCohort)]
        [TestCase(false, AgreementStatus.EmployerAgreed, SaveStatus.AmendAndSend, ExpectedWhatHappensNextType.UpdatedCohort)]
        [TestCase(false, AgreementStatus.NotAgreed, SaveStatus.AmendAndSend, ExpectedWhatHappensNextType.UpdatedCohort)]
        [TestCase(false, AgreementStatus.ProviderAgreed, SaveStatus.Approve, ExpectedWhatHappensNextType.UpdatedCohort)]
        [TestCase(false, AgreementStatus.BothAgreed, SaveStatus.Approve, ExpectedWhatHappensNextType.UpdatedCohort)]
        [TestCase(false, AgreementStatus.EmployerAgreed, SaveStatus.Approve, ExpectedWhatHappensNextType.UpdatedCohort)]
        [TestCase(false, AgreementStatus.NotAgreed, SaveStatus.Approve, ExpectedWhatHappensNextType.UpdatedCohort)]
        [TestCase(false, AgreementStatus.ProviderAgreed, SaveStatus.Save, ExpectedWhatHappensNextType.UpdatedCohort)]
        [TestCase(false, AgreementStatus.BothAgreed, SaveStatus.Save, ExpectedWhatHappensNextType.UpdatedCohort)]
        [TestCase(false, AgreementStatus.EmployerAgreed, SaveStatus.Save, ExpectedWhatHappensNextType.UpdatedCohort)]
        [TestCase(false, AgreementStatus.NotAgreed, SaveStatus.Save, ExpectedWhatHappensNextType.UpdatedCohort)]
        public async Task ThenWhatHappensNextIsPopulatedCorrectly(bool isTransfer, AgreementStatus agreementStatus, SaveStatus saveStatus, ExpectedWhatHappensNextType expectedWhatHappensNextType)
        {
            if (isTransfer)
                _commitment.TransferSender = new TransferSender();

            _commitment.AgreementStatus = agreementStatus;

            var viewModel = await _orchestrator.GetAcknowledgementViewModel(1, "Hashed-Id", saveStatus);

            string[] expectedWhatHappensNext;
            switch (expectedWhatHappensNextType)
            {
                case ExpectedWhatHappensNextType.TransferFirstApproval:
                    expectedWhatHappensNext = new[]
                    {
                        "The employer will receive your cohort and will either confirm the information is correct or contact you to suggest changes.",
                        "Once the employer approves the cohort, a transfer request will be sent to the funding employer to review.",
                        "You will receive a notification once the funding employer approves or rejects the transfer request. You can view the progress of a request from the 'With transfer sending employers' status screen."
                    };
                    break;
                case ExpectedWhatHappensNextType.EmployerWillReview:
                    expectedWhatHappensNext = new[] { "The employer will review the cohort and either approve it or contact you with an update." };
                    break;
                case ExpectedWhatHappensNextType.UpdatedCohort:
                    expectedWhatHappensNext = new[] { "The updated cohort will appear in the employer’s account for them to review." };
                    break;
                default:
                    throw new NotImplementedException();
            }

            CollectionAssert.AreEquivalent(expectedWhatHappensNext, viewModel.WhatHappensNext);
        }
    }
}
