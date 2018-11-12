using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.ProviderApprenticeshipsService.Application.Commands.UpdateUserNotificationSettings;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.UnitTests.Queries.GetUserNotificationSettings
{
    [TestFixture]
    public class WhenUpdatingUserSettings
    {
        private IRequestHandler<UpdateUserNotificationSettingsCommand> _sut;
        private Mock<IUserSettingsRepository> _mockSettingsRepo;
        private Mock<IValidator<UpdateUserNotificationSettingsCommand>> _mockValidator;

        private const string UserRef = "userRef";

        private UpdateUserNotificationSettingsCommand _command;

        [SetUp]
        public void SetUp()
        {
            _mockSettingsRepo = new Mock<IUserSettingsRepository>();
            _mockValidator = new Mock<IValidator<UpdateUserNotificationSettingsCommand>>();

            _sut = new UpdateUserNotificationSettingsHandler(_mockSettingsRepo.Object, _mockValidator.Object, Mock.Of<IProviderCommitmentsLogger>());

            _command = new UpdateUserNotificationSettingsCommand { UserRef = UserRef, ReceiveNotifications = true };
        }

        [Test]
        public async Task ThenGettingUsersSettings()
        {
            _mockValidator.Setup(m => m.Validate(_command)) .Returns(new ValidationResult() );

            await _sut.Handle(_command, new CancellationToken());

            _mockSettingsRepo.Verify(m => m.GetUserSetting(It.IsAny<string>()), Times.Never);
            _mockSettingsRepo.Verify(m => m.UpdateUserSettings(_command.UserRef, _command.ReceiveNotifications), Times.Once);
        }

        [Test]
        public void ThenCreatingSettingsIfNoSettingsFoundGettingUsersSettings()
        {
            _mockValidator.Setup(m => m.Validate(_command))
                .Returns(new ValidationResult { Errors = { new ValidationFailure("Error", "Error message") } });

            Func<Task> act = async () => await _sut.Handle(_command, new CancellationToken());
            act.ShouldThrow<InvalidRequestException>();

            _mockSettingsRepo.Verify(m => m.GetUserSetting(It.IsAny<string>()), Times.Never);
            _mockSettingsRepo.Verify(m => m.UpdateUserSettings(It.IsAny<string>(), It.IsAny<bool>()), Times.Never);
        }
    }
}
