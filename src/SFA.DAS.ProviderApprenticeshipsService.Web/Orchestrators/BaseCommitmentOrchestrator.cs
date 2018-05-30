using System;
using System.Linq;
using System.Threading.Tasks;

using MediatR;

using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.Commitments.Api.Types.Commitment.Types;
using SFA.DAS.HashingService;
using SFA.DAS.ProviderApprenticeshipsService.Application.Exceptions;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetCommitment;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators
{
    public class BaseCommitmentOrchestrator
    {
        protected readonly IMediator _mediator;
        protected readonly IHashingService _hashingService;
        protected readonly IProviderCommitmentsLogger _logger;

        public BaseCommitmentOrchestrator(IMediator mediator,
            IHashingService hashingService, IProviderCommitmentsLogger logger)
        {
            // we keep null checks here, as this is a base class
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _hashingService = hashingService ?? throw new ArgumentNullException(nameof(hashingService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<CommitmentView> GetCommitment(long providerId, string hashedCommitmentId)
        {
            return await GetCommitment(providerId, _hashingService.DecodeValue(hashedCommitmentId));
        }

        public async Task<CommitmentView> GetCommitment(long providerId, long commitmentId)
        {
            _logger.Info($"Getting commitment:{commitmentId} for provider:{providerId}", providerId, commitmentId);

            var data = await _mediator.SendAsync(new GetCommitmentQueryRequest
            {
                ProviderId = providerId,
                CommitmentId = commitmentId
            });

            return data.Commitment;
        }

        //todo: the consumers of this get the commitment themeselves,
        // so it makes more sense for the consumers to pass it in
        // and not fetch it twice from the database
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

        protected static void AssertCommitmentStatus(CommitmentView commitment, params AgreementStatus[] allowedAgreementStatuses)
        {
            if (commitment == null)
                throw new InvalidStateException("Null commitment");

            if (!allowedAgreementStatuses.Contains(commitment.AgreementStatus))
                throw new InvalidStateException($"Invalid commitment state (agreement status is {commitment.AgreementStatus}, expected {string.Join(",", allowedAgreementStatuses)})");
        }

        protected static void AssertCommitmentStatus(CommitmentView commitment, params EditStatus[] allowedEditStatuses)
        {
            if (commitment == null)
                throw new InvalidStateException("Null commitment");

            if (!allowedEditStatuses.Contains(commitment.EditStatus))
                throw new InvalidStateException($"Invalid commitment state (edit status is {commitment.EditStatus}, expected {string.Join(",", allowedEditStatuses)})");
        }
    }
}