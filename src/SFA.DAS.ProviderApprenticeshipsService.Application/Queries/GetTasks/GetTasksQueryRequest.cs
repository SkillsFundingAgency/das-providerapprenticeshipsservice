using MediatR;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetTasks
{
    public class GetTasksQueryRequest : IAsyncRequest<GetTasksQueryResponse>
    {
        public long Id { get; set; }

        public bool IsProvider { get; set; }
    }
}