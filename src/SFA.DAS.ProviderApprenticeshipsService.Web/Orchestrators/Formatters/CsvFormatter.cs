using System.Collections.Generic;
using System.IO;
using CsvHelper;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Agreement;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.Formatters
{
    public class CsvFormatter : ICsvFormatter
    {
        public byte[] Format(IEnumerable<CommitmentAgreement> commitmentAgreements)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (TextWriter textWriter = new StreamWriter(memoryStream))
                {

                    var csvWriter = new CsvWriter(textWriter);
                    csvWriter.WriteRecords(commitmentAgreements);

                    textWriter.Flush();
                    memoryStream.Seek(0L, SeekOrigin.Begin);

                    return memoryStream.ToArray();
                }
            }
        }
    }
}