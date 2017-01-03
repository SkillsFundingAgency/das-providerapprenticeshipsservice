using System;
using System.Threading.Tasks;
using MediatR;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Commands.BulkUploadApprenticeships
{
    public sealed class BulkUploadApprenticeshipsCommandHandler : AsyncRequestHandler<BulkUploadApprenticeshipsCommand>
    {
        protected override Task HandleCore(BulkUploadApprenticeshipsCommand message)
        {
            throw new NotImplementedException();
        }
    }
}
