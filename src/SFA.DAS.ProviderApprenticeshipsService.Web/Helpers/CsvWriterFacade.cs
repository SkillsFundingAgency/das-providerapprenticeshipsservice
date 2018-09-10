using System.Collections;
using System.IO;
using System.Web.Mvc;
using CsvHelper;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Helpers
{
    public static class CsvWriterFacade
    {
        private const string CsvContentType = "text/csv";

        public static FileStreamResult Deliver(IEnumerable records, string fileName)
        {
            var memoryStream = new MemoryStream();
            TextWriter textWriter = new StreamWriter(memoryStream);

            var csvWriter = new CsvWriter(textWriter);
            csvWriter.WriteRecords(records);

            textWriter.Flush();
            memoryStream.Seek(0L, SeekOrigin.Begin);

            var fileStreamResult = new FileStreamResult(memoryStream, CsvContentType)
            {
                FileDownloadName = fileName
            };
            return fileStreamResult;
        }
    }
}