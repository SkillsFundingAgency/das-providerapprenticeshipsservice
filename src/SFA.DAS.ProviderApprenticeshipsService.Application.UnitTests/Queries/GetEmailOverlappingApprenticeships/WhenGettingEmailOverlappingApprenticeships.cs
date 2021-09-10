using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.Validation;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetEmailOverlapingApprenticeships;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.UnitTests.Queries.GetEmailOverlappingApprenticeships
{
    [TestFixture]
    public class WhenGettingEmailOverlappingApprenticeships
    {
        private GetEmailOverlappingApprenticeshipsQueryHandler _handler;
        private Mock<IValidationApi> _validationApi;
        private GetEmailOverlappingApprenticeshipsQueryRequest _validRequest;
        private List<ApprenticeshipEmailOverlapValidationResult> _validationResult;

        [SetUp]
        public void Arrange()
        {
            _validRequest = new GetEmailOverlappingApprenticeshipsQueryRequest()
            {
                Apprenticeship = new List<Apprenticeship>
                {
                    new Apprenticeship { StartDate = System.DateTime.UtcNow.AddMonths(6), EndDate =  System.DateTime.UtcNow.AddYears(2), Email = "apprentice1@yahoo.com" }
                }
            };
          
            _validationApi = new Mock<IValidationApi>();
            _handler = new GetEmailOverlappingApprenticeshipsQueryHandler(_validationApi.Object);
        }

        [Test]
        public async Task ThenTheApiClientIsCalled()
        {
            //Arrange
            _validationResult = new List<ApprenticeshipEmailOverlapValidationResult>()
            {
               new ApprenticeshipEmailOverlapValidationResult {
                   OverlappingApprenticeships = new List<OverlappingApprenticeship> {
                       new OverlappingApprenticeship {  Apprenticeship = new Apprenticeship { Email = "apprentice1@yahoo.com"  } }
                   }
               }
            };

            _validationApi.Setup(x => x.ValidateEmailOverlapping(It.IsAny<IEnumerable<ApprenticeshipEmailOverlapValidationRequest>>()))
               .ReturnsAsync(_validationResult);

            //Act
            var  result =  await _handler.Handle(TestHelper.Clone(_validRequest), new CancellationToken());

            //Assert
            Assert.AreEqual(result.Overlaps.FirstOrDefault().OverlappingApprenticeships.Count(), 1);            
            _validationApi.Verify(x => x.ValidateEmailOverlapping(It.IsAny<IEnumerable<ApprenticeshipEmailOverlapValidationRequest>>()), Times.Once);
        }


        [Test]
        public async Task ThenTheApiClientIsCalledWithOutEmailOverlap()
        {
            //Arrange
            _validationApi.Setup(x => x.ValidateEmailOverlapping(It.IsAny<IEnumerable<ApprenticeshipEmailOverlapValidationRequest>>()))
                          .ReturnsAsync(It.IsAny<IEnumerable<ApprenticeshipEmailOverlapValidationResult>>());

            _validRequest = new GetEmailOverlappingApprenticeshipsQueryRequest()
            {
                Apprenticeship = new List<Apprenticeship> { new Apprenticeship { StartDate = System.DateTime.UtcNow.AddMonths(6), EndDate =  System.DateTime.UtcNow.AddYears(2) } }
            };            

            //Act
            var result = await _handler.Handle(TestHelper.Clone(_validRequest), new CancellationToken());

            //Assert
            Assert.AreEqual(result.Overlaps.Count(), 0);            
            _validationApi.Verify(x => x.ValidateEmailOverlapping(It.IsAny<IEnumerable<ApprenticeshipEmailOverlapValidationRequest>>()), Times.Never);
        }        
    }
}
