using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetCommitment;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Orchestrators
{
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
            var _sut = new CommitmentOrchestrator(mockMediator.Object, Mock.Of<ICommitmentStatusCalculator>(), Mock.Of<IHashingService>());

            var result = _sut.GetFinishEditing(1L, "ABBA123").Result;

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
            var _sut = new CommitmentOrchestrator(mockMediator.Object, Mock.Of<ICommitmentStatusCalculator>(), Mock.Of<IHashingService>());

            var result = _sut.GetFinishEditing(1L, "ABBA123").Result;

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
            var _sut = new CommitmentOrchestrator(mockMediator.Object, Mock.Of<ICommitmentStatusCalculator>(), Mock.Of<IHashingService>());

            var result = _sut.GetFinishEditing(1L, "ABBA123").Result;

            result.ApproveAndSend.ShouldBeEquivalentTo(true);
        }

        // --- Helpers ---

        private static Mock<IMediator> GetMediator(List<Apprenticeship> apprenticeships)
        {
            var respons = new GetCommitmentQueryResponse
            {
                Commitment = new Commitment { Apprenticeships = apprenticeships, EditStatus = EditStatus.ProviderOnly }
            };

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(m => m.SendAsync(It.IsAny<GetCommitmentQueryRequest>()))
                .Returns(Task.Factory.StartNew(() => respons));

            return mockMediator;
        }
    }
}
