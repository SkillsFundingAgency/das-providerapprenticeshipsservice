using System;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using NUnit.Framework;
using SFA.DAS.ProviderApprenticeshipsService.Application.Commands.SendNotification;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.UnitTests.Commands.SendNotification
{
    [TestFixture]
    public class WhenHandlingTheCommand
    {
        [Test, MoqCustomisedAutoData]
        public async Task ThenItValidatesTheCommand(
            SendNotificationCommand command,
            [Frozen] Mock<IValidator<SendNotificationCommand>> mockValidator,
            SendNotificationCommandHandler sut)
        {
            await sut.Handle(command);

            mockValidator.Verify(validator => validator.Validate(command), Times.Once);
        }

        [Test, MoqCustomisedAutoData]
        public void ThenThrowsInvalidRequestExceptionIfCommandInvalid(
            SendNotificationCommand command,
            ValidationFailure validationFailure,
            ValidationResult validationResult,
            InvalidRequestException validationException,
            [Frozen] Mock<IValidator<SendNotificationCommand>> mockValidator,
            SendNotificationCommandHandler sut)
        {
            validationResult.Errors.Add(validationFailure);

            mockValidator
                .Setup(validator => validator.Validate(command))
                .Returns(validationResult);
                
            Func<Task> act = async () => { await sut.Handle(command); };

            act.ShouldThrowExactly<ValidationException>()
                .Which.Errors.ToList().Contains(validationFailure).Should().BeTrue();
        }

        [Test, MoqCustomisedAutoData]
        public async Task ThenSendsEmailToNotificationApi(
            SendNotificationCommand command,
            [Frozen] Mock<IBackgroundNotificationService> mockNotificationsApi,
            SendNotificationCommandHandler sut)
        {
            await sut.Handle(command);

            mockNotificationsApi.Verify(api => api.SendEmail(command.Email), Times.Once);
        }
    }
}