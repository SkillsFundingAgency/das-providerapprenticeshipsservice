﻿using System.Collections.Generic;
using SFA.DAS.Tasks.Api.Types;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Models
{
    public class TaskListViewModel
    {
        public long ProviderId { get; set; }
        public List<Task> Tasks { get; set; }
    }
}