using System;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetApprenticeship;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators;
using SFA.DAS.ProviderApprenticeshipsService.Web.Validation;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.Mappers;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Orchestrators.Commitments
{
    [TestFixture]
    public sealed class WhenGettingDeleteApprenticeshipConfirmation : ApprenticeshipValidationTestBase
    {
        protected override void SetUp()
        {
            _mockHashingService.Setup(m => m.DecodeValue("ABBA99")).Returns(123L);
            _mockHashingService.Setup(m => m.DecodeValue("ABBA66")).Returns(321L);
        }

        [Test]
        public async Task ShouldCallMediatorToGetApprenticeship()
        {
            GetApprenticeshipQueryRequest arg = null;

            var dummyResponse = new GetApprenticeshipQueryResponse
            {
                Apprenticeship = new DAS.Commitments.Api.Types.Apprenticeship.Apprenticeship { FirstName = "Bob", LastName = "Tester", DateOfBirth = new DateTime(1999, 1, 2) }
            };

            _mockMediator.Setup(x => x.SendAsync(It.IsAny<GetApprenticeshipQueryRequest>()))
                .ReturnsAsync(dummyResponse).Callback<GetApprenticeshipQueryRequest>(x => arg = x);

            await _orchestrator.GetDeleteConfirmationModel(123L, "ABBA99", "ABBA66");

            _mockMediator.Verify(x => x.SendAsync(It.IsAny<GetApprenticeshipQueryRequest>()), Times.Once);
            arg.ProviderId.Should().Be(123);
            arg.ApprenticeshipId.Should().Be(321);
        }

        [Test]
        public async Task ShouldReturnApprenticeshipName()
        {
            GetApprenticeshipQueryRequest arg = null;

            var dummyResponse = new GetApprenticeshipQueryResponse
            {
                Apprenticeship = new DAS.Commitments.Api.Types.Apprenticeship.Apprenticeship { FirstName = "Bob", LastName = "Tester", DateOfBirth = new DateTime(1999, 1, 2) }
            };

            _mockMediator.Setup(x => x.SendAsync(It.IsAny<GetApprenticeshipQueryRequest>()))
                .ReturnsAsync(dummyResponse).Callback<GetApprenticeshipQueryRequest>(x => arg = x);

            var viewModel = await _orchestrator.GetDeleteConfirmationModel(123L, "ABBA99", "ABBA66");

            viewModel.ApprenticeshipName.Should().Be("Bob Tester");
        }

        public async Task ShouldReturnJustLastNameIfFirstNameNotSpecified()
        {
            GetApprenticeshipQueryRequest arg = null;

            var dummyResponse = new GetApprenticeshipQueryResponse
            {
                Apprenticeship = new DAS.Commitments.Api.Types.Apprenticeship.Apprenticeship { LastName = "Tester", DateOfBirth = new DateTime(1999, 1, 2) }
            };

            _mockMediator.Setup(x => x.SendAsync(It.IsAny<GetApprenticeshipQueryRequest>()))
                .ReturnsAsync(dummyResponse).Callback<GetApprenticeshipQueryRequest>(x => arg = x);

            var viewModel = await _orchestrator.GetDeleteConfirmationModel(123L, "ABBA99", "ABBA66");

            viewModel.ApprenticeshipName.Should().Be("Tester");
        }

        [Test]
        public async Task ShouldReturnDateOfBirthWhenPopulated()
        {
            GetApprenticeshipQueryRequest arg = null;

            var dummyResponse = new GetApprenticeshipQueryResponse
            {
                Apprenticeship = new DAS.Commitments.Api.Types.Apprenticeship.Apprenticeship { FirstName = "Bob", LastName = "Tester", DateOfBirth = new DateTime(1999, 1, 2) }
            };

            _mockMediator.Setup(x => x.SendAsync(It.IsAny<GetApprenticeshipQueryRequest>()))
                .ReturnsAsync(dummyResponse).Callback<GetApprenticeshipQueryRequest>(x => arg = x);

            var viewModel = await _orchestrator.GetDeleteConfirmationModel(123L, "ABBA99", "ABBA66");

            viewModel.DateOfBirth.Should().Be("2 Jan 1999");
        }

        [Test]
        public async Task ShouldReturnEmptyStringForDateOfBirthWhenNotPopulated()
        {
            GetApprenticeshipQueryRequest arg = null;

            var dummyResponse = new GetApprenticeshipQueryResponse
            {
                Apprenticeship = new DAS.Commitments.Api.Types.Apprenticeship.Apprenticeship { FirstName = "Bob", LastName = "Tester", DateOfBirth = null }
            };

            _mockMediator.Setup(x => x.SendAsync(It.IsAny<GetApprenticeshipQueryRequest>()))
                .ReturnsAsync(dummyResponse).Callback<GetApprenticeshipQueryRequest>(x => arg = x);

            var viewModel = await _orchestrator.GetDeleteConfirmationModel(123L, "ABBA99", "ABBA66");

            viewModel.DateOfBirth.Should().BeEmpty();
        }
    }
}
