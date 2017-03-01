using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Client;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.ProviderApprenticeshipsService.Application.Commands.UpdateRelationship;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.UnitTests.Commands.UpdateRelationship
{
    [TestFixture]
    public class WhenUpdatingARelationship
    {
        private UpdateRelationshipCommandHandler _handler;
        private Mock<UpdateRelationshipCommandValidator> _validator;
        private Mock<ICommitmentsApi> _commitmentsApi;


        [SetUp]
        public void Arrange()
        {
            _validator = new Mock<UpdateRelationshipCommandValidator>();
            _validator.Setup(x => x.Validate(It.IsAny<UpdateRelationshipCommand>()))
                .Returns(new ValidationResult());

            _commitmentsApi = new Mock<ICommitmentsApi>();
            _commitmentsApi.Setup(x =>
                    x.PatchRelationship(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<string>(),
                        It.IsAny<RelationshipRequest>())).Returns(Task.FromResult(new Unit()));

            _handler = new UpdateRelationshipCommandHandler(_commitmentsApi.Object, _validator.Object);
        }


        [Test]
        public async Task ThenTheApiIsCalledToPerformPatch()
        {
            //Arrange
            var command = new UpdateRelationshipCommand {Relationship = new Relationship()};

            //Act
            await _handler.Handle(command);

            //Assert
            _commitmentsApi.Verify(x => x.PatchRelationship(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<string>(),
                It.IsAny<RelationshipRequest>()), Times.Once);
        }

        [Test]
        public async Task ThenTheApiIsNotCalledIfTheRequestIsNotValid()
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
            _commitmentsApi.Verify(x => x.PatchRelationship(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<string>(),
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
