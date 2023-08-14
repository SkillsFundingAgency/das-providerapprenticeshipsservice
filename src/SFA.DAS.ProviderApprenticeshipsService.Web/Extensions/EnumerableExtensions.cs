using System.Data;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Agreement;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Extensions;

public static class EnumerableExtensions
{
    public static bool IsNullOrEmpty<T>(this IEnumerable<T> value)
    {
        return value == null || !value.Any();
    }

    public static DataTable ToDataTable(this IEnumerable<CommitmentAgreement> commitmentAgreements)
    {
        var dataTable = new DataTable("Agreements");
        var dataColumns = new[]
        {
            new DataColumn(nameof(CommitmentAgreement.OrganisationName)),                
            new DataColumn(nameof(CommitmentAgreement.AgreementID))
        };
        
        dataTable.Columns.AddRange(dataColumns);
        
        foreach (var agreement in commitmentAgreements)
        {
            var row = dataTable.NewRow();
            row[0] = agreement.OrganisationName;                
            row[1] = agreement.AgreementID;
            dataTable.Rows.Add(row);
        }
        
        dataTable.AcceptChanges();
        
        return dataTable;
    }
}