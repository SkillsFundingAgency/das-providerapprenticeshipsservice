using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.PAS.Account.Api.Orchestrator;
using SFA.DAS.PAS.Account.Application.Queries.GetProviderAgreement;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Enums;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;

namespace SFA.DAS.PAS.Account.Api.UnitTests.Orchestrator
{
    [TestFixture]
    public class WhenGettingAgreement
    {
        private const long Ukprn = 12345678;

        private AccountOrchestrator _sut;
        private Mock<IMediator> _mediator;

        [SetUp]
        public void SetUp()
        {
            _mediator = new Mock<IMediator>();
            _sut = new AccountOrchestrator(_mediator.Object);
        }

        [TestCase(ProviderAgreementStatus.Agreed)]
        [TestCase(ProviderAgreementStatus.NotAgreed)]
        public async Task ShouldReturnAgreement(ProviderAgreementStatus expectedStatus)
        {
            var response = new GetProviderAgreementQueryResponse
            {
                HasAgreement = expectedStatus
            };

            _mediator.Setup(m => m.Send(
                It.Is<GetProviderAgreementQueryRequest>(r => r.ProviderId == Ukprn),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            var result = await _sut.GetAgreement(Ukprn);

            var apiAgreementStatus = (Types.ProviderAgreementStatus) Enum.Parse(typeof(Types.ProviderAgreementStatus), expectedStatus.ToString());

            result.Status.Should().Be(apiAgreementStatus);
        }
    }
}
