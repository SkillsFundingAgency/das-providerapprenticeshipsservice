using System;
using System.Threading.Tasks;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Application.Commands.ReviewApprenticeshipUpdate;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.UnitTests.Commands.ReviewApprenticeshipUpdate
{
    [TestFixture]
    public class WhenReviewingApprenticeshipUpdate
    {
        private ReviewApprenticeshipUpdateCommandHandler _handler;
        private Mock<IProviderCommitmentsApi> _commitmentsApi;
        private Mock<AbstractValidator<ReviewApprenticeshipUpdateCommand>> _validator;

        [SetUp]
        public void Arrange()
        {
            _validator = new Mock<AbstractValidator<ReviewApprenticeshipUpdateCommand>>();
            _validator.Setup(x => x.Validate(It.IsAny<ReviewApprenticeshipUpdateCommand>()))
                .Returns(() => new ValidationResult());

            _commitmentsApi = new Mock<IProviderCommitmentsApi>();
            //_commitmentsApi.Setup(x=> x.) //todo: update method

            _handler = new ReviewApprenticeshipUpdateCommandHandler(_validator.Object, _commitmentsApi.Object);
        }

        [Test]
        public async Task ThenTheCommandIsValidated()
        {
            //Arrange
            var command = new ReviewApprenticeshipUpdateCommand();

            //Act
            await _handler.Handle(command);

            //Assert
            _validator.Verify(x=> x.Validate(It.IsAny<ReviewApprenticeshipUpdateCommand>()), Times.Once);
        }

        [Test]
        public async Task ThenIfTheRequestIsNotValidThenAnExceptionIsThrown()
        {
            //Arrange
            _validator.Setup(x => x.Validate(It.IsAny<ReviewApprenticeshipUpdateCommand>()))
                .Returns(() => new ValidationResult
                {
                    Errors = { new ValidationFailure("Test", "Error")}
                });

            var command = new ReviewApprenticeshipUpdateCommand();

            //Act & Assert
            Func<Task> act = async () => { await _handler.Handle(command); };
            act.ShouldThrow<ValidationException>();
        }

        [Test]
        public async Task ThenTheCommitmentsApiIsCalledToReviewTheUpdate()
        {
            //Arrange
            var command = new ReviewApprenticeshipUpdateCommand
            {
                ApprenticeshipId = 1,
                ProviderId = 2,
                IsApproved = true,
                UserId = "tester"
            };

            //Act
            await _handler.Handle(command);

            //Assert
            //todo:cf complete
            //_commitmentsApi.Verify();
            Assert.Fail();

        }
    }
}
