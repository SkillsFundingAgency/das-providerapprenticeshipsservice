using MediatR;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetTask
{
    public class GetTaskQueryRequest : IAsyncRequest<GetTaskQueryResponse>
    {
        public long Id { get; set; }

        public long TaskId { get; set; }

        public bool IsProvider { get; set; } = true;
    }
}