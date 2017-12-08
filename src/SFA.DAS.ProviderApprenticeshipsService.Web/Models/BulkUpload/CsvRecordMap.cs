using CsvHelper.Configuration;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Models.BulkUpload
{
    public sealed class CsvRecordMap : CsvClassMap<CsvRecord>
    {
        public CsvRecordMap()
        {
            AutoMap();
        }
    }
}