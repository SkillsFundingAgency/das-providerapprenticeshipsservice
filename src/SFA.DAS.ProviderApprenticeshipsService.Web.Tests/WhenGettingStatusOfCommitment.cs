﻿using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Tests
{
    [TestFixture]
    public sealed class WhenGettingStatusOfCommitment
    {
        private static readonly ICommitmentStatusCalculator _calculator = new CommitmentStatusCalculator();

        // [TestCase(RequestStatus.None, EditStatus.EmployerOnly, 0, LastAction.None, TestName = "Employer creates empty cohort and saves")]
        [TestCase(RequestStatus.NewRequest, EditStatus.ProviderOnly, 0, LastAction.None, TestName = "Employer sends to provider for amendment")]
        [TestCase(RequestStatus.NewRequest, EditStatus.ProviderOnly, 2, LastAction.None, TestName = "Provider adds apprenticeship")]
        // TODO: this needs to differentiate when sent from employer for ammendment or provider adds to empty commitment
        [TestCase(RequestStatus.SentForReview, EditStatus.EmployerOnly, 2, LastAction.Amend, TestName = "Provider sends to employer for amendment")]
        [TestCase(RequestStatus.SentForReview, EditStatus.EmployerOnly, 2, LastAction.Amend, TestName = "Employer edits apprenticeship")]
        [TestCase(RequestStatus.ReadyForReview, EditStatus.ProviderOnly, 2, LastAction.Amend, TestName = "Employer sends to providder for amendment")]

        [TestCase(RequestStatus.WithEmployerForApproval, EditStatus.EmployerOnly, 2, LastAction.Approve, TestName = "Provider approves")]
        [TestCase(RequestStatus.WithEmployerForApproval, EditStatus.EmployerOnly, 2, LastAction.Approve, TestName = "Employer edits apprenticeship & changes employer reference")]
        [TestCase(RequestStatus.WithEmployerForApproval, EditStatus.EmployerOnly, 2, LastAction.Approve, TestName = "Employer change apprenticeship & changes cost")] // ToDo Confirm status text

        [TestCase(RequestStatus.ReadyForReview, EditStatus.ProviderOnly, 2, LastAction.Amend, TestName = "Employer sends to provider for amendment")]
        [TestCase(RequestStatus.ReadyForReview, EditStatus.ProviderOnly, 2, LastAction.Amend, TestName = "Provider amends")]
        [TestCase(RequestStatus.WithEmployerForApproval, EditStatus.EmployerOnly, 2, LastAction.Approve, TestName = "Provider approves")]
        [TestCase(RequestStatus.Approved, EditStatus.Both, 2, LastAction.Approve, TestName = "Employer approves")]

        public void EmployerSendsToProviderToAddApprentices(RequestStatus expectedResult, EditStatus editStatus, int numberOfApprenticeships, LastAction lastAction)
        {
            var status = _calculator.GetStatus(editStatus, numberOfApprenticeships, lastAction);

            status.Should().Be(expectedResult);
        }

        [TestCase(RequestStatus.None, 0, LastAction.Amend, TestName = "With no apprenticeships")]
        [TestCase(RequestStatus.SentForReview, 2, LastAction.Amend, TestName = "With last action is Amend")]
        [TestCase(RequestStatus.WithEmployerForApproval, 2, LastAction.Approve, TestName = "With last action is Approve")]
        public void EmployerOnly(RequestStatus expectedResult, int numberOfApprenticeships, LastAction lastAction)
        {
            var editStatus = EditStatus.EmployerOnly;

            var status = _calculator.GetStatus(editStatus, numberOfApprenticeships, lastAction);

            status.Should().Be(expectedResult);
        }

        [TestCase(RequestStatus.NewRequest, 0, LastAction.Amend, TestName = "With no apprenticeships")]
        [TestCase(RequestStatus.ReadyForReview, 2, LastAction.Amend, TestName = "With last action is Amend")]
        [TestCase(RequestStatus.None, 2, LastAction.Approve, TestName = "With last action is Approve")]
        public void ProviderOnly(RequestStatus expectedResult, int numberOfApprenticeships, LastAction lastAction)
        {
            var editStatus = EditStatus.ProviderOnly;

            var status = _calculator.GetStatus(editStatus, numberOfApprenticeships, lastAction);

            status.Should().Be(expectedResult);
        }

        [TestCase(RequestStatus.Approved, 0, LastAction.Amend, TestName = "With no apprenticeships, Amend")]
        [TestCase(RequestStatus.Approved, 0, LastAction.Approve, TestName = "With no apprenticeships, Approve")]
        [TestCase(RequestStatus.Approved, 0, LastAction.None, TestName = "With no apprenticeships, None")]
        [TestCase(RequestStatus.Approved, 2, LastAction.Amend, TestName = "With last action is Amend")]
        [TestCase(RequestStatus.Approved, 2, LastAction.Approve, TestName = "With last action is Approve")]
        [TestCase(RequestStatus.Approved, 2, LastAction.None, TestName = "With last action is None")]
        public void EditStatusBoth(RequestStatus expectedResult, int numberOfApprenticeships, LastAction lastAction)
        {
            var status = _calculator.GetStatus(EditStatus.Both, numberOfApprenticeships, lastAction);

            status.Should().Be(expectedResult);
        }

        [TestCase(RequestStatus.None, 0, LastAction.Amend, TestName = "With no apprenticeships, Amend")]
        [TestCase(RequestStatus.None, 0, LastAction.Approve, TestName = "With no apprenticeships, Approve")]
        [TestCase(RequestStatus.None, 0, LastAction.None, TestName = "With no apprenticeships, None")]
        [TestCase(RequestStatus.None, 2, LastAction.Amend, TestName = "With last action is Amend")]
        [TestCase(RequestStatus.None, 2, LastAction.Approve, TestName = "With last action is Approve")]
        [TestCase(RequestStatus.None, 2, LastAction.None, TestName = "With last action is None")]
        public void EditStatusNeither(RequestStatus expectedResult, int numberOfApprenticeships, LastAction lastAction)
        {
            var status = _calculator.GetStatus(EditStatus.Neither, numberOfApprenticeships, lastAction);

            status.Should().Be(expectedResult);
        }
    }
}
