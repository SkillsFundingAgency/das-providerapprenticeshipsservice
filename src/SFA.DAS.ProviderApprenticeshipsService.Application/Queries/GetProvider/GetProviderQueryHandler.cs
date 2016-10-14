using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetProvider
{
    public class GetProviderQueryHandler : IAsyncRequestHandler<GetProviderQueryRequest, GetProviderQueryResponse>
    {
        private readonly IApprenticeshipInfoServiceWrapper _apprenticeshipInfoServiceWrapper;

        public GetProviderQueryHandler(IApprenticeshipInfoServiceWrapper apprenticeshipInfoServiceWrapper)
        {
            if (apprenticeshipInfoServiceWrapper == null)
                throw new ArgumentNullException(nameof(apprenticeshipInfoServiceWrapper));
            _apprenticeshipInfoServiceWrapper = apprenticeshipInfoServiceWrapper;
        }

        public async Task<GetProviderQueryResponse> Handle(GetProviderQueryRequest message)
        {
            var provider = _apprenticeshipInfoServiceWrapper.GetProvider(message.UKPRN);

            return new GetProviderQueryResponse();
        }
    }
}