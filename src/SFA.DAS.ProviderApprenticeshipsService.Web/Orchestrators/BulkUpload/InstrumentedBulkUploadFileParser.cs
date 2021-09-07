using System.Diagnostics;
using System.Linq;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.NLog.Logger;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.BulkUpload;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.BulkUpload
{
    public sealed class InstrumentedBulkUploadFileParser : IBulkUploadFileParser
    {
        private readonly ILog _logger;
        private readonly IBulkUploadFileParser _parser;

        public InstrumentedBulkUploadFileParser(ILog logger, IBulkUploadFileParser parser)
        {
            _logger = logger;
            _parser = parser;
        }

        public BulkUploadResult CreateViewModels(long providerId, CommitmentView commitment, string fileContent, bool blackListed)
        {
            var stopwatch = Stopwatch.StartNew();

            var result = _parser.CreateViewModels(providerId, commitment, fileContent, blackListed); //TODO : check blacklist

            _logger.Trace($"Took {stopwatch.ElapsedMilliseconds} milliseconds to create {result.Data?.Count()} viewmodels");

            return result;
        }
    }
}