namespace SFA.DAS.ProviderApprenticeshipsService.Web.Tests.Orchestrators
{
    using System.Collections.Generic;
    using FluentAssertions;
    using MediatR;
    using Moq;
    using NUnit.Framework;

    using Commitments.Api.Types;
    using Application.Queries.GetCommitment;

    using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;

    using Web.Orchestrators;

    using Task = System.Threading.Tasks.Task;

    public class WhenGettingCommitmentViewModel
    {
        [Test(Description = "Should return false on PendingChanges if overall agreement status is EmployerAgreed")]
        public void ShouldCommitmentWithEmployerAndBothAgreed()
        {
            var commitment = new Commitment
            {
                AgreementStatus = AgreementStatus.EmployerAgreed,
                Apprenticeships = new List<Apprenticeship>
                {
                    new Apprenticeship (),
                    new Apprenticeship ()
                }
            };

            var mockMediator = GetMediator(commitment);
            var _sut = new CommitmentOrchestrator(mockMediator.Object, Mock.Of<ICommitmentStatusCalculator>(), Mock.Of<IHashingService>());

            var result = _sut.GetCommitmentDetails(1L, "ABBA123").Result;

            result.PendingChanges.ShouldBeEquivalentTo(false);
        }

        [Test(Description = "Should return true on PendingChanges overall agreement status isn't EmployerAgreed")]
        public void CommitmentWithOneProviderAgreed()
        {
            var commitment = new Commitment
            {
                AgreementStatus = AgreementStatus.BothAgreed,
                Apprenticeships = new List<Apprenticeship>
                {
                    new Apprenticeship (),
                    new Apprenticeship ()
                }
            };

            var mockMediator = GetMediator(commitment);
            var _sut = new CommitmentOrchestrator(mockMediator.Object, Mock.Of<ICommitmentStatusCalculator>(), Mock.Of<IHashingService>());

            var result = _sut.GetCommitmentDetails(1L, "ABBA213").Result;

            result.PendingChanges.ShouldBeEquivalentTo(true);
        }

        // --- Helpers ---

        private static Mock<IMediator> GetMediator(Commitment commitment)
        {
            var respons = new GetCommitmentQueryResponse
            {
                Commitment = commitment
            };

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(m => m.SendAsync(It.IsAny<GetCommitmentQueryRequest>()))
                .Returns(Task.Factory.StartNew(() => respons));

            return mockMediator;
        }
    }
}
