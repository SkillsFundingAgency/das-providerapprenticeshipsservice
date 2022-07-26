using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
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
            IHashingService hashingService,
            IProviderCommitmentsLogger logger)
        {
            // we keep null checks here, as this is a base class
            Mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            HashingService = hashingService ?? throw new ArgumentNullException(nameof(hashingService));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
    }
}