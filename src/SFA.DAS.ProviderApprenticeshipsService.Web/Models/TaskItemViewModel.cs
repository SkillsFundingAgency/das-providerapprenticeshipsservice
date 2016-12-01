namespace SFA.DAS.ProviderApprenticeshipsService.Web.Models
{
    using System;

    public class TaskItemViewModel
    {
        public DateTime CreatedOn { get; set; }

        public string Name { get; set; }

        public object TaskStatus { get; set; }

        public string HashedTaskId { get; set; }
    }
}