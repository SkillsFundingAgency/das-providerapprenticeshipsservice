using System;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.NLog.Logger;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetProvider;
using SFA.DAS.ProviderApprenticeshipsService.Domain;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Orchestrators.Account
{
    [TestFixture]
    public class WhenGettingProvider
    {
        private AccountOrchestrator _orchestrator;
        private Mock<IMediator> _mediator;
        private Mock<ICurrentDateTime> _currentDateTime;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetProviderQueryRequest>()))
                .ReturnsAsync(new GetProviderQueryResponse
                {
                    ProvidersView = new ProvidersView
                    {
                        CreatedDate = new DateTime(2000, 1, 1),
                        Provider = new Provider()
                    }
                });

            _currentDateTime = new Mock<ICurrentDateTime>();

            _orchestrator = new AccountOrchestrator(
                _mediator.Object,
                Mock.Of<ILog>(),
                _currentDateTime.Object
            );
        }


        [TestCase("2017-10-19", true, Description = "Banner visible")]
        [TestCase("2017-10-19 11:59:59", true, Description = "Banner visible until midnight")]
        [TestCase("2017-10-20 00:00:00", false, Description = "Banner hidden after midnight")]
        public async Task ThenDisplayOfAcademicYearBannerIsDetermined(DateTime now, bool expectShowBanner)
        {
            //Arrange
            _currentDateTime.Setup(x => x.Now).Returns(now);

            //Act
            var model = await _orchestrator.GetProvider(1);

            //Assert
            Assert.AreEqual(expectShowBanner, model.ShowAcademicYearBanner);
        }
    }
}
