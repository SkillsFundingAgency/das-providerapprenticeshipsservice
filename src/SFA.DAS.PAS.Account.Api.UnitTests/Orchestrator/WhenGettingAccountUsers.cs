using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.PAS.Account.Api.Orchestrator;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetAccountUsers;
using SFA.DAS.ProviderApprenticeshipsService.Domain;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models.UserProfile;

namespace SFA.DAS.PAS.Account.Api.UnitTests.Orchestrator
{
    [TestFixture]
    public class WhenGettingAccountUsers
    {
        private AccountOrchestrator _sut;
        private Mock<IMediator> _mediator;
        private Fixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _fixture = new Fixture();
            _mediator = new Mock<IMediator>();
            _sut = new AccountOrchestrator(_mediator.Object, Mock.Of<IProviderCommitmentsLogger>());
        }


        [Test]
        public async Task ShouldReturnUsers()
        {
            var response = new GetAccountUsersResponse();
            response.Add(_fixture.Build<User>().With(m => m.UserRef, "userRef1").Create(), _fixture.Build<UserSetting>().With(m => m.ReceiveNotifications, true).Create());
            response.Add(_fixture.Build<User>().With(m => m.UserRef, "userRef2").Create(), _fixture.Build<UserSetting>().With(m => m.ReceiveNotifications, false).Create());

            _mediator.Setup(m => m.SendAsync(It.IsAny<GetAccountUsersQuery>())).ReturnsAsync(response);
            var result = (await _sut.GetAccountUsers(12345)).ToArray();

            result.Length.Should().Be(2);
            result[0].UserRef.Should().Be("userRef1");
            result[0].ReceiveNotifications.Should().BeTrue();
            result[1].UserRef.Should().Be("userRef2");
            result[1].ReceiveNotifications.Should().BeFalse();
        }

        [Test]
        public async Task UserShouldReceiveEmailIfNotSettings()
        {
            var response = new GetAccountUsersResponse();
            response.Add(_fixture.Build<User>().With(m => m.UserRef, "userRef1").Create(), null);
            response.Add(_fixture.Build<User>().With(m => m.UserRef, "userRef2").Create(), _fixture.Build<UserSetting>().With(m => m.ReceiveNotifications, false).Create());

            _mediator.Setup(m => m.SendAsync(It.IsAny<GetAccountUsersQuery>())).ReturnsAsync(response);
            var result = (await _sut.GetAccountUsers(12345)).ToArray();

            result.Length.Should().Be(2);
            result[0].UserRef.Should().Be("userRef1");
            result[0].ReceiveNotifications.Should().BeTrue();
            result[1].UserRef.Should().Be("userRef2");
            result[1].ReceiveNotifications.Should().BeFalse();
        }

        [Test]
        public async Task NoUsersReturned()
        {
            var response = new GetAccountUsersResponse();

            _mediator.Setup(m => m.SendAsync(It.IsAny<GetAccountUsersQuery>())).ReturnsAsync(response);
            var result = (await _sut.GetAccountUsers(12345)).ToArray();

            result.Length.Should().Be(0);
        }
    }
}
