using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.ProviderApprenticeshipsService.Application.Commands.CreateCommitment;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.UnitTests.Commands.CreateCommitment
{
    [TestFixture]
    public class WhenCreatingCommitment
    {
        private CreateCommitmentCommandHandler _handler;
        private Mock<IProviderCommitmentsApi> _commitmentsApi;
        private Mock<IValidator<CreateCommitmentCommand>> _validator;
        private CreateCommitmentCommand _validCommand;

        [SetUp]
        public void Arrange()
        {
            _validCommand = new CreateCommitmentCommand
            {
                Commitment = new Commitment
                {
                    ProviderId = 1,
                    ProviderName = "Test Provider",
                    LegalEntityId = "2",
                    LegalEntityName = "Test Legal Entity"
                }
            };

            _validator = new Mock<IValidator<CreateCommitmentCommand>>();
            _validator.Setup(x => x.Validate(It.IsAny<CreateCommitmentCommand>())).Returns(new ValidationResult());

            _commitmentsApi = new Mock<IProviderCommitmentsApi>();
            _commitmentsApi.Setup(x => x.CreateProviderCommitment(It.IsAny<long>(), It.IsAny<CommitmentRequest>()))
                .ReturnsAsync(new CommitmentView());

            _handler = new CreateCommitmentCommandHandler(_commitmentsApi.Object, _validator.Object);
        }

        [Test]
        public async Task ThenTheCommandIsValidated()
        {
            await _handler.Handle(TestHelper.Clone(_validCommand), new CancellationToken());
            _validator.Verify(x => x.Validate(It.IsAny<CreateCommitmentCommand>()));
        }

        [Test]
        public void ThenIfValidationFailsThenAnExceptionIsThrown()
        {
            _validator.Setup(x => x.Validate(It.IsAny<CreateCommitmentCommand>()))
                .Returns(new ValidationResult(new List<ValidationFailure>
                {
                    new ValidationFailure("TEST","ERROR")
                }));

            Assert.ThrowsAsync<ValidationException>(() =>
                _handler.Handle(TestHelper.Clone(_validCommand), new CancellationToken()));
        }

        [Test]
        public async Task TheTheCommitmentsApiIsCalledWithTheCommitment()
        {
            await _handler.Handle(TestHelper.Clone(_validCommand), new CancellationToken());

            _commitmentsApi.Verify(x =>
                x.CreateProviderCommitment(It.Is<long>(p => p == _validCommand.Commitment.ProviderId.Value),
                    It.Is<CommitmentRequest>(r => r.Commitment.LegalEntityId == _validCommand.Commitment.LegalEntityId
                        && r.Commitment.LegalEntityName == _validCommand.Commitment.LegalEntityName
                        && r.Commitment.ProviderName == _validCommand.Commitment.ProviderName
                        && r.Commitment.ProviderId == _validCommand.Commitment.ProviderId
                )));
        }
    }
}
