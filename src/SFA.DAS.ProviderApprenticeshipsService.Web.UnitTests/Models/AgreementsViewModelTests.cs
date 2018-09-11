using System.Linq;
using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Agreement;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Models
{
    [TestFixture]
    public class AgreementsViewModelTests
    {
        [TestFixture]
        public class WhenCallingToDataTable
        {
            [Test, AutoData]
            public void ThenFirstColumnIsOrganisationName(AgreementsViewModel sut)
            {
                var dataTable = sut.ToDataTable();

                dataTable.Columns[0].ColumnName
                    .Should().Be(nameof(CommitmentAgreement.OrganisationName));
            }

            [Test, AutoData]
            public void ThenSecondColumnIsCohortId(AgreementsViewModel sut)
            {
                var dataTable = sut.ToDataTable();

                dataTable.Columns[1].ColumnName
                    .Should().Be(nameof(CommitmentAgreement.CohortID));
            }

            [Test, AutoData]
            public void ThenThirdColumnIsAgreementId(AgreementsViewModel sut)
            {
                var dataTable = sut.ToDataTable();

                dataTable.Columns[2].ColumnName
                    .Should().Be(nameof(CommitmentAgreement.AgreementID));
            }

            [Test, AutoData]
            public void ThenTheDataTableRowCountIsSameAsCommitmentAgreementsCount(AgreementsViewModel sut)
            {
                var dataTable = sut.ToDataTable();

                dataTable.Rows.Count
                    .Should().Be(sut.CommitmentAgreements.Count());
            }

            [Test, AutoData]
            public void ThenEachDataRowHasCorrectValuesAssigned(AgreementsViewModel sut)
            {
                var dataTable = sut.ToDataTable();
                var expected = sut.CommitmentAgreements.ToList();

                for (var i = 0; i < expected.Count; i++)
                {
                    dataTable.Rows[i][nameof(CommitmentAgreement.OrganisationName)]
                        .Should().Be(expected[i].OrganisationName);
                    dataTable.Rows[i][nameof(CommitmentAgreement.CohortID)]
                        .Should().Be(expected[i].CohortID);
                    dataTable.Rows[i][nameof(CommitmentAgreement.AgreementID)]
                        .Should().Be(expected[i].AgreementID);
                }
            }
        }
    }
}