using MediatR;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetAllApprentices
{
    public class GetAllApprenticesRequest : IAsyncRequest<GetAllApprenticesResponse>
    {
        public long ProviderId { get; set; }
    }
}