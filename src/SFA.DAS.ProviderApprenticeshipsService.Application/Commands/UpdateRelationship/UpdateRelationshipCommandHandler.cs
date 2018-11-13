using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Commands.UpdateRelationship
{
    public class UpdateRelationshipCommandHandler : AsyncRequestHandler<UpdateRelationshipCommand>
    {
        private readonly IRelationshipApi _relationshipApi;
        private readonly IValidator<UpdateRelationshipCommand> _validator;

        public UpdateRelationshipCommandHandler(IRelationshipApi relationship, IValidator<UpdateRelationshipCommand> validator)
        {
            _relationshipApi = relationship;
            _validator = validator;
        }

        protected override Task Handle(UpdateRelationshipCommand message, CancellationToken cancellationToken)
        {
            var validationResult = _validator.Validate(message);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var request = new RelationshipRequest()
            {
                UserId = message.UserId,
                Relationship = message.Relationship
            };

            return _relationshipApi.PatchRelationship(message.ProviderId, message.Relationship.EmployerAccountId,
                message.Relationship.LegalEntityId, request);
        }
    }
}
