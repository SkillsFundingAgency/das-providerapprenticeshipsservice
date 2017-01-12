using System;
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
using SFA.DAS.NLog.Logger;
using SFA.DAS.ProviderApprenticeshipsService.Application.Commands.BulkUploadApprenticeships;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetFrameworks;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetStandards;
using SFA.DAS.ProviderApprenticeshipsService.Domain;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.BulkUpload;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Orchestrators
{
    [TestFixture]
    public sealed class WhenUploadingFile
    {
        private const string HeaderLine = @"CohortRef,GivenNames,FamilyName,DateOfBirth,NINumber,FworkCode,PwayCode,ProgType,StdCode,LearnStartDate,LearnPlanEndDate,TrainingPrice,EPAPrice,EPAOrgId,EmpRef,ProviderRef,ULN";
        private BulkUploadOrchestrator _sut;
        private Mock<HttpPostedFileBase> _file;
        private Mock<IMediator> _mockMediator;

        [SetUp]
        public void Setup()
        {
            var mockHashingService = new Mock<IHashingService>();
            mockHashingService.Setup(x => x.DecodeValue(It.IsAny<string>())).Returns(123L);

            _file = new Mock<HttpPostedFileBase>();
            _file.Setup(m => m.FileName).Returns("APPDATA-20051030-213855.csv");
            _file.Setup(m => m.ContentLength).Returns(400);

            _mockMediator = new Mock<IMediator>();
            _mockMediator.Setup(m => m.SendAsync(It.IsAny<GetStandardsQueryRequest>()))
                .Returns(Task.Run(() => new GetStandardsQueryResponse { Standards = new List<Standard>
                                                                                        {
                                                                                            new Standard {Id = "2", Title = "Hej" }
                                                                                        } }));
            _mockMediator.Setup(m => m.SendAsync(It.IsAny<GetFrameworksQueryRequest>()))
                .Returns(Task.Run(() => new GetFrameworksQueryResponse { Frameworks = new List<Framework>
                                                                                          {
                                                                                              new Framework { Id = "1-2-3" }
                                                                                          } }));

            var uploadValidator = new BulkUploadValidator(new ProviderApprenticeshipsServiceConfiguration { MaxBulkUploadFileSize = 512 }, Mock.Of<ILog>());
            var uploadFileParser = new BulkUploadFileParser(Mock.Of<ILog>());
            var bulkUploader = new BulkUploader(_mockMediator.Object, uploadValidator, uploadFileParser, Mock.Of<IProviderCommitmentsLogger>());

            _sut = new BulkUploadOrchestrator(_mockMediator.Object, bulkUploader, mockHashingService.Object, Mock.Of<IProviderCommitmentsLogger>());
        }

        [Test]
        public async Task TestPerformance()
        {
            var upper = 40 * 1000;
            var testData = new List<string>();
            for (int i = 0; i < upper; i++)
            {
                testData.Add("\n\rABBA123,Chris,Froberg,1998-12-08,SE1233211C,,,,2,2020-08-01,2025-08-01,1500,,,Employer ref,Provider ref,1113335559");
            }
            var str = HeaderLine + string.Join("", testData);

            var textStream = new MemoryStream(Encoding.UTF8.GetBytes(str));
            _file.Setup(m => m.InputStream).Returns(textStream);

            var model = new UploadApprenticeshipsViewModel { Attachment = _file.Object, HashedCommitmentId = "ABBA123" };
            var stopwatch = Stopwatch.StartNew();
            var result = await _sut.UploadFileAsync(model);
            stopwatch.Stop(); Console.WriteLine($"Time TOTAL: {stopwatch.Elapsed.Seconds}");
            result.Errors.Count().Should().Be(160 * 1000);
            stopwatch.Elapsed.Seconds.Should().BeLessThan(7);   
        }

        [Test]
        public async Task ShouldCallMediatorPassingInMappedApprenticeships()
        {
            var dataLine = "\n\rABBA123,Chris,Froberg,1998-12-08,SE123321C,,,25,2,2020-08-01,2025-08-01,1500,,,Employer ref,Provider ref,1113335559";
            var fileContents = HeaderLine + dataLine;
            var textStream = new MemoryStream(Encoding.UTF8.GetBytes(fileContents));
            _file.Setup(m => m.InputStream).Returns(textStream);

            BulkUploadApprenticeshipsCommand commandArgument = null;
            _mockMediator.Setup(x => x.SendAsync(It.IsAny<BulkUploadApprenticeshipsCommand>()))
                .ReturnsAsync(new Unit())
                .Callback((object x) => commandArgument = x as BulkUploadApprenticeshipsCommand);

            var model = new UploadApprenticeshipsViewModel { Attachment = _file.Object, HashedCommitmentId = "ABBA123", ProviderId = 111 };
            var result = await _sut.UploadFileAsync(model);

            _mockMediator.Verify(x => x.SendAsync(It.IsAny<BulkUploadApprenticeshipsCommand>()), Times.Once);

            commandArgument.ProviderId.Should().Be(111);
            commandArgument.CommitmentId.Should().Be(123);

            commandArgument.Apprenticeships.Should().NotBeEmpty();
            commandArgument.Apprenticeships.ToList()[0].FirstName.Should().Be("Chris");
            commandArgument.Apprenticeships.ToList()[0].LastName.Should().Be("Froberg");
            commandArgument.Apprenticeships.ToList()[0].DateOfBirth.Should().Be(new DateTime(1998, 12, 8));
            commandArgument.Apprenticeships.ToList()[0].TrainingType.Should().Be(Commitments.Api.Types.TrainingType.Standard);
            commandArgument.Apprenticeships.ToList()[0].TrainingCode.Should().Be("2");
            commandArgument.Apprenticeships.ToList()[0].StartDate.Should().Be(new DateTime(2020, 8, 1));
            commandArgument.Apprenticeships.ToList()[0].EndDate.Should().Be(new DateTime(2025, 8, 1));
            commandArgument.Apprenticeships.ToList()[0].Cost.Should().Be(1500);
            commandArgument.Apprenticeships.ToList()[0].ProviderRef.Should().Be("Provider ref");
            commandArgument.Apprenticeships.ToList()[0].ULN.Should().Be("1113335559");
        }
    }
}
