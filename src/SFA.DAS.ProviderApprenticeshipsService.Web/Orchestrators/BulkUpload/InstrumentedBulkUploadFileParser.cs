﻿using System;
using System.Diagnostics;
using System.Linq;
using System.Web;
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
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));
            if (parser == null)
                throw new ArgumentNullException(nameof(parser));

            _logger = logger;
            _parser = parser;
        }

        public BulkUploadResult CreateViewModels(HttpPostedFileBase attachment)
        {
            var stopwatch = Stopwatch.StartNew();

            var result = _parser.CreateViewModels(attachment);

            _logger.Trace($"Took {stopwatch.ElapsedMilliseconds} milliseconds to create {result.Data.Count()} viewmodels");

            return result;
        }
    }
}