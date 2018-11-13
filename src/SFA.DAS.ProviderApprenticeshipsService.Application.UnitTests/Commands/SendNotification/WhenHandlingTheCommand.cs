using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Notifications.Api.Types;
using SFA.DAS.ProviderApprenticeshipsService.Application.Commands.SendNotification;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.UnitTests.Commands.SendNotification
{
    [TestFixture]
    public class WhenHandlingTheCommand
    {
        private Mock<IValidator<SendNotificationCommand>> _mockValidator;
        IRequestHandler<SendNotificationCommand> _sut;
        private Mock<IBackgroundNotificationService> _backgroundNotificationService;

        [SetUp]
        public void Arrange()
        {
            _mockValidator = new Mock<IValidator<SendNotificationCommand>>();
            _mockValidator.Setup(x => x.Validate(It.IsAny<SendNotificationCommand>()))
                .Returns(() => new ValidationResult());

            _backgroundNotificationService = new Mock<IBackgroundNotificationService>();
            _sut = new SendNotificationCommandHandler(_mockValidator.Object, _backgroundNotificationService.Object, Mock.Of<ILog>());

            _backgroundNotificationService.Setup(api => api.SendEmail(It.IsAny<Email>())).Returns(Task.CompletedTask);
        }

        [Test, MoqCustomisedAutoData]
        public async Task ThenItValidatesTheCommand(SendNotificationCommand command)
        {
            await _sut.Handle(command, new CancellationToken());

            _mockValidator.Verify(validator => validator.Validate(command), Times.Once);
        }

        [Test, MoqCustomisedAutoData]
        public void ThenThrowsInvalidRequestExceptionIfCommandInvalid(
            SendNotificationCommand command,
            ValidationFailure validationFailure,
            ValidationResult validationResult,
            InvalidRequestException validationException)
        {
            validationResult.Errors.Add(validationFailure);

            _mockValidator
                .Setup(validator => validator.Validate(command))
                .Returns(validationResult);
                
            Func<Task> act = async () => { await _sut.Handle(command, new CancellationToken()); };

            act.ShouldThrowExactly<ValidationException>()
                .Which.Errors.ToList().Contains(validationFailure).Should().BeTrue();
        }

        [Test, MoqCustomisedAutoData]
        public async Task ThenSendsEmailToNotificationApi(SendNotificationCommand command)
        {
            await _sut.Handle(command, new CancellationToken());

            _backgroundNotificationService.Verify(api => api.SendEmail(command.Email), Times.Once);
        }
    }
}