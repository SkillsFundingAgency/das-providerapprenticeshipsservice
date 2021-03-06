﻿using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using FluentValidation;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Notifications.Api.Client;
using SFA.DAS.Notifications.Api.Types;
using SFA.DAS.ProviderApprenticeshipsService.Application.Commands.SendNotification;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.UnitTests.Commands.SendNotification
{
    [TestFixture]
    public sealed class WhenValidatingCommand
    {
        private IRequestHandler<SendNotificationCommand> _handler;
        private SendNotificationCommand _validCommand;

        [SetUp]
        public void Setup()
        {
            _validCommand = new SendNotificationCommand
            {
                Email = new Email
                {
                    RecipientsAddress = "test@test.com",
                    ReplyToAddress = "noreply@test.com",
                    Subject = "Test Subject",
                    TemplateId = "ABC123",
                }
            };

            _handler = new SendNotificationCommandHandler(new SendNotificationCommandValidator(), Mock.Of<IBackgroundNotificationService>(), Mock.Of<ILog>());
        }

        [Test]
        public void ShouldThrowExceptionIfEmailObjectNull()
        {
            _validCommand.Email = null;

            Func<Task> act = async () => await _handler.Handle(_validCommand, new CancellationToken());

            act.ShouldThrow<ValidationException>().Which.Message.Contains("Email");
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("  ")]
        public void ShouldThrowExceptionIfRecipientAddressNotSet(string value)
        {
            _validCommand.Email.RecipientsAddress = value;

            Func<Task> act = async () => await _handler.Handle(_validCommand, new CancellationToken());

            act.ShouldThrow<ValidationException>().Which.Message.Contains("Recipients Address");
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("  ")]
        public void ShouldThrowExceptionIfReplyToAddressNotSet(string value)
        {
            _validCommand.Email.ReplyToAddress = value;

            Func<Task> act = async () => await _handler.Handle(_validCommand, new CancellationToken());

            act.ShouldThrow<ValidationException>().Which.Message.Contains("Recipients Address");
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("  ")]
        public void ShouldThrowExceptionIfSubjectNotSet(string value)
        {
            _validCommand.Email.Subject = value;

            Func<Task> act = async () => await _handler.Handle(_validCommand, new CancellationToken());

            act.ShouldThrow<ValidationException>().Which.Message.Contains("Subject");
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("  ")]
        public void ShouldThrowExceptionIfTemplateIdNotSet(string value)
        {
            _validCommand.Email.TemplateId = value;

            Func<Task> act = async () => await _handler.Handle(_validCommand, new CancellationToken());

            act.ShouldThrow<ValidationException>().Which.Message.Contains("Template Id");
        }
    }
}
