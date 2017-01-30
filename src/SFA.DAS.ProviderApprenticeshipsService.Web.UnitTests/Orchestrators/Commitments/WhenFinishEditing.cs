using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetCommitment;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Orchestrators.Commitments
{
    [TestFixture]
    public class WhenFinishEditing
    {
        private Commitment _testCommitment;

        [SetUp]
        public void Setup()
        {
            _testCommitment = new Commitment
            {
                EditStatus = EditStatus.ProviderOnly,
                ProviderId = 1L,
                Apprenticeships = new List<Apprenticeship>
                {
                }
            };
        }

        [Test(Description = "Should return NotReadyForApproval if the commitment is marked as not ready")]
        public void ShouldReturnNotReadyForApprovalWhenCommitmentNotReady()
        {
            _testCommitment.Apprenticeships = new List<Apprenticeship>
                {
                    new Apprenticeship { AgreementStatus = AgreementStatus.ProviderAgreed, CanBeApproved = false },
                    new Apprenticeship { AgreementStatus = AgreementStatus.ProviderAgreed, CanBeApproved = true }
                };

            var mockMediator = GetMediator(_testCommitment);
            var _sut = new CommitmentOrchestrator(mockMediator.Object, Mock.Of<ICommitmentStatusCalculator>(), Mock.Of<IHashingService>(), Mock.Of<IProviderCommitmentsLogger>());

            var result = _sut.GetFinishEditing(1L, "ABBA123").Result;

            result.NotReadyForApproval.Should().BeTrue();
        }

        [Test(Description = "Should return ApproveAndSend if at least one apprenticeship is ProviderAgreed")]
        public void CommitmentWithOneProviderAgreed()
        {
            _testCommitment.CanBeApproved = true;
            _testCommitment.Apprenticeships = new List<Apprenticeship>
                {
                    new Apprenticeship { AgreementStatus = AgreementStatus.BothAgreed },
                    new Apprenticeship { AgreementStatus = AgreementStatus.EmployerAgreed },
                    new Apprenticeship { AgreementStatus = AgreementStatus.ProviderAgreed }
                };

            var mockMediator = GetMediator(_testCommitment);
            var _sut = new CommitmentOrchestrator(mockMediator.Object, Mock.Of<ICommitmentStatusCalculator>(), Mock.Of<IHashingService>(), Mock.Of<IProviderCommitmentsLogger>());

            var result = _sut.GetFinishEditing(1L, "ABBA123").Result;

            result.IsApproveAndSend.Should().BeTrue();
        }

        [Test(Description = "Should return ApproveAndSend if at least one apprenticeship is NotAgreed")]
        public void CommitmentWithOneNotAgreed()
        {
            _testCommitment.CanBeApproved = true;
            _testCommitment.Apprenticeships = new List<Apprenticeship>
                {
                    new Apprenticeship { AgreementStatus = AgreementStatus.BothAgreed },
                    new Apprenticeship { AgreementStatus = AgreementStatus.EmployerAgreed },
                    new Apprenticeship { AgreementStatus = AgreementStatus.NotAgreed }
                };
           
            var mockMediator = GetMediator(_testCommitment);
            var _sut = new CommitmentOrchestrator(mockMediator.Object, Mock.Of<ICommitmentStatusCalculator>(), Mock.Of<IHashingService>(), Mock.Of<IProviderCommitmentsLogger>());

            var result = _sut.GetFinishEditing(1L, "ABBA123").Result;

            result.IsApproveAndSend.Should().BeTrue();
        }

        [Test(Description = "Should return ApproveOnly all are Employer Agreed")]
        public void CommitmentAllEmployerAgreed()
        {
            _testCommitment.CanBeApproved = true;
            _testCommitment.Apprenticeships = new List<Apprenticeship>
                {
                    new Apprenticeship { AgreementStatus = AgreementStatus.EmployerAgreed },
                    new Apprenticeship { AgreementStatus = AgreementStatus.EmployerAgreed },
                    new Apprenticeship { AgreementStatus = AgreementStatus.EmployerAgreed }
                };

            var mockMediator = GetMediator(_testCommitment);
            var _sut = new CommitmentOrchestrator(mockMediator.Object, Mock.Of<ICommitmentStatusCalculator>(), Mock.Of<IHashingService>(), Mock.Of<IProviderCommitmentsLogger>());

            var result = _sut.GetFinishEditing(1L, "ABBA123").Result;

            result.IsApproveAndSend.Should().BeFalse();
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
