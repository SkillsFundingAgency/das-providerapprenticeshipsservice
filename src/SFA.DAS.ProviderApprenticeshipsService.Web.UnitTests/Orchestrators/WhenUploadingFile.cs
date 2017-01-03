using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

using FluentAssertions;

using MediatR;

using Moq;

using NUnit.Framework;

using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetFrameworks;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetStandards;
using SFA.DAS.ProviderApprenticeshipsService.Domain;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Orchestrators
{
    [TestFixture]
    public class WhenUploadingFile
    {
        private BulkUploadOrchestrator _sut;

        private Mock<HttpPostedFileBase> _file;

        [SetUp]
        public void Setup()
        {
            _file = new Mock<HttpPostedFileBase>();
            _file.Setup(m => m.FileName).Returns("APPDATA-20051030-213855.csv");
            _file.Setup(m => m.ContentLength).Returns(400);

            var mediator = new Mock<IMediator>();
            mediator.Setup(m => m.SendAsync(It.IsAny<GetStandardsQueryRequest>()))
                .Returns(Task.Run(() => new GetStandardsQueryResponse { Standards = new List<Standard>
                                                                                        {
                                                                                            new Standard {Id = "2", Title = "Hej" }
                                                                                        } }));
            mediator.Setup(m => m.SendAsync(It.IsAny<GetFrameworksQueryRequest>()))
                .Returns(Task.Run(() => new GetFrameworksQueryResponse { Frameworks = new List<Framework>
                                                                                          {
                                                                                              new Framework { Id = "1-2-3" }
                                                                                          } }));
            _sut = new BulkUploadOrchestrator(mediator.Object, new BulkUploader(), Mock.Of<IHashingService>());
        }

        [Test]
        public void TestPerformance()
        {
            var testDataHead = @"CohortRef,GivenNames,FamilyName,DateOfBirth,NINumber,FworkCode,PwayCode,ProgType,StdCode,LearnStartDate,LearnPlanEndDate,TrainingPrice,EPAPrice,EPAOrgId,EmpRef,ProvRef,ULN";
            var upper = 40 * 1000;
            var testData = new List<string>();
            for (int i = 0; i < upper; i++)
            {
                testData.Add("\n\rAbba123,Chris,Froberg,1998-12-08,SE1233211C,,,,2,2020-08-01,2025-08-01,1500,,,Employer ref,Provider ref,1113335559");
            }
            var str = testDataHead + string.Join("", testData);

            var textStream = new MemoryStream(Encoding.UTF8.GetBytes(str));
            _file.Setup(m => m.InputStream).Returns(textStream);

            var model = new UploadApprenticeshipsViewModel { Attachment = _file.Object };
            var stopwatch = Stopwatch.StartNew();
            var result = _sut.UploadFile(model);
            stopwatch.Stop(); System.Console.WriteLine($"Time TOTAL: {stopwatch.Elapsed.Seconds}");
            result.Result.Errors.Count().Should().Be(200 * 1000);
            stopwatch.Elapsed.Seconds.Should().BeLessThan(7);   
        }
    }
}
