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
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.CreateCohort;
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
        private Mock<ICreateCohortMapper> _createCohortMapper;
        private GetProviderRelationshipsWithPermissionQueryResponse _permissionsResponse;
        private CreateCohortViewModel _cohortViewModel;

        [SetUp]
        public void Arrange()
        {
            _permissionsResponse = new GetProviderRelationshipsWithPermissionQueryResponse
            {
                ProviderRelationships = new List<RelationshipDto>()
            };

            _mediator = new Mock<IMediator>();
            _mediator.Setup(x => x.Send(It.IsAny<GetProviderRelationshipsWithPermissionQueryRequest>(),
                    new CancellationToken()))
                .ReturnsAsync(_permissionsResponse);

            _cohortViewModel = new CreateCohortViewModel();
            _createCohortMapper = new Mock<ICreateCohortMapper>();
            _createCohortMapper.Setup(x =>
                    x.Map(It.IsAny<IEnumerable<RelationshipDto>>()))
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
                x.Map(It.Is<IEnumerable<RelationshipDto>>(r =>
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
