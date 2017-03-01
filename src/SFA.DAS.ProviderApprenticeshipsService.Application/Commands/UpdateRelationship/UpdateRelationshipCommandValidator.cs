using FluentValidation;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Commands.UpdateRelationship
{
    public class UpdateRelationshipCommandValidator : AbstractValidator<UpdateRelationshipCommand>
    {
        public UpdateRelationshipCommandValidator()
        {
            RuleFor(x => x.ProviderId).NotEmpty();
            RuleFor(x => x.Relationship).NotEmpty();
            RuleFor(x => x.UserId).NotEmpty();
        }
    }
}
