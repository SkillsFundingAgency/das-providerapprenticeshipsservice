using FluentValidation;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Commands.BulkUploadApprenticeships
{
    public sealed class BulkUploadApprenticeshipsCommandValidator : AbstractValidator<BulkUploadApprenticeshipsCommand>
    {
        public BulkUploadApprenticeshipsCommandValidator()
        {
            RuleFor(x => x.ProviderId).GreaterThan(0);
            RuleFor(x => x.CommitmentId).GreaterThan(0);
            RuleFor(x => x.Apprenticeships).NotEmpty();
        }
    }
}
