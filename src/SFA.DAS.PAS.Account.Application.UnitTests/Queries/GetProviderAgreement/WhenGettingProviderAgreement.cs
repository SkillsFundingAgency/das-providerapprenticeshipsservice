using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.PAS.Account.Application.Queries.GetProviderAgreement;
using SFA.DAS.ProviderApprenticeshipsService.Domain.ContractFeed;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Enums;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces.Data;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;

namespace SFA.DAS.PAS.Account.Application.UnitTests.Queries.GetProviderAgreement
{
    [TestFixture]
    public class WhenGettingProviderAgreement
    {
        private GetProviderAgreementQueryHandler _sut;
        private Mock<IProviderAgreementStatusRepository> _mockProviderAgreementStatusRepository;
        const long ProviderId = 10;

        [SetUp]
        public void SetUp()
        {
            _mockProviderAgreementStatusRepository = new Mock<IProviderAgreementStatusRepository>();   
        }

        [Test]
        public async Task IfCheckForContractAgreementsFalse_ProviderAgreementStatusIsAgreed()
        {
            // Arrange
            var config = new PasAccountApiConfiguration
            {
                CheckForContractAgreements = false
            };
            var query = new GetProviderAgreementQueryRequest { ProviderId = ProviderId };
            _sut = new GetProviderAgreementQueryHandler(_mockProviderAgreementStatusRepository.Object, config);

            // Act
            var result = await _sut.Handle(query, new CancellationToken());

            // Assert
            result.HasAgreement.Should().Be(ProviderAgreementStatus.Agreed);
        }

        [Test]
        public async Task IfNoStatusApproved_ProviderAgreementStatusIsNotAgreed()
        {
            // Arrange
            var config = new PasAccountApiConfiguration
            {
                CheckForContractAgreements = true
            };
            var query = new GetProviderAgreementQueryRequest { ProviderId = ProviderId };
            _sut = new GetProviderAgreementQueryHandler(_mockProviderAgreementStatusRepository.Object, config);
            _mockProviderAgreementStatusRepository.Setup(m => m.GetContractEvents(ProviderId))
                .ReturnsAsync(new List<ContractFeedEvent> { new ContractFeedEvent
                {
                    Status = ""
                }
                });

            // Act
            var result = await _sut.Handle(query, new CancellationToken());

            // Assert
            result.HasAgreement.Should().Be(ProviderAgreementStatus.NotAgreed);
        }

        [Test]
        public async Task IfStatusApproved_ProviderAgreementStatusIsAgreed()
        {
            // Arrange
            var config = new PasAccountApiConfiguration
            {
                CheckForContractAgreements = true
            };
            var query = new GetProviderAgreementQueryRequest { ProviderId = ProviderId };
            _sut = new GetProviderAgreementQueryHandler(_mockProviderAgreementStatusRepository.Object, config);
            _mockProviderAgreementStatusRepository.Setup(m => m.GetContractEvents(ProviderId))
                .ReturnsAsync(new List<ContractFeedEvent> { new ContractFeedEvent
                {
                    Status = "approved"
                }
                });

            // Act
            var result = await _sut.Handle(query, new CancellationToken());

            // Assert
            result.HasAgreement.Should().Be(ProviderAgreementStatus.Agreed);
        }
    }
}

