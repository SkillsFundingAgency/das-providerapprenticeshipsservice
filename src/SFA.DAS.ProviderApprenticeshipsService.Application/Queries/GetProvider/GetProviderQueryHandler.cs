using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces.Data;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetProvider;

public class GetProviderQueryHandler : IRequestHandler<GetProviderQueryRequest, GetProviderQueryResponse>
{
    private readonly IProviderRepository _providerRepository;

    public GetProviderQueryHandler(IProviderRepository providerRepository)
    {
        _providerRepository = providerRepository;
    }

    public async Task<GetProviderQueryResponse> Handle(GetProviderQueryRequest message, CancellationToken cancellationToken)
    {
        var provider = await _providerRepository.GetProvider(message.UKPRN);

        return new GetProviderQueryResponse
        {
            Provider = provider
        };
    }
}