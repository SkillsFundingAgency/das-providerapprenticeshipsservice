using MediatR;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetClientContent;

public class GetClientContentRequest : IRequest<GetClientContentResponse>
{
    public string ContentType { get; set; }
    public bool UseLegacyStyles { get; set; }
}