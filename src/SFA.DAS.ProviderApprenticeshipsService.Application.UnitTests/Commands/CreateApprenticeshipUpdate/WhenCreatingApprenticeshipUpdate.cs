using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.ProviderApprenticeshipsService.Application.Commands.CreateApprenticeshipUpdate;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.UnitTests.Commands.CreateApprenticeshipUpdate
{
    [TestFixture]
    public class WhenCreatingApprenticeshipUpdate
    {
        private CreateApprenticeshipUpdateCommandHandler _handler;
        private Mock<IProviderCommitmentsApi> _commitmentsApi;
        private Mock<AbstractValidator<CreateApprenticeshipUpdateCommand>> _validator;

        [SetUp]
        public void Arrange()
        {
            _commitmentsApi = new Mock<IProviderCommitmentsApi>();
            _commitmentsApi.Setup(x => x.CreateApprenticeshipUpdate(It.IsAny<long>(), It.IsAny<ApprenticeshipUpdateRequest>()))
                .Returns(() => Task.FromResult(new Unit()));

            _validator = new Mock<AbstractValidator<CreateApprenticeshipUpdateCommand>>();
            _validator.Setup(x => x.Validate(It.IsAny<CreateApprenticeshipUpdateCommand>()))
                .Returns(new ValidationResult());
            
            _handler = new CreateApprenticeshipUpdateCommandHandler(_commitmentsApi.Object, _validator.Object);
        }

        [Test]
        public async Task ThenTheCommandIsValidated()
        {
            //Arrange
            var command = new CreateApprenticeshipUpdateCommand
            {
                ApprenticeshipUpdate = new ApprenticeshipUpdate(),
                ProviderId = 1,
                UserId = "Tester"
            };

            //Act
            await _handler.Handle(command);

            //Assert
            _validator.Verify(x=> x.Validate(It.IsAny<CreateApprenticeshipUpdateCommand>()), Times.Once);
        }

        [Test]
        public async Task ThenTheCommitmentsApiIsCalledToCreateTheUpdate()
        {
            //Arrange
            var command = new CreateApprenticeshipUpdateCommand
            {
                ApprenticeshipUpdate = new ApprenticeshipUpdate(),
                ProviderId = 1,
                UserId = "Tester"
            };

            //Act
            await _handler.Handle(command);

            //Assert
            _commitmentsApi.Verify(x=> x.CreateApprenticeshipUpdate(It.IsAny<long>(), It.IsAny<ApprenticeshipUpdateRequest>()), Times.Once);
        }
    }
}
