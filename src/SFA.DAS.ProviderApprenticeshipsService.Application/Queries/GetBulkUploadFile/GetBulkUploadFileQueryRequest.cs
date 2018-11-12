using MediatR;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetBulkUploadFile
{
    public class GetBulkUploadFileQueryRequest : IRequest<GetBulkUploadFileQueryResponse>
    {
        public long ProviderId { get; set; }
        public long BulkUploadId { get; set; }
    }
}
