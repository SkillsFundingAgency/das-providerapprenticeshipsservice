﻿using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.ProviderApprenticeshipsService.Application.Services;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.UnitTests.Services
{
    [TestFixture]
    public class CommitmentsV2ServiceTests
    {
        private CommitmentsV2Service _sut;
        private Mock<ICommitmentsV2ApiClient> _commitmentsV2ApiClient;

        [SetUp]
        public void Arrange()
        {
            _commitmentsV2ApiClient = new Mock<ICommitmentsV2ApiClient>();
            _sut = new CommitmentsV2Service(_commitmentsV2ApiClient.Object);
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task IsCompleteForProviderReturnsTheStatusFromCohort(bool isComplete)
        {
            var autoFixture = new Fixture();
            var response = autoFixture.Build<GetCohortResponse>().With(p => p.IsCompleteForProvider, isComplete)
                .Create();

            _commitmentsV2ApiClient.Setup(x => x.GetCohort(It.IsAny<long>())).ReturnsAsync(response);

            var result = await _sut.CohortIsCompleteForProvider(1234);

            result.Should().Be(isComplete);
        }

        [Test]
        public async Task IsCompleteForProviderReceivesCorrectId()
        {
            var autoFixture = new Fixture();
            var response = autoFixture.Create<GetCohortResponse>();

            _commitmentsV2ApiClient.Setup(x => x.GetCohort(It.IsAny<long>())).ReturnsAsync(response);

            await _sut.CohortIsCompleteForProvider(1234);

            _commitmentsV2ApiClient.Verify(x=>x.GetCohort(1234));
        }
    }
}
