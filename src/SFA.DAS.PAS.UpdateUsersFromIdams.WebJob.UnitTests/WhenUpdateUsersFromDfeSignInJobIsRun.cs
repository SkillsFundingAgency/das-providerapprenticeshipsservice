using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.PAS.UpdateUsersFromIdams.WebJob.ScheduledJobs;
using SFA.DAS.PAS.UpdateUsersFromIdams.WebJob.Services;

namespace SFA.DAS.PAS.UpdateUsersFromIdams.WebJob.UnitTests;

[TestFixture]
public class WhenUpdateUsersFromDfeSignInJobIsRun
{
    private UpdateUsersFromDfeSignInJob _sut;
    private Mock<IIdamsSyncService> _updateUsersService;


    [SetUp]
    public void SetUp()
    {
        _updateUsersService = new Mock<IIdamsSyncService>();
        _sut = new UpdateUsersFromDfeSignInJob(_updateUsersService.Object,
            Mock.Of<ILogger<UpdateUsersFromDfeSignInJob>>());
    }

    [Test]
    public async Task ShouldCallUpdateProviderAgreementServiceOnce()
    {
        await _sut.UpdateUsers(null);

        _updateUsersService.Verify(x=>x.SyncUsers(), Times.Once);
    }


    [Test]
    public async Task ShouldRethrowError()
    {
        _updateUsersService.Setup(x => x.SyncUsers())
            .ThrowsAsync(new ApplicationException("Inner exception"));
        
        var act = async () => await _sut.UpdateUsers(null);

        await act.Should().ThrowAsync<ApplicationException>().WithMessage("Inner exception");
    }

    [Test]
    public async Task ShouldNotRethrowAggregateError()
    {
        _updateUsersService.Setup(x => x.SyncUsers())
            .ThrowsAsync(new AggregateException("Inner Aggregate Exception"));

        await _sut.UpdateUsers(null);

        _updateUsersService.Verify(x => x.SyncUsers(), Times.Once);
    }
}