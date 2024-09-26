using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.PAS.ContractAgreements.WebJob.Interfaces;
using SFA.DAS.PAS.ContractAgreements.WebJob.ScheduledJobs;

namespace SFA.DAS.PAS.ContractAgreements.WebJob.UnitTests;

[TestFixture]
public class WhenUpdateAgreementStatusJobIsRun
{
    private UpdateAgreementStatusJob _sut;
    private Mock<IProviderAgreementStatusService> _providerAgreementStatusService;


    [SetUp]
    public void SetUp()
    {
        _providerAgreementStatusService = new Mock<IProviderAgreementStatusService>();
        _sut = new UpdateAgreementStatusJob(_providerAgreementStatusService.Object,
            Mock.Of<ILogger<UpdateAgreementStatusJob>>());
    }

    [Test]
    public async Task ShouldCallUpdateProviderAgreementServiceOnce()
    {
        await _sut.UpdateAgreementStatus(null);

        _providerAgreementStatusService.Verify(x=>x.UpdateProviderAgreementStatuses(), Times.Once);
    }


    [Test]
    public async Task ShouldRethrowError()
    {
        _providerAgreementStatusService.Setup(x => x.UpdateProviderAgreementStatuses())
            .ThrowsAsync(new ApplicationException("Inner exception"));
        
        var act = async () => await _sut.UpdateAgreementStatus(null);

        await act.Should().ThrowAsync<ApplicationException>().WithMessage("Inner exception");
    }

    [Test]
    public async Task ShouldNotRethrowAggregateError()
    {
        _providerAgreementStatusService.Setup(x => x.UpdateProviderAgreementStatuses())
            .ThrowsAsync(new AggregateException("Inner Aggregate Exception"));

        await _sut.UpdateAgreementStatus(null);

        _providerAgreementStatusService.Verify(x => x.UpdateProviderAgreementStatuses(), Times.Once);
    }
}