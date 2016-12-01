using System.Collections.Generic;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Models
{
    public class TaskListViewModel
    {
        public long ProviderId { get; set; }
        public List<TaskItemViewModel> Tasks { get; set; }
    }
}