using System;
using System.Collections.Generic;

using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.Commitments.Api.Types.Commitment.Types;
using SFA.DAS.Commitments.Api.Types.Validation;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetCommitment;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetFrameworks;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetOverlappingApprenticeships;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetRelationshipByCommitment;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetStandards;
using SFA.DAS.ProviderApprenticeshipsService.Domain;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators;
using SFA.DAS.ProviderApprenticeshipsService.Web.Validation;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.Mappers;
using CommitmentView = SFA.DAS.Commitments.Api.Types.Commitment.CommitmentView;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Orchestrators.Commitments
{
    using Task = System.Threading.Tasks.Task;

    public class WhenGettingCommitmentViewModel : ApprenticeshipValidationTestBase
    {
        [Test(Description = "Should return false on PendingChanges if overall agreement status is EmployerAgreed")]
        public void ShouldCommitmentWithEmployerAndBothAgreed()
        {
            var commitment = new CommitmentView
            {
                AgreementStatus = AgreementStatus.EmployerAgreed,
                EditStatus = EditStatus.ProviderOnly,
                Apprenticeships = new List<Apprenticeship>
                {
                    new Apprenticeship (),
                    new Apprenticeship ()
                },
                Messages = new List<MessageView>()
            };

            var mockMediator = GetMediator(commitment);

            var result = _orchestrator.GetCommitmentDetails(1L, "ABBA123").Result;

            result.PendingChanges.ShouldBeEquivalentTo(false);
        }

        [Test(Description = "Should return true on PendingChanges overall agreement status isn't EmployerAgreed")]
        public void CommitmentWithOneProviderAgreed()
        {
            var commitment = new CommitmentView
            {
                AgreementStatus = AgreementStatus.ProviderAgreed,
                EditStatus = EditStatus.ProviderOnly,
                Apprenticeships = new List<Apprenticeship>
                {
                    new Apprenticeship (),
                    new Apprenticeship ()
                },
                Messages = new List<MessageView>()
            };

            _mockMediator = GetMediator(commitment);

            var result = _orchestrator.GetCommitmentDetails(1L, "ABBA213").Result;

            result.PendingChanges.ShouldBeEquivalentTo(true);
        }

        [TestCase(EditStatus.EmployerOnly, true)]
        [TestCase(EditStatus.ProviderOnly, false)]
        [TestCase(EditStatus.Neither, true)]
        public void ThenCommitmentReadOnlyFlagIsSet(EditStatus editStatus, bool expectedIsReadOnly)
        {
            var commitment = new CommitmentView
            {
                AgreementStatus = AgreementStatus.ProviderAgreed,
                EditStatus = editStatus,
                Apprenticeships = new List<Apprenticeship>(),
                Messages = new List<MessageView>()
            };

            _mockMediator = GetMediator(commitment);

            var result = _orchestrator.GetCommitmentDetails(1L, "ABBA213").Result;

            result.IsReadOnly.ShouldBeEquivalentTo(expectedIsReadOnly);
        }

        // --- Helpers ---

        private static Mock<IMediator> GetMediator(CommitmentView commitment)
        {
            var respons = new GetCommitmentQueryResponse
            {
                Commitment = commitment
            };

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(m => m.SendAsync(It.IsAny<GetCommitmentQueryRequest>()))
                .Returns(Task.Factory.StartNew(() => respons));

            mockMediator.Setup(m => m.SendAsync(It.IsAny<GetStandardsQueryRequest>()))
                .ReturnsAsync(() => new GetStandardsQueryResponse
                {
                    Standards = new List<Standard>()
                });

            mockMediator.Setup(m => m.SendAsync(It.IsAny<GetFrameworksQueryRequest>()))
                .ReturnsAsync(() => new GetFrameworksQueryResponse
                {
                    Frameworks = new List<Framework>()
                });

            mockMediator.Setup(m => m.SendAsync(It.IsAny<GetRelationshipByCommitmentQueryRequest>()))
                .ReturnsAsync(() => new GetRelationshipByCommitmentQueryResponse
                {
                    Relationship = new Relationship
                    {
                        Verified = true
                    }
                });

            mockMediator.Setup(m => m.SendAsync(It.IsAny<GetOverlappingApprenticeshipsQueryRequest>()))
                .ReturnsAsync(() => new GetOverlappingApprenticeshipsQueryResponse
                {
                    Overlaps = new List<ApprenticeshipOverlapValidationResult>()
                });

            return mockMediator;
        }
    }
}
