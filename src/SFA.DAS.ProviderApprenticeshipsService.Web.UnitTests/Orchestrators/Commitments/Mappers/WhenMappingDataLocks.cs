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

        [Test]
        public async Task ThenSummaryShowsChangesRequestedWhenADataLockCourseHasBeenTriaged()
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
            var result = await _mapper.MapDataLockSummary(source, false);

            //Assert
            Assert.IsTrue(result.ShowChangesRequested);
        }

        [Test]
        public async Task ThenSummaryShowsChangesPendingWhenADataLockPricehasBeenTriaged()
        {
            //Arrange
            var source = new DataLockSummary
            {
                DataLockWithOnlyPriceMismatch = new List<DataLockStatus>
                {
                    new DataLockStatus { IlrTrainingCourseCode = "TEST", TriageStatus = TriageStatus.Change }
                },
                DataLockWithCourseMismatch = new List<DataLockStatus>()
            };

            //Act
            var result = await _mapper.MapDataLockSummary(source, false);

            //Assert
            Assert.IsTrue(result.ShowChangesPending);
        }

        [TestCase(true, false, true, Description="Course triage available when datalock course mismatch is pending and no other has been triaged")]
        [TestCase(true, true, false, Description = "Course triage unavailable when another has already been triaged (must be fully resolved one-at-a-time)")]
        [TestCase(false, false, false, Description = "Course triage unavailable if nothing to triage")]
        [TestCase(false, true, false, Description = "Course triage unavailable if nothing to triage")]
        public async Task ThenSummaryShowsDataLockCourseTriage(bool hasUntriagedCourseDataLock, bool hasTriagedCourseDataLock, bool expectEnabled)
        {
            //Arrange
            var courseDataLocks = new List<DataLockStatus>();
            if (hasUntriagedCourseDataLock)
            {
                courseDataLocks.Add(
                    new DataLockStatus { IlrTrainingCourseCode = "TEST", TriageStatus = TriageStatus.Unknown});
            }

            if (hasTriagedCourseDataLock)
            {
                courseDataLocks.Add(
                    new DataLockStatus { IlrTrainingCourseCode = "TEST", TriageStatus = TriageStatus.Restart });
            }

            var source = new DataLockSummary
            {
                DataLockWithOnlyPriceMismatch = new List<DataLockStatus>(),
                DataLockWithCourseMismatch = courseDataLocks
            };

            //Act
            var result = await _mapper.MapDataLockSummary(source, false);

            //Assert
            Assert.AreEqual(expectEnabled, result.ShowCourseDataLockTriageLink);
        }

        [TestCase(true, true, Description="Price Triage available when there is a Data Lock Price to triage")]
        [TestCase(false, false, Description = "Price Triage available when there is a Data Lock Price to triage")]
        public async Task ThenSummaryShowsDataLockPriceTriage(bool priceTriagePending, bool expectEnabled)
        {
            var source = new DataLockSummary
            {
                DataLockWithOnlyPriceMismatch = priceTriagePending ? new List<DataLockStatus>
                {
                    {
                        new DataLockStatus { IlrTrainingCourseCode = "TEST", TriageStatus = TriageStatus.Unknown}
                    }
                } : new List<DataLockStatus>(),
                DataLockWithCourseMismatch = new List<DataLockStatus>()
            };

            //Act
            var result = await _mapper.MapDataLockSummary(source, false);

            //Assert
            Assert.AreEqual(expectEnabled, result.ShowPriceDataLockTriageLink);
        }
    }
}
