using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using FluentAssertions;

using Moq;

using NUnit.Framework;

using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetAgreement;
using SFA.DAS.ProviderApprenticeshipsService.Domain;
using SFA.DAS.ProviderApprenticeshipsService.Domain.ContractFeed;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Data;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.UnitTests.Queries.GetAgreement
{
    [TestFixture]
    public class WhenIGetAgreement
    {
        private Mock<IAgreementStatusQueryRepository> _agreementRepository;
        private GetProviderAgreementQueryHandler _handler;

        [SetUp]
        public void SetUp()
        {
            var config = new ProviderApprenticeshipsServiceConfiguration { CheckForContractAgreements = true };
            _agreementRepository = new Mock<IAgreementStatusQueryRepository>();
            _handler = new GetProviderAgreementQueryHandler(_agreementRepository.Object, config);
        }

        [Test]
        public async Task TestFilterNoRecord()
        {
            _agreementRepository.Setup(m => m.GetContractEvents(It.IsAny<long>()))
                .Returns(Task.Run(() => new List<ContractFeedEvent>().AsEnumerable()));

            var result = await _handler.Handle(new GetProviderAgreementQueryRequest { ProviderId = 1234567 });

            result.HasAgreement.Should().Be(ProviderAgreementStatus.NotAgreed);
        }

        [Test]
        public async Task ShouldBeAgreedIfOneRecoredIsApprooved()
        {
            var returnList = new List<ContractFeedEvent>
                                 {
                                     new ContractFeedEvent { ProviderId = 1234567, Status = "Approved" }
                                 };

            _agreementRepository.Setup(m => m.GetContractEvents(It.IsAny<long>()))
                .Returns(Task.Run(() =>  returnList.AsEnumerable()));

            var result = await _handler.Handle(new GetProviderAgreementQueryRequest { ProviderId = 1234567 });

            result.HasAgreement.Should().Be(ProviderAgreementStatus.Agreed);
        }

        [Test]
        public async Task ShouldNotBeAgreedIfNoRecoredApproved()
        {
            var returnList = new List<ContractFeedEvent>
                                 {
                                     new ContractFeedEvent { ProviderId = 1234567, Status = "Not Approved" }
                                 };

            _agreementRepository.Setup(m => m.GetContractEvents(It.IsAny<long>()))
                .Returns(Task.Run(() => returnList.AsEnumerable()));

            var result = await _handler.Handle(new GetProviderAgreementQueryRequest { ProviderId = 1234567 });

            result.HasAgreement.Should().Be(ProviderAgreementStatus.NotAgreed);
        }
    }
}
