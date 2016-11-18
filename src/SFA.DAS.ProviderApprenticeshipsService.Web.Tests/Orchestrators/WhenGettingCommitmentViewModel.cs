namespace SFA.DAS.ProviderApprenticeshipsService.Web.Tests.Orchestrators
{
    using System.Collections.Generic;
    using FluentAssertions;
    using MediatR;
    using Moq;
    using NUnit.Framework;

    using Commitments.Api.Types;
    using Application.Queries.GetCommitment;
    using Web.Orchestrators;

    using Task = System.Threading.Tasks.Task;

    public class WhenGettingCommitmentViewModel
    {
        [Test(Description = "Should return false on PendingChanges if no ProviderAgreed or no NotAgreed in at least one apprenticeship ")]
        public void ShouldCommitmentWithEmployerAndBothAgreed()
        {
            var apprenticeships = new List<Apprenticeship>
            {
                new Apprenticeship { AgreementStatus = AgreementStatus.EmployerAgreed },
                new Apprenticeship { AgreementStatus = AgreementStatus.BothAgreed }
            };

            var mockMediator = GetMediator(apprenticeships);
            var _sut = new CommitmentOrchestrator(mockMediator.Object);

            var result = _sut.Get(1L, 2L).Result;

            result.PendingChanges.ShouldBeEquivalentTo(false);
        }

        [Test(Description = "Should return true on PendingChanges if at least one apprenticeship is ProviderAgreed ")]
        public void CommitmentWithOneProviderAgreed()
        {
            var apprenticeships = new List<Apprenticeship>
            {
                new Apprenticeship { AgreementStatus = AgreementStatus.BothAgreed },
                new Apprenticeship { AgreementStatus = AgreementStatus.EmployerAgreed },
                new Apprenticeship { AgreementStatus = AgreementStatus.ProviderAgreed }
            };

            var mockMediator = GetMediator(apprenticeships);
            var _sut = new CommitmentOrchestrator(mockMediator.Object);

            var result = _sut.Get(1L, 2L).Result;

            result.PendingChanges.ShouldBeEquivalentTo(true);
        }

        [Test(Description = "Should return true on PendingChanges if at least one apprenticeship is NotAgreed ")]
        public void CommitmentWithOneNotAgreed()
        {
            var apprenticeships = new List<Apprenticeship>
            {
                new Apprenticeship { AgreementStatus = AgreementStatus.BothAgreed },
                new Apprenticeship { AgreementStatus = AgreementStatus.EmployerAgreed },
                new Apprenticeship { AgreementStatus = AgreementStatus.NotAgreed }
            };

            var mockMediator = GetMediator(apprenticeships);
            var _sut = new CommitmentOrchestrator(mockMediator.Object);

            var result = _sut.Get(1L, 2L).Result;

            result.PendingChanges.ShouldBeEquivalentTo(true);
        }

        // --- Helpers ---

        private static Mock<IMediator> GetMediator(List<Apprenticeship> apprenticeships)
        {
            var respons = new GetCommitmentQueryResponse
            {
                Commitment = new Commitment { Apprenticeships = apprenticeships }
            };

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(m => m.SendAsync(It.IsAny<GetCommitmentQueryRequest>()))
                .Returns(Task.Factory.StartNew(() => respons));

            return mockMediator;
        }
    }
}
