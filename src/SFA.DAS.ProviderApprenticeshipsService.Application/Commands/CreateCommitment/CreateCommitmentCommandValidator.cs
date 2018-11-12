using System;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Commands.CreateCommitment
{
    public class CreateCommitmentCommandValidator : AbstractValidator<CreateCommitmentCommand>
    {
        public CreateCommitmentCommandValidator()
        {
            RuleFor(x => x.Commitment).NotNull();

            When(x => x.Commitment != null, () =>
            {
                RuleFor(c => c.Commitment.EmployerAccountId).NotEmpty();
                RuleFor(c => c.Commitment.LegalEntityId).NotEmpty();
                RuleFor(c => c.Commitment.LegalEntityName).NotEmpty();
                RuleFor(c => c.Commitment.ProviderId).NotEmpty();
                RuleFor(c => c.Commitment.ProviderId).GreaterThan(0);
                RuleFor(c => c.Commitment.ProviderName).NotEmpty();
            });
        }
    }
}
