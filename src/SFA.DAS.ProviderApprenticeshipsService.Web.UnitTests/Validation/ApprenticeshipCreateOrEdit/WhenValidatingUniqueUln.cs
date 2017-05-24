using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetCommitment;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Validation;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Validation.ApprenticeshipCreateOrEdit
{
    [TestFixture]
    public class WhenValidatingUniqueUln
    {
        private ApprenticeshipViewModelUniqueUlnValidator _validator;
        private Mock<IMediator> _mediator;
        private Mock<IHashingService> _hashingService;

        [SetUp]
        public void Arrange()
        {
            _hashingService = new Mock<IHashingService>();
            _hashingService.Setup(x => x.DecodeValue(It.IsAny<string>())).Returns(() => 0);

            _mediator = new Mock<IMediator>();
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetCommitmentQueryRequest>()))
                .ReturnsAsync(() => new GetCommitmentQueryResponse
                {
                    Commitment = new CommitmentView
                    {
                        Apprenticeships = new List<Apprenticeship>()
                    }
                });

            _validator = new ApprenticeshipViewModelUniqueUlnValidator(_mediator.Object, _hashingService.Object);
        }

        [Test]
        public async Task ThenTheMediatorIsCalledToRetrieveTheCohort()
        {
            //Arrange
            var viewModel = new ApprenticeshipViewModel
            {
                ULN = "TEST"
            };

            //Act
            await _validator.ValidateAsync(viewModel);

            //Assert
            _mediator.Verify(x => x.SendAsync(It.IsAny<GetCommitmentQueryRequest>()), Times.Once);
        }

        [Test]
        public async Task ThenValidationIsNotPerformedOnAnEmptyUln()
        {
            //Arrange
            var viewModel = new ApprenticeshipViewModel();

            //Act
            await _validator.ValidateAsync(viewModel);

            //Assert
            _mediator.Verify(x => x.SendAsync(It.IsAny<GetCommitmentQueryRequest>()), Times.Never);
        }

        [Test]
        public async Task ThenIfTheUlnIsNotUniqueThenNotValid()
        {
            //Arrange
            var viewModel = new ApprenticeshipViewModel
            {
                ULN = "TEST"
            };

            _mediator.Setup(x => x.SendAsync(It.IsAny<GetCommitmentQueryRequest>()))
                .ReturnsAsync(() => new GetCommitmentQueryResponse
                {
                    Commitment = new CommitmentView
                    {
                        Apprenticeships = new List<Apprenticeship>
                        {
                            new Apprenticeship { Id = 1, ULN = "TEST2" },
                            new Apprenticeship { Id = 2, ULN = "TEST" },
                            new Apprenticeship { Id = 3, ULN = "TEST3" }
                        }
                    }
                });

            //Act
            var result = await _validator.ValidateAsync(viewModel);

            //Assert
            Assert.IsFalse(result.IsValid);
        }

        [Test]
        public async Task ThenIfTheUlnIsUniqueThenIsValid()
        {
            //Arrange
            var viewModel = new ApprenticeshipViewModel
            {
                ULN = "TEST"
            };

            _mediator.Setup(x => x.SendAsync(It.IsAny<GetCommitmentQueryRequest>()))
                .ReturnsAsync(() => new GetCommitmentQueryResponse
                {
                    Commitment = new CommitmentView
                    {
                        Apprenticeships = new List<Apprenticeship>
                        {
                            new Apprenticeship { Id = 1, ULN = "TEST2" },
                            new Apprenticeship { Id = 2, ULN = "TEST3" }
                        }
                    }
                });

            //Act
            var result = await _validator.ValidateAsync(viewModel);

            //Assert
            Assert.IsTrue(result.IsValid);
        }

        [Test]
        public async Task ThenWhenARecordIsBeingUpdatedTheUlnUniquenessCheckDoesNotIncludeSelf()
        {
            //Arrange
            var viewModel = new ApprenticeshipViewModel
            {
                HashedApprenticeshipId = "1",
                ULN = "TEST"
            };

            _hashingService.Setup(x => x.DecodeValue(It.IsAny<string>())).Returns(() => 1);

            _mediator.Setup(x => x.SendAsync(It.IsAny<GetCommitmentQueryRequest>()))
                .ReturnsAsync(() => new GetCommitmentQueryResponse
                {
                    Commitment = new CommitmentView
                    {
                        Apprenticeships = new List<Apprenticeship>
                        {
                            new Apprenticeship { Id = 1, ULN = "TEST" },
                            new Apprenticeship { Id = 2, ULN = "TEST1" },
                            new Apprenticeship { Id = 3, ULN = "TEST2" }
                        }
                    }
                });

            //Act
            var result = await _validator.ValidateAsync(viewModel);

            //Assert
            Assert.IsTrue(result.IsValid);
        }
    }
}
