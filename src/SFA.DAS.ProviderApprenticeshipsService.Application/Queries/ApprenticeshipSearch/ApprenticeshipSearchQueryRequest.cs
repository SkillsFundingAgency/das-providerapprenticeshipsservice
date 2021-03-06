﻿using MediatR;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.ApprenticeshipSearch
{
    public class ApprenticeshipSearchQueryRequest : IRequest<ApprenticeshipSearchQueryResponse>
    {
        public long ProviderId { get; set; }
        public ApprenticeshipSearchQuery Query { get; set; }
    }
}
