using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.HashingService;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetProviderRelationshipsWithPermission;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Types;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.Mappers;
using SFA.DAS.ProviderRelationships.Types.Dtos;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Orchestrators.SelectEmployer
{
    [TestFixture]
    public class WhenIGetChooseEmployerViewModel
    {
        private SelectEmployerOrchestrator _orchestrator;
        private Mock<IMediator> _mediator;
        private Mock<ISelectEmployerMapper> _selectEmployerMapper;
        private GetProviderRelationshipsWithPermissionQueryResponse _permissionsResponse;
        private ChooseEmployerViewModel _chooseEmployerViewModel;

        [SetUp]
        public void Arrange()
        {
            _permissionsResponse = new GetProviderRelationshipsWithPermissionQueryResponse
            {
                ProviderRelationships = new List<AccountProviderLegalEntityDto>()
            };

            _mediator = new Mock<IMediator>();
            _mediator.Setup(x => x.Send(It.IsAny<GetProviderRelationshipsWithPermissionQueryRequest>(),
                    new CancellationToken()))
                .ReturnsAsync(_permissionsResponse);

            _chooseEmployerViewModel = new ChooseEmployerViewModel();
            _selectEmployerMapper = new Mock<ISelectEmployerMapper>();
            _selectEmployerMapper.Setup(x =>
                    x.Map(It.IsAny<IEnumerable<AccountProviderLegalEntityDto>>(), EmployerSelectionAction.CreateCohort))
                .Returns(_chooseEmployerViewModel);

            _orchestrator = new SelectEmployerOrchestrator(_mediator.Object,
                _selectEmployerMapper.Object,
                Mock.Of<IHashingService>(),
                Mock.Of<IProviderCommitmentsLogger>());
        }

        [TestCase(EmployerSelectionAction.CreateCohort)]
        [TestCase(EmployerSelectionAction.CreateReservation)]
        public async Task ThenProviderRelationshipResponseIsMapped(EmployerSelectionAction action)
        {
            await _orchestrator.GetChooseEmployerViewModel(1, action);

            _selectEmployerMapper.Verify(x =>
                x.Map(It.Is<IEnumerable<AccountProviderLegalEntityDto>>(r =>
                    r.Equals(_permissionsResponse.ProviderRelationships)), It.Is<EmployerSelectionAction>(r => r == action)));
        }

        [Test]
        public async Task ThenTheMappedResultIsReturned()
        {
            var result = await _orchestrator.GetChooseEmployerViewModel(1, EmployerSelectionAction.CreateCohort);

            Assert.AreEqual(_chooseEmployerViewModel, result);
        }
    }
}
