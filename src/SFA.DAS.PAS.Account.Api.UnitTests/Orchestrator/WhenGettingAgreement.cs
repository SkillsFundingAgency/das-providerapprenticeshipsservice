using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.PAS.Account.Api.Orchestrator;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetProviderAgreement;
using SFA.DAS.ProviderApprenticeshipsService.Domain;
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
            _sut = new AccountOrchestrator(_mediator.Object, Mock.Of<IProviderCommitmentsLogger>());
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

            result.Status.Should().Be(expectedStatus.ToString());
        }
    }
}
