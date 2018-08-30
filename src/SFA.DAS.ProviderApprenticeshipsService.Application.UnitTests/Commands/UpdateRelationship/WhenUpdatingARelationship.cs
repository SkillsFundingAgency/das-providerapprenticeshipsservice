using System.Collections.Generic;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.ProviderApprenticeshipsService.Application.Commands.UpdateRelationship;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.UnitTests.Commands.UpdateRelationship
{
    [TestFixture]
    public class WhenUpdatingARelationship
    {
        private UpdateRelationshipCommandHandler _handler;
        private Mock<IValidator<UpdateRelationshipCommand>> _validator;
        private Mock<IRelationshipApi> _relationshipApi;

        [SetUp]
        public void Arrange()
        {
            _validator = new Mock<IValidator<UpdateRelationshipCommand>>();
            _validator.Setup(x => x.Validate(It.IsAny<UpdateRelationshipCommand>()))
                .Returns(new ValidationResult());

            _relationshipApi = new Mock<IRelationshipApi>();
            _relationshipApi.Setup(x =>
                    x.PatchRelationship(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<string>(),
                        It.IsAny<RelationshipRequest>())).Returns(Task.FromResult(new Unit()));

            _handler = new UpdateRelationshipCommandHandler(_relationshipApi.Object, _validator.Object);
        }

        [Test]
        public async Task ThenTheApiIsCalledToPerformPatch()
        {
            //Arrange
            var command = new UpdateRelationshipCommand {Relationship = new Relationship()};

            //Act
            await _handler.Handle(command);

            //Assert
            _relationshipApi.Verify(x => x.PatchRelationship(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<string>(),
                It.IsAny<RelationshipRequest>()), Times.Once);
        }

        [Test]
        public void ThenTheApiIsNotCalledIfTheRequestIsNotValid()
        {
            //Arrange
            _validator.Setup(x => x.Validate(It.IsAny<UpdateRelationshipCommand>()))
                .Returns(new ValidationResult(new List<ValidationFailure>
                {
                    new ValidationFailure("Test", "Test error")
                }));

            var command = new UpdateRelationshipCommand { Relationship = new Relationship() };

            //Act & Assert
            Assert.ThrowsAsync<ValidationException>(() =>
                _handler.Handle(command)
            );

            //Assert
            _relationshipApi.Verify(x => x.PatchRelationship(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<string>(),
                It.IsAny<RelationshipRequest>()), Times.Never);
        }

        [Test]
        public async Task ThenTheRequestIsValidated()
        {
            //Arrange
            var command = new UpdateRelationshipCommand { Relationship = new Relationship() };

            //Act
            await _handler.Handle(command);

            //Assert
            _validator.Verify(x=> x.Validate(It.IsAny<UpdateRelationshipCommand>()), Times.Once);
        }
    }
}
