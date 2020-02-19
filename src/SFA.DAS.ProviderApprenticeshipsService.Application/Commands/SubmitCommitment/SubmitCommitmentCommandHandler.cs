using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.Commitments.Api.Types.Commitment.Types;
using SFA.DAS.HashingService;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Commands.SubmitCommitment
{
    public class SubmitCommitmentCommandHandler : AsyncRequestHandler<SubmitCommitmentCommand>
    {
        private readonly IProviderCommitmentsApi _commitmentsApi;
        private readonly IValidator<SubmitCommitmentCommand> _validator;
        private readonly IMediator _mediator;
        private readonly ProviderApprenticeshipsServiceConfiguration _configuration;
        private readonly IHashingService _hashingService;

        public SubmitCommitmentCommandHandler(IProviderCommitmentsApi commitmentsApi,
            IValidator<SubmitCommitmentCommand> validator,
            IMediator mediator,
            ProviderApprenticeshipsServiceConfiguration configuration,
            IHashingService hashingService)
        {
            _commitmentsApi = commitmentsApi;
            _validator = validator;
            _mediator = mediator;
            _configuration = configuration;
            _hashingService = hashingService;
        }

        protected override async Task Handle(SubmitCommitmentCommand message, CancellationToken cancellationToken)
        {
            var validationResult = _validator.Validate(message);
            if (!validationResult.IsValid)
                throw new InvalidRequestException(validationResult.Errors);

            var commitment = await _commitmentsApi.GetProviderCommitment(message.ProviderId, message.CommitmentId);

            if (commitment.ProviderId != message.ProviderId)
                throw new InvalidRequestException(new Dictionary<string, string> { { "Commitment", "This commitment does not belong to this Provider" } });

            var submission = new CommitmentSubmission { Action = message.LastAction, LastUpdatedByInfo = 
                new LastUpdateInfo
                    {
                        Name = message?.UserDisplayName ?? "",
                        EmailAddress = message?.UserEmailAddress ?? ""
                    },
                    Message = message.Message,
                    UserId = message.UserId
                };

            if (message.LastAction != LastAction.Approve)
            {
                await _commitmentsApi.PatchProviderCommitment(message.ProviderId, message.CommitmentId, submission);
            }
            else
            {
                await _commitmentsApi.ApproveCohort(message.ProviderId, message.CommitmentId, submission);
            }
        }
    }
}