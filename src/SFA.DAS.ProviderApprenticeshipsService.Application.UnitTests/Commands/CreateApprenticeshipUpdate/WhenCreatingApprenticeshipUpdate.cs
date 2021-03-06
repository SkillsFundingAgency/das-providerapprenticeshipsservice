﻿using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.ProviderApprenticeshipsService.Application.Commands.CreateApprenticeshipUpdate;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.UnitTests.Commands.CreateApprenticeshipUpdate
{
    [TestFixture]
    public class WhenCreatingApprenticeshipUpdate
    {
        private IRequestHandler<CreateApprenticeshipUpdateCommand> _handler;
        private Mock<IProviderCommitmentsApi> _commitmentsApi;
        private Mock<IValidator<CreateApprenticeshipUpdateCommand>> _validator;

        [SetUp]
        public void Arrange()
        {
            _commitmentsApi = new Mock<IProviderCommitmentsApi>();
            _commitmentsApi.Setup(x => x.CreateApprenticeshipUpdate(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<ApprenticeshipUpdateRequest>()))
                .Returns(() => Task.FromResult(new Unit()));

            _validator = new Mock<IValidator<CreateApprenticeshipUpdateCommand>>();
            _validator.Setup(x => x.Validate(It.IsAny<CreateApprenticeshipUpdateCommand>()))
                .Returns(new ValidationResult());

            _handler = new CreateApprenticeshipUpdateCommandHandler(_commitmentsApi.Object, _validator.Object);
        }

        [Test]
        public async Task ThenTheCommandIsValidated()
        {
            //Arrange
            var command = new CreateApprenticeshipUpdateCommand
            {
                ApprenticeshipUpdate = new ApprenticeshipUpdate(),
                ProviderId = 1,
                UserId = "Tester"
            };

            //Act
            await _handler.Handle(command, new CancellationToken());

            //Assert
            _validator.Verify(x=> x.Validate(It.IsAny<CreateApprenticeshipUpdateCommand>()), Times.Once);
        }

        [Test]
        public async Task ThenTheCommitmentsApiIsCalledToCreateTheUpdate()
        {
            //Arrange
            var command = new CreateApprenticeshipUpdateCommand
            {
                ApprenticeshipUpdate = new ApprenticeshipUpdate { ApprenticeshipId = 123 },
                ProviderId = 1,
                UserId = "Tester",
                UserEmailAddress = "test@email.com",
                UserDisplayName = "Bob"
            };

            //Act
            await _handler.Handle(command, new CancellationToken());

            //Assert
            _commitmentsApi.Verify(
                x =>
                    x.CreateApprenticeshipUpdate(command.ProviderId, command.ApprenticeshipUpdate.ApprenticeshipId,
                        It.Is<ApprenticeshipUpdateRequest>(
                            r =>
                                r.UserId == command.UserId && r.ApprenticeshipUpdate == command.ApprenticeshipUpdate && r.LastUpdatedByInfo.EmailAddress == command.UserEmailAddress &&
                                r.LastUpdatedByInfo.Name == command.UserDisplayName)), Times.Once);
        }
    }
}
