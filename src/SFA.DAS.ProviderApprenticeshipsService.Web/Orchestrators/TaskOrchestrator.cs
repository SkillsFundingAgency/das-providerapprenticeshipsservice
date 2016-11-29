using System;
using System.Threading.Tasks;
using MediatR;
using Newtonsoft.Json;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetTask;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetTasks;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.Tasks.Api.Types.Templates;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators
{
    using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;

    public class TaskOrchestrator
    {
        private readonly IMediator _mediator;

        private readonly IHashingService _hashingService;

        public TaskOrchestrator(IMediator mediator, IHashingService hashingService)
        {
            if (mediator == null)
                throw new ArgumentNullException(nameof(mediator));
            _mediator = mediator;
            this._hashingService = hashingService;
        }

        public async Task<TaskListViewModel> GetAll(long providerId)
        {
            var response = await _mediator.SendAsync(new GetTasksQueryRequest
            {
                ProviderId = providerId
            });

            return new TaskListViewModel
            {
                ProviderId = providerId,
                Tasks = response.Tasks
            };
        }

        public async Task<TaskViewModel> GetTask(long taskId, long providerId)
        {
            var response = await _mediator.SendAsync(new GetTaskQueryRequest
            {
                ProviderId = providerId,
                TaskId = taskId
            });

            var taskTemplate = JsonConvert.DeserializeObject<CreateCommitmentTemplate>(response.Task.Body);

            return new TaskViewModel
            {
                ProviderId = providerId,
                Task = response.Task,
                LinkId = taskTemplate.CommitmentId,
                HashedCommitmentId = this._hashingService.HashValue(taskTemplate.CommitmentId),
                Message = taskTemplate.Message
            };
        }
    }
}