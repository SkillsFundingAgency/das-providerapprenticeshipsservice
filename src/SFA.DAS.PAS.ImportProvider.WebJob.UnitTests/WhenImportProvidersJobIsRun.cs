using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.PAS.ContractAgreements.WebJob.Interfaces;
using SFA.DAS.PAS.ContractAgreements.WebJob.ScheduledJobs;
using SFA.DAS.PAS.ImportProvider.WebJob.ScheduledJobs;
using SFA.DAS.PAS.ImportProvider.WebJob.Services;

namespace SFA.DAS.PAS.ContractAgreements.WebJob.UnitTests;

[TestFixture]
public class WhenImportProvidersJobIsRun
{
    private ImportProvidersJob _sut;
    private Mock<IImportProviderService> _importProvidersService;


    [SetUp]
    public void SetUp()
    {
        _importProvidersService = new Mock<IImportProviderService>();
        _sut = new ImportProvidersJob(_importProvidersService.Object,
            Mock.Of<ILogger<ImportProvidersJob>>());
    }

    [Test]
    public async Task ShouldCallUpdateProviderAgreementServiceOnce()
    {
        await _sut.ImportProviders(null);

        _importProvidersService.Verify(x=>x.Import(), Times.Once);
    }


    [Test]
    public async Task ShouldRethrowError()
    {
        _importProvidersService.Setup(x => x.Import())
            .ThrowsAsync(new ApplicationException("Inner exception"));
        
        var act = async () => await _sut.ImportProviders(null);

        await act.Should().ThrowAsync<ApplicationException>().WithMessage("Inner exception");
    }

    [Test]
    public async Task ShouldNotRethrowAggregateError()
    {
        _importProvidersService.Setup(x => x.Import())
            .ThrowsAsync(new AggregateException("Inner Aggregate Exception"));

        await _sut.ImportProviders(null);

        _importProvidersService.Verify(x => x.Import(), Times.Once);
    }
}