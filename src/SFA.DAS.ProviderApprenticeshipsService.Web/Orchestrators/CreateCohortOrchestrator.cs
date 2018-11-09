﻿using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
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
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.CreateCohort;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.Mappers;
using SFA.DAS.ProviderRelationships.Types.Models;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators
{
    public class CreateCohortOrchestrator : BaseCommitmentOrchestrator
    {
        private readonly ICreateCohortMapper _createCohortMapper;

        public CreateCohortOrchestrator(
            IMediator mediator,
            ICreateCohortMapper createCohortMapper,
            IHashingService hashingService,
            IProviderCommitmentsLogger logger) : base(mediator, hashingService, logger)
        {
            _createCohortMapper = createCohortMapper;
        }

        public async Task<CreateCohortViewModel> GetCreateCohortViewModel(long providerId)
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

            //todo: call provider relationships again to verify that the user selected a valid option/has permission
            //this will be obsoleted by the Auth check but worth doing now?

            var providerResponse = await Mediator.Send(new GetProviderQueryRequest { UKPRN = providerId });

            var employerAccountId = HashingService.DecodeValue(confirmEmployerViewModel.EmployerAccountHashedId);

            var accountRequest = await Mediator.Send(new GetEmployerAccountLegalEntitiesRequest
            {
                UserId = userId,
                HashedAccountId = confirmEmployerViewModel.EmployerAccountHashedId
            });

            var legalEntity = accountRequest.LegalEntities.SingleOrDefault(x =>
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