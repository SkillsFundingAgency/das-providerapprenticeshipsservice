using FluentValidation;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Commands.SaveBulkUploadFile
{
    public class SaveBulkUploadFileValidator : AbstractValidator<SaveBulkUploadFileCommand>
    {
        public SaveBulkUploadFileValidator()
        {
            RuleFor(m => m.ProviderId).NotNull().NotEmpty();
            RuleFor(m => m.CommitmentId).NotNull().NotEmpty();
            RuleFor(m => m.FileContent).NotNull();
        }
    }
}