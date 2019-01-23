using System;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.Commitments.Api.Types.Commitment.Types;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.HashingService;
using SFA.DAS.ProviderApprenticeshipsService.Application.Commands.CreateCommitment;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetEmployerAccountLegalEntities;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetProvider;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetProviderRelationshipsWithPermission;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Services;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.Mappers;
using SFA.DAS.ProviderRelationships.Types.Models;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators
{
    public class CreateCohortOrchestrator : BaseCommitmentOrchestrator
    {
        private readonly ICreateCohortMapper _createCohortMapper;
        private readonly IPublicHashingService _publicHashingService;

        public CreateCohortOrchestrator(
            IMediator mediator,
            ICreateCohortMapper createCohortMapper,
            IHashingService hashingService,
            IProviderCommitmentsLogger logger,
            IPublicHashingService publicHashingService) : base(mediator, hashingService, logger)
        {
            _createCohortMapper = createCohortMapper;
            _publicHashingService = publicHashingService;
        }

        public async Task<ChooseEmployerViewModel> GetCreateCohortViewModel(long providerId)
        {
            Logger.Info($"Getting create cohort view model", providerId);

            var relationshipsWithPermission = await Mediator.Send(new GetProviderRelationshipsWithPermissionQueryRequest
            {
                ProviderId = providerId,
                Permission = Operation.CreateCohort
            });

            var result = _createCohortMapper.Map(relationshipsWithPermission.ProviderRelationships);

            return result;
        }

        public async Task<string> CreateCohort(int providerId, ConfirmEmployerViewModel confirmEmployerViewModel, string userId, SignInUserModel signinUser)
        {
            Logger.Info($"Creating cohort", providerId);

            var employerAccountId = _publicHashingService.DecodeValue(confirmEmployerViewModel.EmployerAccountPublicHashedId);

            var relationshipData = await Mediator.Send(new GetProviderRelationshipsWithPermissionQueryRequest
            {
                Permission = Operation.CreateCohort,
                ProviderId = providerId
            });

            //Check that the relationship is a valid selection
            var relationship = relationshipData.ProviderRelationships.SingleOrDefault(x =>
                x.AccountPublicHashedId == confirmEmployerViewModel.EmployerAccountPublicHashedId
                && x.AccountLegalEntityPublicHashedId == confirmEmployerViewModel.EmployerAccountLegalEntityPublicHashedId
            );

            if (relationship == null)
            {
                throw new InvalidOperationException(
                    $"Error creating cohort - operation not permitted for Provider: {providerId}, Employer Account {employerAccountId}, Legal Entity {confirmEmployerViewModel.EmployerAccountLegalEntityPublicHashedId} ");
            }

            var providerResponse = await Mediator.Send(new GetProviderQueryRequest { UKPRN = providerId });

            var employerAccountHashedId = HashingService.HashValue(employerAccountId);

            var accountResponse = await Mediator.Send(new GetEmployerAccountLegalEntitiesRequest
            {
                UserId = userId,
                HashedAccountId = employerAccountHashedId
            });

            var legalEntity = accountResponse.LegalEntities.SingleOrDefault(x =>
                x.AccountLegalEntityPublicHashedId ==
                confirmEmployerViewModel.EmployerAccountLegalEntityPublicHashedId);

            if (legalEntity == null)
            {
                throw new InvalidOperationException(
                    $"Error getting Employer Account Legal entity for {confirmEmployerViewModel.EmployerAccountLegalEntityPublicHashedId}");
            }

            var createCommitmentRequest = new CreateCommitmentCommand
            {
                UserId = userId,
                Commitment = new Commitment
                {
                    Reference = Guid.NewGuid().ToString().ToUpper(),
                    EmployerAccountId = employerAccountId,
                    LegalEntityId = legalEntity.Code,
                    LegalEntityName = legalEntity.Name,
                    LegalEntityAddress = legalEntity.RegisteredAddress,
                    LegalEntityOrganisationType = (OrganisationType) legalEntity.Source,
                    AccountLegalEntityPublicHashedId = legalEntity.AccountLegalEntityPublicHashedId,
                    ProviderId = providerId,
                    ProviderName = providerResponse.ProvidersView.Provider.ProviderName,
                    CommitmentStatus = CommitmentStatus.New,
                    EditStatus = EditStatus.ProviderOnly,
                    ProviderLastUpdateInfo = new LastUpdateInfo {Name = signinUser.DisplayName, EmailAddress = signinUser.Email}
                }
            };

            var response = await Mediator.Send(createCommitmentRequest);

            return HashingService.HashValue(response.CommitmentId);
        }
    }
}