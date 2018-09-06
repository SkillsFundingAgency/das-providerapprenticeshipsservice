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

                dataTable.Columns[0].ColumnName.Should().Be(nameof(CommitmentAgreement.OrganisationName));
            }

            [Test, AutoData]
            public void ThenSecondColumnIsCohortId(AgreementsViewModel sut)
            {
                var dataTable = sut.ToDataTable();

                dataTable.Columns[1].ColumnName.Should().Be(nameof(CommitmentAgreement.CohortID));
            }

            [Test, AutoData]
            public void ThenThirdColumnIsAgreementId(AgreementsViewModel sut)
            {
                var dataTable = sut.ToDataTable();

                dataTable.Columns[2].ColumnName.Should().Be(nameof(CommitmentAgreement.AgreementID));
            }

            //todo: use phil's technique to assert entire collection

            [Test, AutoData]
            public void ThenFirstRowHasCorrectOrganisationName(AgreementsViewModel sut)
            {
                var dataTable = sut.ToDataTable();

                dataTable.Rows[0][nameof(CommitmentAgreement.OrganisationName)]
                    .Should().Be(sut.CommitmentAgreements.First().OrganisationName);
            }

            [Test, AutoData]
            public void ThenFirstRowHasCorrectCohortId(AgreementsViewModel sut)
            {
                var dataTable = sut.ToDataTable();

                dataTable.Rows[0][nameof(CommitmentAgreement.CohortID)]
                    .Should().Be(sut.CommitmentAgreements.First().CohortID);
            }

            [Test, AutoData]
            public void ThenFirstRowHasCorrectAgreementId(AgreementsViewModel sut)
            {
                var dataTable = sut.ToDataTable();

                dataTable.Rows[0][nameof(CommitmentAgreement.AgreementID)]
                    .Should().Be(sut.CommitmentAgreements.First().AgreementID);
            }
        }
    }
}