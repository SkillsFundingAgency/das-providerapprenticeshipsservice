using System;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Commands.UndoApprenticeshipUpdate
{
    public sealed class UndoApprenticeshipUpdateCommandHandler : AsyncRequestHandler<UndoApprenticeshipUpdateCommand>
    {
        private readonly IProviderCommitmentsApi _commitmentsApi;
        private readonly AbstractValidator<UndoApprenticeshipUpdateCommand> _validator;

        public UndoApprenticeshipUpdateCommandHandler(AbstractValidator<UndoApprenticeshipUpdateCommand> validator, IProviderCommitmentsApi commitmentsApi)
        {
            if (validator == null)
                throw new ArgumentNullException(nameof(validator));
            if(commitmentsApi == null)
                throw new ArgumentNullException(nameof(commitmentsApi));

            _validator = validator;
            _commitmentsApi = commitmentsApi;
        }

        protected override async Task HandleCore(UndoApprenticeshipUpdateCommand command)
        {
            var validationResult = _validator.Validate(command);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            //todo:cf complete
            //throw new NotImplementedException();
        }
    }
}
