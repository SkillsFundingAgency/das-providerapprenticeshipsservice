using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Commands.TriageApprenticeshipDataLocks
{
    public class TriageApprenticeshipDataLocksCommandHandler : AsyncRequestHandler<TriageApprenticeshipDataLocksCommand>
    {
        private readonly IDataLockApi _dataLockApi;

        private readonly ILog _logger;

        public TriageApprenticeshipDataLocksCommandHandler(IDataLockApi dataLockApi, ILog logger)
        {
            _dataLockApi = dataLockApi;
            _logger = logger;


        }

        protected override Task HandleCore(TriageApprenticeshipDataLocksCommand message)
        {
            throw new System.NotImplementedException();
        }
    }
}
