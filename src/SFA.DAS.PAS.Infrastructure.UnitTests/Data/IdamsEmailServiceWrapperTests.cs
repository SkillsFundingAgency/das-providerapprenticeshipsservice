﻿using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces.Services;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Services;

namespace SFA.DAS.PAS.Infrastructure.UnitTests.Data;

[TestFixture]
public class IdamsEmailServiceWrapperTests
{
    private IdamsEmailServiceWrapper _sut;
    private Mock<IHttpClientWrapper> _mockHttpClientWrapper;

    [SetUp]
    public void SetUp()
    {
        var config = new ProviderNotificationConfiguration
        {
            IdamsListUsersUrl = "https://url.to/listusersforarole?roleid={0}&ukprn={1}",
            ClientToken = "AbbA-Rules-4.Ever"
        };
        _mockHttpClientWrapper = new Mock<IHttpClientWrapper>();
        _sut = new IdamsEmailServiceWrapper(Mock.Of<ILogger<IdamsEmailServiceWrapper>>(), config, _mockHttpClientWrapper.Object);
    }

    [Test]
    public void ShouldThrowIfGetUsersResponseIsEmpty()
    {
        Assert.ThrowsAsync<InvalidOperationException>(() => _sut.GetEmailsAsync(10005143L, "UserRole"));
    }

    [Test]
    public void ShouldThrowIfGetUsersResponseIsInvalid()
    {
        const string mockResponse = "THIS-IS-NOT-JSON";
        _mockHttpClientWrapper.Setup(m => m.GetStringAsync(It.IsAny<string>())).ReturnsAsync(mockResponse);
        Assert.ThrowsAsync<InvalidOperationException>(() => _sut.GetEmailsAsync(10005143L, "UserRole"));
    }

    [Test]
    public async Task ShouldReturnEmailFromResult()
    {
        const string mockResponse = "{\"result\": {\"name.familyname\": [\"James\"],\"emails\": [\"abba@email.uk\"],\"name.givenname\": [\"Sally\"],\"Title\": [\"Miss\"]}}";
        _mockHttpClientWrapper.Setup(m => m.GetStringAsync(It.IsAny<string>())).ReturnsAsync(mockResponse);
        var res = await _sut.GetEmailsAsync(10005143L, "UserRole");

        res.Count.Should().Be(1);
        res[0].Should().Be("abba@email.uk");
    }

    [Test]
    public async Task ShouldReturnEmailFromResultWithMultiResult()
    {
        const string mockResponse = "{\"result\": [{\"name.familyname\": [\"James\"],\"emails\": [\"abba@email.uk\"],\"name.givenname\": [\"Sally\"],\"Title\": [\"Miss\"]}]}";
        _mockHttpClientWrapper.Setup(m => m.GetStringAsync(It.IsAny<string>())).ReturnsAsync(mockResponse);
        var res = await _sut.GetEmailsAsync(10005143L, "UserRole");

        Assert.Multiple(() =>
        {
            res.Count.Should().Be(1);
            res[0].Should().Be("abba@email.uk");
        });
    }

    [Test]
    public async Task ShouldReturnEmailsFromResult()
    {
        const string mockResponse = "{\"result\": {\"name.familyname\": [\"James\", \"Octavo\"],\"emails\": "
                                    + "[\"abba@email.uk\", \"test@email.uk\"],\"name.givenname\": [\"Sally\", \"Chris\"],\"Title\": [\"Miss\", \"Mr\"]}}";

        _mockHttpClientWrapper.Setup(m => m.GetStringAsync(It.IsAny<string>())).ReturnsAsync(mockResponse);

        var res = await _sut.GetEmailsAsync(10005143L, "UserRole");

        Assert.Multiple(() =>
        {
            res.Count.Should().Be(2);
            res[0].Should().Be("abba@email.uk");
            res[1].Should().Be("test@email.uk");
        });
    }

    [Test]
    public async Task ShouldReturnEmailsForCorrectRolesFromResult()
    {
        const string mockUserRoleResponse = "{\"result\": {\"name.familyname\": [\"James\", \"Octavo\"],\"emails\": "
                                            + "[\"abba@email.uk\", \"test@email.uk\"],\"name.givenname\": [\"Sally\", \"Chris\"],\"Title\": [\"Miss\", \"Mr\"]}}";

        const string mockViewerUserRoleResponse = "{\"result\": {\"name.familyname\": [\"Smith\", \"Pipps\"],\"emails\": "
                                                  + "[\"thomas@email.uk\", \"alice@email.uk\"],\"name.givenname\": [\"POppy\", \"Sarah\"],\"Title\": [\"Captain\", \"Prof\"]}}";

        const string mockSuperUserRoleResponse = "{\"result\": {\"name.familyname\": [\"Phelps\", \"Williams\"],\"emails\": "
                                                 + "[\"billy@email.uk\", \"charlie@email.uk\"],\"name.givenname\": [\"Edward\", \"James\"],\"Title\": [\"Sir\", \"Madam\"]}}";

        _mockHttpClientWrapper.Setup(m => m.GetStringAsync("https://url.to/listusersforarole?roleid=UserRole&ukprn=10005143")).ReturnsAsync(mockUserRoleResponse);
        _mockHttpClientWrapper.Setup(m => m.GetStringAsync("https://url.to/listusersforarole?roleid=ViewerUserRole&ukprn=10005143")).ReturnsAsync(mockViewerUserRoleResponse);
        _mockHttpClientWrapper.Setup(m => m.GetStringAsync("https://url.to/listusersforarole?roleid=SuperUserRole&ukprn=10005143")).ReturnsAsync(mockSuperUserRoleResponse);

        var res = await _sut.GetEmailsAsync(10005143L, "UserRole,ViewerUserRole");

        Assert.Multiple(() =>
        {
            res.Count.Should().Be(4);
            res[0].Should().Be("abba@email.uk");
            res[1].Should().Be("test@email.uk");
            res[2].Should().Be("thomas@email.uk");
            res[3].Should().Be("alice@email.uk");
        });

        var res2 = await _sut.GetEmailsAsync(10005143L, "SuperUserRole");

        Assert.Multiple(() =>
        {
            res2.Count.Should().Be(2);
            res2[0].Should().Be("billy@email.uk");
            res2[1].Should().Be("charlie@email.uk");
        });
    }

    [Test]
    public async Task ShouldHandleInternalErrorFromResult()
    {
        const string mockUserRoleResponse = "{\"result\": {\"name.familyname\": [\"James\", \"Octavo\"],\"emails\": "
                                            + "[\"abba@email.uk\", \"test@email.uk\"],\"name.givenname\": [\"Sally\", \"Chris\"],\"Title\": [\"Miss\", \"Mr\"]}}";

        const string mockViewerUserRoleResponse = "{\"result\": [\"internal error\"]}";

        _mockHttpClientWrapper.Setup(m => m.GetStringAsync("https://url.to/listusersforarole?roleid=UserRole&ukprn=10005143")).ReturnsAsync(mockUserRoleResponse);
        _mockHttpClientWrapper.Setup(m => m.GetStringAsync("https://url.to/listusersforarole?roleid=ViewerUserRole&ukprn=10005143")).ReturnsAsync(mockViewerUserRoleResponse);

        var res = await _sut.GetEmailsAsync(10005143L, "UserRole,ViewerUserRole");

        Assert.Multiple(() =>
        {
            res.Count.Should().Be(2);
            res[0].Should().Be("abba@email.uk");
            res[1].Should().Be("test@email.uk");
        });
    }
}