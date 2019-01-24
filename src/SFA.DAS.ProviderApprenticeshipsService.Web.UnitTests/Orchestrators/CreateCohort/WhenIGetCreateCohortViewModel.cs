using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.HashingService;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetProviderRelationshipsWithPermission;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Services;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.Mappers;
using SFA.DAS.ProviderRelationships.Types.Dtos;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Orchestrators.CreateCohort
{
    [TestFixture]
    public class WhenIGetCreateCohortViewModel
    {
        private CreateCohortOrchestrator _orchestrator;
        private Mock<IMediator> _mediator;
        private Mock<ISelectEmployerMapper> _createCohortMapper;
        private GetProviderRelationshipsWithPermissionQueryResponse _permissionsResponse;
        private ChooseEmployerViewModel _cohortViewModel;

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

            _cohortViewModel = new ChooseEmployerViewModel();
            _createCohortMapper = new Mock<ISelectEmployerMapper>();
            _createCohortMapper.Setup(x =>
                    x.Map(It.IsAny<IEnumerable<AccountProviderLegalEntityDto>>()))
                .Returns(_cohortViewModel);

            _orchestrator = new CreateCohortOrchestrator(_mediator.Object,
                _createCohortMapper.Object,
                Mock.Of<IHashingService>(),
                Mock.Of<IProviderCommitmentsLogger>(),
                Mock.Of<IPublicHashingService>());
        }

        [Test]
        public async Task ThenProviderRelationshipResponseIsMapped()
        {
            await _orchestrator.GetCreateCohortViewModel(1);

            _createCohortMapper.Verify(x =>
                x.Map(It.Is<IEnumerable<AccountProviderLegalEntityDto>>(r =>
                    r.Equals(_permissionsResponse.ProviderRelationships))));
        }

        [Test]
        public async Task ThenTheMappedResultIsReturned()
        {
            var result = await _orchestrator.GetCreateCohortViewModel(1);

            Assert.AreEqual(_cohortViewModel, result);
        }
    }
}
