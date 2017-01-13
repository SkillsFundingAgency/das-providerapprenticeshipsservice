using System;
using System.Diagnostics;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Commands.BulkUploadApprenticeships
{
    public sealed class InstrumentedBulkUploadApprenticeshipsCommandHandler : AsyncRequestHandler<BulkUploadApprenticeshipsCommand>
    {
        private readonly AsyncRequestHandler<BulkUploadApprenticeshipsCommand> _handler;
        private readonly IProviderCommitmentsLogger _logger;

        public InstrumentedBulkUploadApprenticeshipsCommandHandler(IProviderCommitmentsLogger logger, AsyncRequestHandler<BulkUploadApprenticeshipsCommand> handler)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            _logger = logger;
            _handler = handler;
        }

        protected override async Task HandleCore(BulkUploadApprenticeshipsCommand message)
        {
            var stopwatch = Stopwatch.StartNew();

            await _handler.Handle(message);

            _logger.Trace($"Took {stopwatch.ElapsedMilliseconds} milliseconds to Bulk Upload {message.Apprenticeships.Count} apprentices to Commitments Api");
        }
    }
}
