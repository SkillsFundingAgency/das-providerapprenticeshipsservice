using AutoFixture;
using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.PAS.Account.Api.Orchestrator;
using SFA.DAS.PAS.Account.Application.Queries.GetAccountUsers;
using SFA.DAS.ProviderApprenticeshipsService.Domain;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models.UserProfile;

namespace SFA.DAS.PAS.Account.Api.UnitTests.Orchestrator
{
    [TestFixture]
    public class WhenGettingAccountUsers
    {
        private AccountOrchestrator _sut;
        private Mock<IMediator> _mediator;
        private Fixture _fixture;
        private User _normalUser;
        private User _superUser;

        [SetUp]
        public void SetUp()
        {
            _fixture = new Fixture();
            _mediator = new Mock<IMediator>();
            _sut = new AccountOrchestrator(_mediator.Object);
            _normalUser = _fixture.Build<User>().With(u => u.UserType, UserType.NormalUser).Create();
            _superUser = _fixture.Build<User>().With(u => u.UserType, UserType.SuperUser).Create();
        }

        [Test]
        public async Task ShouldReturnUsers()
        {

            var response = new GetAccountUsersResponse();

            response.Add(_superUser, _fixture.Build<UserSetting>().With(m => m.ReceiveNotifications, true).Create());
            response.Add(_normalUser, _fixture.Build<UserSetting>().With(m => m.ReceiveNotifications, false).Create());

            _mediator.Setup(m => m.Send(It.IsAny<GetAccountUsersQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(response);
            var result = (await _sut.GetAccountUsers(12345)).ToArray();

            result.Length.Should().Be(2);
            result[0].UserRef.Should().Be(_superUser.UserRef);
            result[0].ReceiveNotifications.Should().BeTrue();
            result[0].IsSuperUser.Should().BeTrue();
            result[0].DisplayName.Should().Be(_superUser.DisplayName);
            result[1].UserRef.Should().Be(_normalUser.UserRef);
            result[1].ReceiveNotifications.Should().BeFalse();
            result[1].IsSuperUser.Should().BeFalse();
            result[1].DisplayName.Should().Be(_normalUser.DisplayName);
        }

        [Test]
        public async Task UserShouldReceiveEmailIfNotSettings()
        {
            var response = new GetAccountUsersResponse();
            response.Add(_fixture.Build<User>().With(m => m.UserRef, "userRef1").Create(), null);
            response.Add(_fixture.Build<User>().With(m => m.UserRef, "userRef2").Create(), _fixture.Build<UserSetting>().With(m => m.ReceiveNotifications, false).Create());

            _mediator.Setup(m => m.Send(It.IsAny<GetAccountUsersQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(response);
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

            _mediator.Setup(m => m.Send(It.IsAny<GetAccountUsersQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(response);
            var result = (await _sut.GetAccountUsers(12345)).ToArray();

            result.Length.Should().Be(0);
        }
    }
}
