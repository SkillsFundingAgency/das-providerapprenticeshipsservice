using System.Collections.Generic;
using System.IO;
using ClosedXML.Excel;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Agreement;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.Formatters;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Orchestrators.Formatters
{
    [TestFixture]
    public class GivenAnExcelFormatter
    {
        [TestFixture]
        public class WhenCallingFormat
        {
            [Test, MoqCustomisedAutoData]
            public void ThenTheExcelResultHasColumnHeaders(
                List<CommitmentAgreement> agreements,
                ExcelFormatter sut)
            {
                var byteResult = sut.Format(TestHelper.Clone(agreements));

                using (var memoryStream = new MemoryStream(byteResult))
                {
                    var workbook = new XLWorkbook(memoryStream);

                    workbook.Worksheets.Worksheet("Agreements").Should().NotBeNull();

                    var worksheet = workbook.Worksheets.Worksheet("Agreements");

                    // note the offset of 1... VB (vomit emoji)
                    worksheet.Column(1).Cell(1).GetValue<string>().Should().Be(nameof(CommitmentAgreement.OrganisationName));                    
                    worksheet.Column(2).Cell(1).GetValue<string>().Should().Be(nameof(CommitmentAgreement.AgreementID));
                }
            }

            [Test, MoqCustomisedAutoData]
            public void ThenTheExcelResultHasRowData(
                List<CommitmentAgreement> agreements,
                ExcelFormatter sut)
            {
                var byteResult = sut.Format(TestHelper.Clone(agreements));

                using (var memoryStream = new MemoryStream(byteResult))
                {
                    var workbook = new XLWorkbook(memoryStream);
                    var worksheet = workbook.Worksheets.Worksheet("Agreements");

                    worksheet.Column(1).Cell(2).GetValue<string>().Should().Be(agreements[0].OrganisationName);                    
                    worksheet.Column(2).Cell(2).GetValue<string>().Should().Be(agreements[0].AgreementID);
                }
            }
        }
    }
}