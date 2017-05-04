using System;
using System.Threading.Tasks;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.Apprenticeship.Types;
using SFA.DAS.ProviderApprenticeshipsService.Application.Commands.UndoApprenticeshipUpdate;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.UnitTests.Commands.UndoApprenticeshipUpdate
{
    [TestFixture]
    public class WhenUndoingApprenticeshipUpdate
    {
        private UndoApprenticeshipUpdateCommandHandler _handler;
        private Mock<IProviderCommitmentsApi> _commitmentsApi;
        private Mock<AbstractValidator<UndoApprenticeshipUpdateCommand>> _validator;

        [SetUp]
        public void Arrange()
        {
            _validator = new Mock<AbstractValidator<UndoApprenticeshipUpdateCommand>>();
            _validator.Setup(x => x.Validate(It.IsAny<UndoApprenticeshipUpdateCommand>()))
                .Returns(() => new ValidationResult());

            _commitmentsApi = new Mock<IProviderCommitmentsApi>();
            _commitmentsApi.Setup(x => x.PatchApprenticeshipUpdate(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<ApprenticeshipUpdateSubmission>()))
                .Returns(() => Task.FromResult(new Unit()));

            _handler = new UndoApprenticeshipUpdateCommandHandler(_validator.Object, _commitmentsApi.Object);
        }

        [Test]
        public async Task ThenTheCommandIsValidated()
        {
            //Arrange
            var command = new UndoApprenticeshipUpdateCommand();

            //Act
            await _handler.Handle(command);

            //Assert
            _validator.Verify(x => x.Validate(It.IsAny<UndoApprenticeshipUpdateCommand>()), Times.Once);
        }

        [Test]
        public async Task ThenIfTheRequestIsNotValidThenAnExceptionIsThrown()
        {
            //Arrange
            _validator.Setup(x => x.Validate(It.IsAny<UndoApprenticeshipUpdateCommand>()))
                .Returns(() => new ValidationResult
                {
                    Errors = { new ValidationFailure("Test", "Error") }
                });

            var command = new UndoApprenticeshipUpdateCommand();

            //Act & Assert
            Func<Task> act = async () => { await _handler.Handle(command); };
            act.ShouldThrow<ValidationException>();
        }

        [Test]
        public async Task ThenTheCommitmentsApiIsCalledToSubmitTheUpdate()
        {
            //Arrange
            var command = new UndoApprenticeshipUpdateCommand
            {
                ApprenticeshipId = 1,
                ProviderId = 2,
                UserId = "tester",
                UserDisplayName = "Bob",
                UserEmailAddress = "test@email.com"
            };

            //Act
            await _handler.Handle(command);

            //Assert
            _commitmentsApi.Verify(x => x.PatchApprenticeshipUpdate(
                    command.ProviderId,
                    command.ApprenticeshipId,
                    It.Is<ApprenticeshipUpdateSubmission>(
                        s =>
                            s.UpdateStatus == ApprenticeshipUpdateStatus.Deleted && s.UserId == command.UserId && s.LastUpdatedByInfo.EmailAddress == command.UserEmailAddress &&
                            s.LastUpdatedByInfo.Name == command.UserDisplayName)),
                Times.Once);
        }
    }
}
