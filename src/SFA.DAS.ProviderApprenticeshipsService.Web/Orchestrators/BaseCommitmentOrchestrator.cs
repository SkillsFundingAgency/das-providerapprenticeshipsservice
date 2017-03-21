using System;
using System.Linq;
using System.Threading.Tasks;

using MediatR;

using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.Commitments.Api.Types.Commitment.Types;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetCommitment;
using SFA.DAS.ProviderApprenticeshipsService.Web.Exceptions;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators
{
    public class BaseCommitmentOrchestrator
    {
        private readonly IMediator _mediator;

        public BaseCommitmentOrchestrator(IMediator mediator)
        {
            if (mediator == null)
                throw new ArgumentNullException(nameof(mediator));

            _mediator = mediator;
        }
        protected async Task AssertCommitmentStatus(long commitmentId, long providerId)
        {
            var commitmentData = await _mediator.SendAsync(new GetCommitmentQueryRequest
                                                               {
                                                                   ProviderId = providerId,
                                                                   CommitmentId = commitmentId
                                                               });
            AssertCommitmentStatus(commitmentData.Commitment, EditStatus.ProviderOnly);
            AssertCommitmentStatus(commitmentData.Commitment, AgreementStatus.EmployerAgreed, AgreementStatus.ProviderAgreed, AgreementStatus.NotAgreed);
        }

        protected static void AssertCommitmentStatus(Commitment commitment, params AgreementStatus[] allowedAgreementStatuses)
        {
            if (commitment == null)
                throw new InvalidStateException("Null commitment");

            if (!allowedAgreementStatuses.Contains(commitment.AgreementStatus))
                throw new InvalidStateException($"Invalid commitment state (agreement status is {commitment.AgreementStatus}, expected {string.Join(",", allowedAgreementStatuses)})");
        }

        protected static void AssertCommitmentStatus(Commitment commitment, params EditStatus[] allowedEditStatuses)
        {
            if (commitment == null)
                throw new InvalidStateException("Null commitment");

            if (!allowedEditStatuses.Contains(commitment.EditStatus))
                throw new InvalidStateException($"Invalid commitment state (edit status is {commitment.EditStatus}, expected {string.Join(",", allowedEditStatuses)})");
        }
    }
}