using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Agreement;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.Formatters;
using static System.Text.Encoding;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Orchestrators.Formatters
{
    [TestFixture]
    public class GivenACsvFormatter
    {
        [TestFixture]
        public class WhenCallingFormat
        {
            [Test, MoqCustomisedAutoData]
            public void ThenTheResultStartsWithCsvHeaders(
                List<CommitmentAgreement> agreements,
                CsvFormatter sut)
            {
                var byteResult = sut.Format(TestHelper.Clone(agreements));

                var stringResult = Default.GetString(byteResult);
                stringResult.Should().StartWith(
                    $"{nameof(CommitmentAgreement.OrganisationName)},{nameof(CommitmentAgreement.CohortID)},{nameof(CommitmentAgreement.AgreementID)}");
            }

            [Test, MoqCustomisedAutoData]
            public void ThenTheResultContainsCsvAgreements(
                List<CommitmentAgreement> agreements,
                CsvFormatter sut)
            {
                var byteResult = sut.Format(TestHelper.Clone(agreements));

                var stringResult = Default.GetString(byteResult);
                stringResult.Should().Contain($"{agreements[0].OrganisationName},{agreements[0].CohortID},{agreements[0].AgreementID}");
                stringResult.Should().Contain($"{agreements[1].OrganisationName},{agreements[1].CohortID},{agreements[1].AgreementID}");
                stringResult.Should().Contain($"{agreements[2].OrganisationName},{agreements[2].CohortID},{agreements[2].AgreementID}");
            }
        }
    }
}