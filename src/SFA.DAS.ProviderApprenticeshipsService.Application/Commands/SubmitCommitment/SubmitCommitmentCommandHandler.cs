using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Commitments.Api.Client;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.Tasks.Api.Client;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Commands.SubmitCommitment
{
    public class SubmitCommitmentCommandHandler : AsyncRequestHandler<SubmitCommitmentCommand>
    {
        private readonly ICommitmentsApi _commitmentsApi;
        private readonly ITasksApi _tasksApi;
        private readonly SubmitCommitmentCommandValidator _validator;

        public SubmitCommitmentCommandHandler(ICommitmentsApi commitmentsApi, ITasksApi tasksApi)
        {
            if (commitmentsApi == null)
                throw new ArgumentNullException(nameof(commitmentsApi));
            if (tasksApi == null)
                throw new ArgumentNullException(nameof(tasksApi));

            _commitmentsApi = commitmentsApi;
            _tasksApi = tasksApi;
            _validator = new SubmitCommitmentCommandValidator();
        }

        protected override async Task HandleCore(SubmitCommitmentCommand message)
        {
            var validationResult = _validator.Validate(message);

            if (!validationResult.IsValid)
                throw new InvalidRequestException(validationResult.Errors);

            var commitment = await _commitmentsApi.GetProviderCommitment(message.ProviderId, message.CommitmentId);

            await _commitmentsApi.PatchProviderCommitment(message.ProviderId, message.CommitmentId, CommitmentStatus.Active);

            if (!string.IsNullOrWhiteSpace(message.Message))
            {
                var assignee = $"EMPLOYER-{commitment.EmployerAccountId}";
                var task = TaskFactory.Create(commitment.EmployerAccountId, "SubmitCommitment", message.Message);

                await _tasksApi.CreateTask(assignee, task);

            }
        }
    }
}