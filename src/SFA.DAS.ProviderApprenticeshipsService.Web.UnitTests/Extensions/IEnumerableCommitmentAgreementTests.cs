using System.Collections.Generic;
using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.ProviderApprenticeshipsService.Web.Extensions;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Agreement;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Extensions
{
    [TestFixture]
    public class IEnumerableCommitmentAgreementTests
    {
        [TestFixture]
        public class WhenCallingToDataTable
        {
            [Test, AutoData]
            public void ThenFirstColumnIsOrganisationName(List<CommitmentAgreement> sut)
            {
                var dataTable = sut.ToDataTable();

                dataTable.Columns[0].ColumnName
                    .Should().Be(nameof(CommitmentAgreement.OrganisationName));
            }

            [Test, AutoData]
            public void ThenSecondColumnIsCohortId(List<CommitmentAgreement> sut)
            {
                var dataTable = sut.ToDataTable();

                dataTable.Columns[1].ColumnName
                    .Should().Be(nameof(CommitmentAgreement.CohortID));
            }

            [Test, AutoData]
            public void ThenThirdColumnIsAgreementId(List<CommitmentAgreement> sut)
            {
                var dataTable = sut.ToDataTable();

                dataTable.Columns[2].ColumnName
                    .Should().Be(nameof(CommitmentAgreement.AgreementID));
            }

            [Test, AutoData]
            public void ThenTheDataTableRowCountIsSameAsCommitmentAgreementsCount(List<CommitmentAgreement> sut)
            {
                var dataTable = sut.ToDataTable();

                dataTable.Rows.Count
                    .Should().Be(sut.Count);
            }

            [Test, AutoData]
            public void ThenEachDataRowHasCorrectValuesAssigned(List<CommitmentAgreement> sut)
            {
                var expected = new CommitmentAgreement[sut.Count];
                sut.CopyTo(expected);

                var dataTable = sut.ToDataTable();

                for (var i = 0; i < expected.Length; i++)
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