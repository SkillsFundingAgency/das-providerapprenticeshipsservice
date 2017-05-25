using MediatR;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Commands.SaveBulkUploadFile
{
    public class SaveBulkUploadFileCommand : IAsyncRequest<long>
    {
        public long ProviderId { get; set; }

        public long CommitmentId { get; set; }

        public string FileContent { get; set; }

        public string FileName { get; set; }
    }
}
