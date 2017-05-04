using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using FluentAssertions;
using Moq;
using NUnit.Framework;

using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types.DataLock;
using SFA.DAS.NLog.Logger;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetApprenticeshipDataLock;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Orchestrators.Commitments
{
    [TestFixture]
    public class WhenGettingDataLock
    {
        private GetApprenticeshipDataLockHandler _sut;

        private Mock<IDataLockApi> _dataLockApi;

        private Mock<ILog> _logger;

        [SetUp]
        public void SetUp()
        {
            _dataLockApi = new Mock<IDataLockApi>();
            _logger = new Mock<ILog>();
            _sut = new GetApprenticeshipDataLockHandler(_dataLockApi.Object);
        }

        [Test]
        public async Task ShouldGetDataLockStatus()
        {
            _dataLockApi.Setup(m => m.GetDataLocks(1))
                .ReturnsAsync(new List<DataLockStatus> { new DataLockStatus { IsResolved = false, ApprenticeshipId = 1, IlrTotalCost = 1500 } });

            var request = new GetApprenticeshipDataLockRequest { ApprenticeshipId = 1 };
            var response = await _sut.Handle(request);

            response.Data.IsResolved.Should().BeFalse();
            response.Data.ApprenticeshipId.Should().Be(1);
            response.Data.IlrTotalCost.Should().Be(1500);
        }

        [Test]
        public async Task ShouldBeNullIfDataLockStatusIsResolved()
        {
            _dataLockApi.Setup(m => m.GetDataLocks(1))
                .ReturnsAsync(new List<DataLockStatus> { new DataLockStatus { IsResolved = true } });

            var request = new GetApprenticeshipDataLockRequest { ApprenticeshipId = 1 };
            var response = await _sut.Handle(request);

            response.Data.Should().BeNull();
        }

        [Test]
        public void ShouldThrowExceptionIfApiThrowsAnError()
        {
            _dataLockApi.Setup(m => m.GetDataLocks(It.IsAny<long>())).Throws<Exception>();
            var request = new GetApprenticeshipDataLockRequest { ApprenticeshipId = 1 };

            Func<Task<GetApprenticeshipDataLockResponse>> act = async () =>  await _sut.Handle(request);

            act.ShouldThrow<Exception>();
        }
    }
}
