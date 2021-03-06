﻿using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Commitments.Api.Client.Interfaces;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetBulkUploadFile
{
    public class GetBulkUploadFileQueryHandler : IRequestHandler<GetBulkUploadFileQueryRequest, GetBulkUploadFileQueryResponse>
    {
        private readonly IProviderCommitmentsApi _commitmentsApi;

        public GetBulkUploadFileQueryHandler(IProviderCommitmentsApi commitmentsApi)
        {
            _commitmentsApi = commitmentsApi;
        }

        public async Task<GetBulkUploadFileQueryResponse> Handle(GetBulkUploadFileQueryRequest message, CancellationToken cancellationToken)
        {
            var result = await _commitmentsApi.BulkUploadFile(message.ProviderId, message.BulkUploadId);

            return new GetBulkUploadFileQueryResponse
            {
                FileContent = result
            };

        }
    }
}
