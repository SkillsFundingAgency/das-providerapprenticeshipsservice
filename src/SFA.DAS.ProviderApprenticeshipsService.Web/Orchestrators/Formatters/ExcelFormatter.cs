using System.Collections.Generic;
using System.IO;
using ClosedXML.Excel;
using SFA.DAS.ProviderApprenticeshipsService.Web.Extensions;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Agreement;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.Formatters
{
    public class ExcelFormatter : IExcelFormatter
    {
        public byte[] Format(IEnumerable<CommitmentAgreement> commitmentAgreements)
        {
            var workbook = new XLWorkbook();
            workbook.AddWorksheet(commitmentAgreements.ToDataTable(), "Agreements");

            using (var memoryStream = new MemoryStream())
            {
                workbook.SaveAs(memoryStream);
                memoryStream.Seek(0L, SeekOrigin.Begin);

                return memoryStream.ToArray();
            }
        }
    }
}