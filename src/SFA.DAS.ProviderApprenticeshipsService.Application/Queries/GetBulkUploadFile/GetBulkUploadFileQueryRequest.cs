using MediatR;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetBulkUploadFile
{
    public class GetBulkUploadFileQueryRequest : IAsyncRequest<GetBulkUploadFileQueryResponse>
    {
        public long ProviderId { get; set; }
        public long BulkUploadId { get; set; }
    }
}
