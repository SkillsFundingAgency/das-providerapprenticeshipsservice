using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.Apprenticeship.Types;
using SFA.DAS.ProviderApprenticeshipsService.Application.Commands.ReviewApprenticeshipUpdate;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.UnitTests.Commands.ReviewApprenticeshipUpdate
{
    [TestFixture]
    public class WhenReviewingApprenticeshipUpdate
    {
        private IRequestHandler<ReviewApprenticeshipUpdateCommand> _handler;
        private Mock<IProviderCommitmentsApi> _commitmentsApi;
        private Mock<IValidator<ReviewApprenticeshipUpdateCommand>> _validator;

        [SetUp]
        public void Arrange()
        {
            _validator = new Mock<IValidator<ReviewApprenticeshipUpdateCommand>>();
            _validator.Setup(x => x.Validate(It.IsAny<ReviewApprenticeshipUpdateCommand>()))
                .Returns(() => new ValidationResult());

            _commitmentsApi = new Mock<IProviderCommitmentsApi>();
            _commitmentsApi.Setup(x => x.PatchApprenticeshipUpdate(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<ApprenticeshipUpdateSubmission>()))
                .Returns(()=> Task.FromResult(new Unit()));

            _handler = new ReviewApprenticeshipUpdateCommandHandler(_validator.Object, _commitmentsApi.Object);
        }

        [Test]
        public async Task ThenTheCommandIsValidated()
        {
            //Arrange
            var command = new ReviewApprenticeshipUpdateCommand();

            //Act
            await _handler.Handle(command, new CancellationToken());

            //Assert
            _validator.Verify(x=> x.Validate(It.IsAny<ReviewApprenticeshipUpdateCommand>()), Times.Once);
        }

        [Test]
        public void ThenIfTheRequestIsNotValidThenAnExceptionIsThrown()
        {
            //Arrange
            _validator.Setup(x => x.Validate(It.IsAny<ReviewApprenticeshipUpdateCommand>()))
                .Returns(() => new ValidationResult
                {
                    Errors = { new ValidationFailure("Test", "Error")}
                });

            var command = new ReviewApprenticeshipUpdateCommand();

            //Act & Assert
            Func<Task> act = async () => { await _handler.Handle(command, new CancellationToken()); };
            act.ShouldThrow<ValidationException>();
        }

        [TestCase(true, ApprenticeshipUpdateStatus.Approved)]
        [TestCase(false, ApprenticeshipUpdateStatus.Rejected)]
        public async Task ThenTheCommitmentsApiIsCalledToSubmitTheUpdate(bool isApproved, ApprenticeshipUpdateStatus expectedStatus)
        {
            //Arrange
            var command = new ReviewApprenticeshipUpdateCommand
            {
                ApprenticeshipId = 1,
                ProviderId = 2,
                IsApproved = isApproved,
                UserId = "tester",
                UserDisplayName = "Bob",
                UserEmailAddress = "test@email.com"
            };

            //Act
            await _handler.Handle(command, new CancellationToken());

            //Assert
            _commitmentsApi.Verify(x => x.PatchApprenticeshipUpdate(
                    command.ProviderId,
                    command.ApprenticeshipId,
                    It.Is<ApprenticeshipUpdateSubmission>(
                        s =>
                            s.UpdateStatus == expectedStatus && s.UserId == command.UserId && s.LastUpdatedByInfo.EmailAddress == command.UserEmailAddress &&
                            s.LastUpdatedByInfo.Name == command.UserDisplayName)),
                Times.Once);
        }
    }
}
