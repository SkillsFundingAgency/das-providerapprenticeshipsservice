using AutoFixture;
using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.PAS.Account.Api.Orchestrator;
using SFA.DAS.PAS.Account.Application.Queries.GetAccountUsers;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Enums;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models.UserProfile;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models.UserSetting;

namespace SFA.DAS.PAS.Account.Api.UnitTests.Orchestrator;

[TestFixture]
public class WhenGettingAccountUsers
{
    private AccountOrchestrator _sut;
    private Mock<IMediator> _mediator;
    private Fixture _fixture;
    private User _user;

    [SetUp]
    public void SetUp()
    {
        _fixture = new Fixture();
        _mediator = new Mock<IMediator>();
        _sut = new AccountOrchestrator(_mediator.Object);
        _user = _fixture.Build<User>().Create();
    }

    [Test]
    public async Task ShouldReturnUsers()
    {
        var response = new GetAccountUsersResponse();

        response.Add(_user, _fixture.Build<UserSetting>().With(m => m.ReceiveNotifications, true).Create());

        _mediator.Setup(m => m.Send(It.IsAny<GetAccountUsersQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(response);
        var result = (await _sut.GetAccountUsers(12345)).ToArray();

        result.Length.Should().Be(1);
        result[0].UserRef.Should().Be(_user.UserRef);
        result[0].ReceiveNotifications.Should().BeTrue();
        result[0].DisplayName.Should().Be(_user.DisplayName);
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