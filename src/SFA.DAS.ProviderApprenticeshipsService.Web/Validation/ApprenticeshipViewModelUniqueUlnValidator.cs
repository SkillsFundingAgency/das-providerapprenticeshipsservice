using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using FluentValidation.Validators;
using MediatR;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetCommitment;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.HashingService;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Validation
{
    public class ApprenticeshipViewModelUniqueUlnValidator: AbstractValidator<ApprenticeshipViewModel>
    {
        private readonly IMediator _mediator;
        private readonly IHashingService _hashingService;

        //parameterless ctor for testing
        public ApprenticeshipViewModelUniqueUlnValidator()
        {
        }

        public ApprenticeshipViewModelUniqueUlnValidator(IMediator mediator, IHashingService hashingService)
        {
            if(mediator==null)
                throw new ArgumentNullException(nameof(mediator));
            if (hashingService == null)
                throw new ArgumentNullException(nameof(hashingService));

            _mediator = mediator;
            _hashingService = hashingService;

            RuleFor(x=> x.ULN)
                .MustAsync(UniqueUln)
                .When(x=> !string.IsNullOrWhiteSpace(x.ULN))
                .OverridePropertyName("ULN")
                .WithMessage("The unique learner number has already been used for an apprentice in this cohort");
        }

        public virtual async Task<ValidationResult> ValidateAsyncOverride(ApprenticeshipViewModel context)
        {
            return await base.ValidateAsync(context);
        }

        private async Task<bool> UniqueUln(ApprenticeshipViewModel viewModel, string uln, PropertyValidatorContext arg3, CancellationToken arg4)
        {
            var id = viewModel.HashedApprenticeshipId == null ? 0 : _hashingService.DecodeValue(viewModel.HashedApprenticeshipId);
            var commitmentId = _hashingService.DecodeValue(viewModel.HashedCommitmentId);

            var cohort = await _mediator.SendAsync(new GetCommitmentQueryRequest
            {
                CommitmentId = commitmentId,
                ProviderId = viewModel.ProviderId
            });

            return cohort.Commitment.Apprenticeships.All(existing => existing.Id == id || existing.ULN != uln);
        }
    }
}