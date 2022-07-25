using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.Commitments.Api.Types.Commitment.Types;
using SFA.DAS.Commitments.Api.Types.TrainingProgramme;
using SFA.DAS.HashingService;
using SFA.DAS.ProviderApprenticeshipsService.Application.Exceptions;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models.ApprenticeshipCourse;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators
{
    public class BaseCommitmentOrchestrator
    {
        protected readonly IMediator Mediator;
        protected readonly IHashingService HashingService;
        protected readonly IProviderCommitmentsLogger Logger;

        public BaseCommitmentOrchestrator(IMediator mediator,
            IHashingService hashingService, IProviderCommitmentsLogger logger)
        {
            // we keep null checks here, as this is a base class
            Mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            HashingService = hashingService ?? throw new ArgumentNullException(nameof(hashingService));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected static void AssertCommitmentStatus(CommitmentView commitment)
        {
            AssertCommitmentStatus(commitment, EditStatus.ProviderOnly);
            AssertCommitmentStatus(commitment, AgreementStatus.EmployerAgreed, AgreementStatus.ProviderAgreed, AgreementStatus.NotAgreed);
        }

        protected static void AssertCommitmentStatus(CommitmentView commitment, params AgreementStatus[] allowedAgreementStatuses)
        {
            if (commitment == null)
                throw new InvalidStateException("Null commitment");

            if (!allowedAgreementStatuses.Contains(commitment.AgreementStatus))
                throw new InvalidStateException($"Invalid commitment state (agreement status is {commitment.AgreementStatus}, expected {string.Join(",", allowedAgreementStatuses)})");
        }

        protected static void AssertCommitmentStatus(CommitmentView commitment, params EditStatus[] allowedEditStatuses)
        {
            if (commitment == null)
                throw new InvalidStateException("Null commitment");

            if (!allowedEditStatuses.Contains(commitment.EditStatus))
                throw new InvalidStateException($"Invalid commitment state (edit status is {commitment.EditStatus}, expected {string.Join(",", allowedEditStatuses)})");
        }
    }
}