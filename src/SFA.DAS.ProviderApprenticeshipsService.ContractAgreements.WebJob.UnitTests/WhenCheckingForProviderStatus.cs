using FluentAssertions;

using Moq;

using NUnit.Framework;

using SFA.DAS.NLog.Logger;
using SFA.DAS.ProviderApprenticeshipsService.ContractAgreements.WebJob.ContractFeed;
using SFA.DAS.ProviderApprenticeshipsService.ContractAgreements.WebJob.UnitTests.MockClasses;
using SFA.DAS.ProviderApprenticeshipsService.Domain;

namespace SFA.DAS.ProviderApprenticeshipsService.ContractAgreements.WebJob.UnitTests
{
    [TestFixture]
    public class WhenCheckingForProviderStatus
    {
        [Test]
        public void GettingAgreementStatus()
        {
            var repository = new InMemoryProviderAgreementStatusRepository(Mock.Of<ILog>());
            repository.AddContractEvent(new ContractFeedEvent { ProviderId = 1234565, Status = "Auto-Withdrawn" });
            repository.AddContractEvent(new ContractFeedEvent { ProviderId = 1234566, Status = "Withdrawn By Provider" });
            repository.AddContractEvent(new ContractFeedEvent { ProviderId = 1234567, Status = "Published To Provider" });
            repository.AddContractEvent(new ContractFeedEvent { ProviderId = 1234560, Status = "Approved" });
            var sut = new ProviderAgreementStatusService(Mock.Of<IContractDataProvider>(), repository, Mock.Of<ILog>());

            sut.GetProviderAgreementStatus(1234565)
                .Should().Be(ProviderAgreementStatus.NotAgreed);
            sut.GetProviderAgreementStatus(1234566)
                .Should().Be(ProviderAgreementStatus.NotAgreed);
            sut.GetProviderAgreementStatus(1234567)
                .Should().Be(ProviderAgreementStatus.NotAgreed);
            sut.GetProviderAgreementStatus(12)
                .Should().Be(ProviderAgreementStatus.NotAgreed);
            sut.GetProviderAgreementStatus(1234560)
                .Should().Be(ProviderAgreementStatus.Agreed);
        }
    }
}
