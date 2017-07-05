using System.Collections.Generic;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.ProviderApprenticeshipsService.Application.Commands.UpsertRegisteredUser;
using SFA.DAS.ProviderApprenticeshipsService.Domain;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.UnitTests.Commands.UpsertRegisteredUser
{
    [TestFixture]
    public class WhenUpsertingRegisteredUser
    {
        private UpsertRegisteredUserCommandHandler _handler;
        private Mock<UpsertRegisteredUserCommandValidator> _validator;
        private Mock<IUserRepository> _repository;

        [SetUp]
        public async Task Arrange()
        {
            _validator = new Mock<UpsertRegisteredUserCommandValidator>();
            _validator.Setup(x => x.Validate(It.IsAny<UpsertRegisteredUserCommand>()))
                .Returns(new ValidationResult());

            _repository = new Mock<IUserRepository>();
            _repository.Setup(x => x.Upsert(It.IsAny<User>()))
                .Returns(() => Task.FromResult(new Unit()));

            _handler = new UpsertRegisteredUserCommandHandler(_validator.Object, _repository.Object);
        }

        [Test]
        public async Task ThenTheCommandIsValidated()
        {
            //Arrange
            var command = new UpsertRegisteredUserCommand();

            //Act
            await _handler.Handle(command);

            //Assert
            _validator.Verify(x => x.Validate(It.IsAny<UpsertRegisteredUserCommand>()), Times.Once);
        }

        [Test]
        public void ThenAnExceptionIsThrownIfTheCommandIsInvalid()
        {
            //Arrange
            _validator.Setup(x => x.Validate(It.IsAny<UpsertRegisteredUserCommand>()))
                .Returns(new ValidationResult(
                    new List<ValidationFailure>
                    {
                        new ValidationFailure("TEST", "ERROR")
                    }
                ));

            var command = new UpsertRegisteredUserCommand();

            //Act & Assert
            Assert.ThrowsAsync<ValidationException>(() =>_handler.Handle(command));
        }
    

        [Test]
        public async Task ThenTheRepositoryIsCalledToUpsertRegisteredUser()
        {
            //Arrange
            var command = new UpsertRegisteredUserCommand
            {
                UserRef = "UserRef",
                DisplayName = "DisplayName",
                Email = "Email",
                Ukprn = 12345
            };

            //Act
            await _handler.Handle(command);

            //Assert
            _repository.Verify(x => x.Upsert(
                It.Is<User>(
                    user => user.UserRef == command.UserRef
                    && user.DisplayName == command.DisplayName
                    && user.Email == command.Email
                    && user.Ukprn == command.Ukprn
                    )),
                Times.Once);
        }
    }
}
