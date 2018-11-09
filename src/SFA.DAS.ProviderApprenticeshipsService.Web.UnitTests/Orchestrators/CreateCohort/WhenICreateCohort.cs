using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types.Commitment.Types;
using SFA.DAS.HashingService;
using SFA.DAS.ProviderApprenticeshipsService.Application.Commands.CreateCommitment;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetEmployerAccountLegalEntities;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetProvider;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetProviderRelationshipsWithPermission;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models.ApprenticeshipProvider;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models.Organisation;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.CreateCohort;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.Mappers;
using SFA.DAS.ProviderRelationships.Types.Dtos;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Orchestrators.CreateCohort
{
    [TestFixture]
    public class WhenICreateCohort
    {
        private CreateCohortOrchestrator _orchestrator;
        private Mock<IMediator> _mediator;
        private Mock<IHashingService> _hashingService;
        private GetProviderRelationshipsWithPermissionQueryResponse _permissionsResponse;
        private ConfirmEmployerViewModel _confirmEmployerViewModel;
        private GetProviderQueryResponse _provider;
        private LegalEntity _legalEntity;
        private SignInUserModel _signInUserModel;
        private readonly string _userId = "userId";
        private readonly int _providerId = 10005124;
        private readonly long _employerAccountId = 1234;
        private readonly long _employerAccountLegalEntityId = 5678;
        private readonly string _employerAccountLegalEntityPublicHashedId = "EmployerAccountLegalEntityPublicHashedId";
        private readonly long _apiResponse = 789;

        [SetUp]
        public void Arrange()
        {
            _permissionsResponse = new GetProviderRelationshipsWithPermissionQueryResponse
            {
                ProviderRelationships = new List<RelationshipDto>
                {
                    new RelationshipDto
                    {
                        EmployerAccountId = _employerAccountId,
                        EmployerAccountLegalEntityId = _employerAccountLegalEntityId,
                        EmployerAccountLegalEntityPublicHashedId = _employerAccountLegalEntityPublicHashedId
                    }
                }
            };

            _hashingService = new Mock<IHashingService>();
            _hashingService.Setup(x => x.DecodeValue(It.Is<string>(s => s == "EmployerAccountHashedId"))).Returns(_employerAccountId);
            _hashingService.Setup(x => x.HashValue(It.Is<long>(l => l == _apiResponse))).Returns("CohortRef");
            _hashingService.Setup(x => x.DecodeValue(It.Is<string>(s => s == _employerAccountLegalEntityPublicHashedId)))
                .Returns(_employerAccountLegalEntityId);

            _mediator = new Mock<IMediator>();
            _mediator.Setup(x => x.Send(It.IsAny<GetProviderRelationshipsWithPermissionQueryRequest>(),
                    new CancellationToken()))
                .ReturnsAsync(_permissionsResponse);

            _provider = new GetProviderQueryResponse
            {
                ProvidersView = new ProvidersView
                {
                    CreatedDate = new DateTime(2018, 11, 6),
                    Provider = new Provider
                    {
                        ProviderName = "Test Provider"
                    }
                }
            };
            _mediator.Setup(x => x.Send(It.IsAny<GetProviderQueryRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(_provider);

            _legalEntity = new LegalEntity
            {
                Id = 1,
                AccountLegalEntityPublicHashedId = _employerAccountLegalEntityPublicHashedId,
                Code = "code",
                Name = "Test Legal Entity",
                RegisteredAddress = "Test Address",
                Source = 1
            };
            _mediator.Setup(x =>
                x.Send(It.IsAny<GetEmployerAccountLegalEntitiesRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(
                () => new GetEmployerAccountLegalEntitiesResponse
                {
                    LegalEntities = new List<LegalEntity>
                    {
                        _legalEntity
                    }
                });

            _mediator.Setup(x => x.Send(It.IsAny<CreateCommitmentCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CreateCommitmentCommandResponse{ CommitmentId = _apiResponse });

            _signInUserModel = new SignInUserModel {DisplayName = "DisplayName", Email = "test@email.com"};

            _confirmEmployerViewModel = new ConfirmEmployerViewModel
            {
                EmployerAccountLegalEntityPublicHashedId = _employerAccountLegalEntityPublicHashedId,
                EmployerAccountHashedId = "EmployerAccountHashedId"
            };

            _orchestrator = new CreateCohortOrchestrator(_mediator.Object,
                Mock.Of<ICreateCohortMapper>(),
                _hashingService.Object,
                Mock.Of<IProviderCommitmentsLogger>());
        }

        [Test]
        public async Task ThenTheCreateCommitmentCommandHasTheCorrectEmployerAccountId()
        {
            await _orchestrator.CreateCohort(_providerId, TestHelper.Clone(_confirmEmployerViewModel), _userId, _signInUserModel);

            _mediator.Verify(x =>
                x.Send(It.Is<CreateCommitmentCommand>(c =>
                        c.Commitment.EmployerAccountId == _employerAccountId),
                    It.IsAny<CancellationToken>()));
        }

        [Test]
        public async Task ThenTheCreateCommitmentCommandHasTheCorrectLegalEntityId()
        {
            await _orchestrator.CreateCohort(_providerId, TestHelper.Clone(_confirmEmployerViewModel), _userId, _signInUserModel);

            _mediator.Verify(x =>
                x.Send(It.Is<CreateCommitmentCommand>(c =>
                        c.Commitment.LegalEntityId == _legalEntity.Code),
                    It.IsAny<CancellationToken>()));
        }

        [Test]
        public async Task ThenTheCreateCommitmentCommandHasTheCorrectLegalEntityName()
        {
            await _orchestrator.CreateCohort(_providerId, TestHelper.Clone(_confirmEmployerViewModel), _userId, _signInUserModel);

            _mediator.Verify(x =>
                x.Send(It.Is<CreateCommitmentCommand>(c =>
                        c.Commitment.LegalEntityName == _legalEntity.Name),
                    It.IsAny<CancellationToken>()));
        }

        [Test]
        public async Task ThenTheCreateCommitmentCommandHasTheCorrectLegalEntityAddress()
        {
            await _orchestrator.CreateCohort(_providerId, TestHelper.Clone(_confirmEmployerViewModel), _userId, _signInUserModel);

            _mediator.Verify(x =>
                x.Send(It.Is<CreateCommitmentCommand>(c =>
                        c.Commitment.LegalEntityAddress == _legalEntity.RegisteredAddress),
                    It.IsAny<CancellationToken>()));
        }

        [Test]
        public async Task ThenTheCreateCommitmentCommandHasTheCorrectLegalEntityOrganisationType()
        {
            await _orchestrator.CreateCohort(_providerId, TestHelper.Clone(_confirmEmployerViewModel), _userId, _signInUserModel);

            _mediator.Verify(x =>
                x.Send(It.Is<CreateCommitmentCommand>(c =>
                        (short) c.Commitment.LegalEntityOrganisationType == _legalEntity.Source),
                    It.IsAny<CancellationToken>()));
        }

        [Test]
        public async Task ThenTheCreateCommitmentCommandHasTheCorrectAccountLegalEntityPublicHashedId()
        {
            await _orchestrator.CreateCohort(_providerId, TestHelper.Clone(_confirmEmployerViewModel), _userId, _signInUserModel);

            _mediator.Verify(x =>
                x.Send(It.Is<CreateCommitmentCommand>(c =>
                        c.Commitment.AccountLegalEntityPublicHashedId == _legalEntity.AccountLegalEntityPublicHashedId),
                    It.IsAny<CancellationToken>()));
        }

        [Test]
        public async Task ThenTheCreateCommitmentCommandHasTheCorrectProviderId()
        {
            await _orchestrator.CreateCohort(_providerId, TestHelper.Clone(_confirmEmployerViewModel), _userId, _signInUserModel);

            _mediator.Verify(x =>
                x.Send(It.Is<CreateCommitmentCommand>(c =>
                        c.Commitment.ProviderId == _providerId),
                    It.IsAny<CancellationToken>()));
        }
        [Test]
        public async Task ThenTheCreateCommitmentCommandHasTheCorrectProviderName()
        {
            await _orchestrator.CreateCohort(_providerId, TestHelper.Clone(_confirmEmployerViewModel), _userId, _signInUserModel);

            _mediator.Verify(x =>
                x.Send(It.Is<CreateCommitmentCommand>(c =>
                        c.Commitment.ProviderName == _provider.ProvidersView.Provider.ProviderName),
                    It.IsAny<CancellationToken>()));
        }

        [Test]
        public async Task ThenTheCreateCommitmentCommandHasTheCorrectEditStatus()
        {
            await _orchestrator.CreateCohort(_providerId, TestHelper.Clone(_confirmEmployerViewModel), _userId, _signInUserModel);

            _mediator.Verify(x =>
                x.Send(It.Is<CreateCommitmentCommand>(c =>
                        c.Commitment.EditStatus == EditStatus.ProviderOnly),
                    It.IsAny<CancellationToken>()));
        }

        [Test]
        public async Task ThenTheCreateCommitmentCommandHasTheCorrectCommitmentStatus()
        {
            await _orchestrator.CreateCohort(_providerId, TestHelper.Clone(_confirmEmployerViewModel), _userId, _signInUserModel);

            _mediator.Verify(x =>
                x.Send(It.Is<CreateCommitmentCommand>(c =>
                        c.Commitment.CommitmentStatus == CommitmentStatus.New),
                    It.IsAny<CancellationToken>()));
        }


        [Test]
        public async Task ThenTheCreateCommitmentCommandHasTheCorrectProviderLastUpdateInfo()
        {
            await _orchestrator.CreateCohort(_providerId, TestHelper.Clone(_confirmEmployerViewModel), _userId, _signInUserModel);

            _mediator.Verify(x =>
                x.Send(It.Is<CreateCommitmentCommand>(c =>
                        c.Commitment.ProviderLastUpdateInfo.Name == _signInUserModel.DisplayName &&
                        c.Commitment.ProviderLastUpdateInfo.EmailAddress == _signInUserModel.Email),
                    It.IsAny<CancellationToken>()));
        }

        [Test]
        public async Task ThenTheApiResponseIsReturned()
        {
            var result = await _orchestrator.CreateCohort(_providerId, TestHelper.Clone(_confirmEmployerViewModel), _userId, _signInUserModel);
            Assert.AreEqual("CohortRef", result);
        }


        [Test]
        public void ThenTheProviderMustHavePermission()
        {
            _confirmEmployerViewModel.EmployerAccountLegalEntityPublicHashedId = "LEGAL_ENTITY_WITHOUT_PERMISSION";

            Assert.ThrowsAsync<InvalidOperationException>(() => _orchestrator.CreateCohort(_providerId, TestHelper.Clone(_confirmEmployerViewModel), _userId, _signInUserModel));
        }
    }
}
