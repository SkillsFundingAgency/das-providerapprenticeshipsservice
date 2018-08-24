using System.Threading.Tasks;
using FluentValidation;
using MediatR;

using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.Commitments.Api.Types.Commitment.Types;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Commands.DeleteApprenticeship
{
    public sealed class DeleteApprenticeshipCommandHandler : AsyncRequestHandler<DeleteApprenticeshipCommand>
    {
        private readonly IProviderCommitmentsApi _commitmentsApi;
        private readonly IValidator<DeleteApprenticeshipCommand> _validator;

        public DeleteApprenticeshipCommandHandler(IValidator<DeleteApprenticeshipCommand> validator, IProviderCommitmentsApi commitmentsApi)
        {
            _validator = validator;
            _commitmentsApi = commitmentsApi;
        }

        protected override Task HandleCore(DeleteApprenticeshipCommand message)
        {
            _validator.ValidateAndThrow(message);

            return _commitmentsApi.DeleteProviderApprenticeship(message.ProviderId, message.ApprenticeshipId,
                new DeleteRequest { UserId = message.UserId, LastUpdatedByInfo = new LastUpdateInfo { EmailAddress = message.UserEmailAddress, Name = message.UserDisplayName } });
        }
    }
}
