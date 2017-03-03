using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using SFA.DAS.Commitments.Api.Client;
using SFA.DAS.Commitments.Api.Types;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Commands.UpdateRelationship
{
    public class UpdateRelationshipCommandHandler : AsyncRequestHandler<UpdateRelationshipCommand>
    {
        private readonly ICommitmentsApi _commitmentsApi;
        private readonly AbstractValidator<UpdateRelationshipCommand> _validator;

        public UpdateRelationshipCommandHandler(ICommitmentsApi commitmentsApi, AbstractValidator<UpdateRelationshipCommand> validator)
        {
            _commitmentsApi = commitmentsApi;
            _validator = validator;
        }

        protected override async Task HandleCore(UpdateRelationshipCommand message)
        {
            var validationResult = _validator.Validate(message);

            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var request = new RelationshipRequest()
            {
                UserId = message.UserId,
                Relationship = message.Relationship
            };

            await _commitmentsApi.PatchRelationship(message.ProviderId, message.Relationship.EmployerAccountId,
                message.Relationship.LegalEntityId, request);
        }
    }
}
