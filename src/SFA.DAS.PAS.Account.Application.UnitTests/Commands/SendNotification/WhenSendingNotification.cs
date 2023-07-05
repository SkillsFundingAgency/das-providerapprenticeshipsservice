using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Notifications.Api.Types;
using SFA.DAS.PAS.Account.Application.Commands.SendNotification;
using SFA.DAS.PAS.Application.UnitTests.Extensions;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces.Services;
using ValidationException = System.ComponentModel.DataAnnotations.ValidationException;

namespace SFA.DAS.PAS.Account.Application.UnitTests.Commands.SendNotification;

[TestFixture]
public class WhenSendingNotification
{
    private SendNotificationCommandHandler _sut;
    private Mock<IValidator<SendNotificationCommand>> _mockValidator;
    private Mock<ILogger<SendNotificationCommandHandler>> _mockLogger;
    private Mock<IBackgroundNotificationService> _mockBackgroundNotificationService;

    [SetUp]
    public void SetUp()
    {
        _mockBackgroundNotificationService = new Mock<IBackgroundNotificationService>();
        _mockValidator = new Mock<IValidator<SendNotificationCommand>>();
        _mockLogger = new Mock<ILogger<SendNotificationCommandHandler>>();
        _sut = new SendNotificationCommandHandler(_mockValidator.Object, _mockBackgroundNotificationService.Object, _mockLogger.Object);
    }

    [Test]
    public Task IfSendNotificationCommandMessageIsEmpty_ValidationExceptionThrown()
    {
        // Arrange
        var command = new SendNotificationCommand(null);
        IEnumerable<ValidationFailure> failures = new List<ValidationFailure>
        {
            new ValidationFailure
            {
                ErrorMessage = "empty",
                ErrorCode = "40023"
            }
        };
        _mockValidator.Setup(m => m.Validate(command))
            .Returns(new ValidationResult(failures));

        // Act and Assert
        Assert.That(async () => await _sut.Handle(command, new CancellationToken()), Throws.TypeOf<ValidationException>());

        return Task.CompletedTask;
    }

    [Test]
    public async Task IfBackgroundNotificationServiceFails_ErrorCaughtAndMessageLogged()
    {
        // Arrange
        const string testEmailAddress = "test@email.com";
        var testEmail = new Email
        {
            TemplateId = "1",
            ReplyToAddress = "test@noreply.com",
            Subject = "test",
            RecipientsAddress = testEmailAddress
        };
        var command = new SendNotificationCommand(testEmail);
        _mockValidator.Setup(m => m.Validate(command))
            .Returns(new ValidationResult());
        _mockBackgroundNotificationService.Setup(m => m.SendEmail(testEmail))
            .ThrowsAsync(new Exception("Error"));

        // Act
        await _sut.Handle(command, new CancellationToken());

        // Assert
        _mockLogger.VerifyLogging($"Error calling Notification Api. Recipient: {testEmail.RecipientsAddress}", LogLevel.Error, Times.Once());
    }

    [Test]
    public async Task IfBackgroundNotificationServiceSucceeds_ReturnsTaskCompleted()
    {
        // Arrange
        const string testEmailAddress = "test@email.com";
        var testEmail = new Email
        {
            TemplateId = "1",
            ReplyToAddress = "test@noreply.com",
            Subject = "test",   
            RecipientsAddress = testEmailAddress
        };
        var command = new SendNotificationCommand (testEmail);
        _mockValidator.Setup(m => m.Validate(command))
            .Returns(new ValidationResult());
        _mockBackgroundNotificationService.Setup(m => m.SendEmail(testEmail))
            .Returns(Task.CompletedTask);

        // Act
        await _sut.Handle(command, new CancellationToken());

        // Assert
        _mockLogger.VerifyLogging("Email sent to recipient.", LogLevel.Information, Times.Once());
    }
}