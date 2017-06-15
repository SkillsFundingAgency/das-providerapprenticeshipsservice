using System.Collections.Generic;
using System.Threading.Tasks;
using Castle.Components.DictionaryAdapter;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types.DataLock;
using SFA.DAS.Commitments.Api.Types.DataLock.Types;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetFrameworks;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetStandards;
using SFA.DAS.ProviderApprenticeshipsService.Domain;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.Mappers;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Orchestrators.Commitments.Mappers
{
    [TestFixture]
    public class WhenMappingDataLocks
    {
        private Mock<IMediator> _mediator;
        private DataLockMapper _mapper;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetStandardsQueryRequest>()))
                .ReturnsAsync(() => new GetStandardsQueryResponse
                {
                    Standards = new List<Standard>
                    {
                        new Standard
                        {
                            Id = "TEST"
                        }
                    }
                });

            _mediator.Setup(x => x.SendAsync(It.IsAny<GetFrameworksQueryRequest>()))
                .ReturnsAsync(() => new GetFrameworksQueryResponse
                {
                    Frameworks = new List<Framework>()
                });

            _mapper = new DataLockMapper(_mediator.Object);
        }

        [Test(Description = "Show Changes flag set when apprenticeship has a course data lock triaged as Restart")]
        public async Task ThenSummarySetsShowChangesRequestedCorrectly()
        {
            //Arrange
            var source = new DataLockSummary
            {
                DataLockWithCourseMismatch = new List<DataLockStatus>
                {
                    new DataLockStatus { IlrTrainingCourseCode = "TEST", TriageStatus = TriageStatus.Restart }
                },
                DataLockWithOnlyPriceMismatch = new List<DataLockStatus>()
            };

            //Act
            var result = await _mapper.MapDataLockSummary(source);

            //Assert
            Assert.IsTrue(result.ShowChangesRequested);
        }
    }
}
