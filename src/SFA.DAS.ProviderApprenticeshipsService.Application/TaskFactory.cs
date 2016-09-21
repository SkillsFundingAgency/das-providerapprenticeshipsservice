using System;
using SFA.DAS.Tasks.Api.Types;


namespace SFA.DAS.ProviderApprenticeshipsService.Application
{
    public static class TaskFactory
    {
        public static Task Create(long accountId, string taskName, string body)
        {
            var assignee = $"EMPLOYER-{accountId}";

            return new Task
            {
                Assignee = assignee,
                Body = body,
                TaskTemplateId = 1,
                Name = taskName,
                CreatedOn = DateTime.UtcNow,
                TaskStatus = 0
            };
        }
    }
}
