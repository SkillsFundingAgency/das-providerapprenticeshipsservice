using System.Collections.Generic;
using System.Data;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Models.Agreement
{
    public class AgreementsViewModel
    {
        public IEnumerable<CommitmentAgreement> CommitmentAgreements;

        public DataTable ToDataTable()
        {
            var dataTable = new DataTable("Agreements");

            var dataColumns = new[]
            {
                new DataColumn(nameof(CommitmentAgreement.OrganisationName)),
                new DataColumn(nameof(CommitmentAgreement.CohortID)),
                new DataColumn(nameof(CommitmentAgreement.AgreementID))
            };

            dataTable.Columns.AddRange(dataColumns);

            foreach (var agreement in CommitmentAgreements)
            {
                var row = dataTable.NewRow();
                row[0] = agreement.OrganisationName;
                row[1] = agreement.CohortID;
                row[2] = agreement.AgreementID;
                dataTable.Rows.Add(row);
            }

            dataTable.AcceptChanges();

            return dataTable;
        }
    }
}