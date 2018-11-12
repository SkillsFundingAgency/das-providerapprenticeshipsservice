using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetEmployerAccountLegalEntities;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models.Organisation;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.UnitTests.Queries.GetEmployerAccountLegalEntities
{
    [TestFixture]
    public class WhenIGetEmployerAccountLegalEntities
    {
        private Mock<IEmployerAccountService> _employerAccountService;
        private GetEmployerAccountLegalEntitiesRequest _validRequest { get; set; }
        private GetEmployerAccountLegalEntitiesHandler _handler { get; set; }
        private List<LegalEntity> _apiResponse;


        [SetUp]
        public void Arrange()
        {
            _apiResponse = new List<LegalEntity>();

            _employerAccountService = new Mock<IEmployerAccountService>();
            _employerAccountService.Setup(x => x.GetLegalEntitiesForAccount(It.IsAny<string>()))
                .ReturnsAsync(_apiResponse);

            _validRequest = new GetEmployerAccountLegalEntitiesRequest();

            _handler = new GetEmployerAccountLegalEntitiesHandler(_employerAccountService.Object);
        }

        [Test]
        public async Task ThenTheApiResponseIsReturned()
        {
            var result = await _handler.Handle(TestHelper.Clone(_validRequest), new CancellationToken());

            Assert.AreEqual(_apiResponse, result.LegalEntities);
        }
    }
}
