using MediatR;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetTrainingProgrammes
{
    public sealed class GetTrainingProgrammesQueryRequest : IAsyncRequest<GetTrainingProgrammesQueryResponse>
    {
        public GetTrainingProgrammesQueryRequest()
        {
            IncludeFrameworks = true;
        }

        public bool IncludeFrameworks { get; set; }
    }
}
