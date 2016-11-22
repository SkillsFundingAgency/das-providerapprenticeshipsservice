namespace SFA.DAS.ProviderApprenticeshipsService.Web.Tests.Orchestrators
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using FluentAssertions;
    using MediatR;
    using Moq;
    using NUnit.Framework;

    using Commitments.Api.Types;
    using Application.Queries.GetCommitment;
    using Web.Orchestrators;

    [TestFixture]
    public class WhenFinishEditing
    {
        [Test(Description = "Should return false on ApproveAndSend if no ProviderAgreed or no NotAgreed in at least one apprenticeship ")]
        public void ShouldCommitmentWithEmployerAndBothAgreed()
        {
            var apprenticeships = new List<Apprenticeship>
            {
                new Apprenticeship { AgreementStatus = AgreementStatus.EmployerAgreed },
                new Apprenticeship { AgreementStatus = AgreementStatus.BothAgreed }
            };

            var mockMediator = GetMediator(apprenticeships);
            var _sut = new CommitmentOrchestrator(mockMediator.Object);

            var result = _sut.GetFinishEditing(1L, 2L).Result;

            result.ApproveAndSend.ShouldBeEquivalentTo(false);
        }

        [Test(Description = "Should return true on ApproveAndSend if at least one apprenticeship is ProviderAgreed ")]
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

            var result = _sut.GetFinishEditing(1L, 2L).Result;

            result.ApproveAndSend.ShouldBeEquivalentTo(true);
        }

        [Test(Description = "Should return true on ApproveAndSend if at least one apprenticeship is NotAgreed ")]
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

            var result = _sut.GetFinishEditing(1L, 2L).Result;

            result.ApproveAndSend.ShouldBeEquivalentTo(true);
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
