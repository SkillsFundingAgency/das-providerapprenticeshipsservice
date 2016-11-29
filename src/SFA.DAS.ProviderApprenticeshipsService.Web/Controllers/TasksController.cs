using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Controllers
{
    [Authorize]
    public class TasksController : Controller
    {
        private readonly TaskOrchestrator _taskOrchestrator;

        public TasksController(TaskOrchestrator taskOrchestrator)
        {
            if (taskOrchestrator == null)
                throw new ArgumentNullException(nameof(taskOrchestrator));
            _taskOrchestrator = taskOrchestrator;
        }

        public async Task<ActionResult> Index(long providerId)
        {
            var model = await _taskOrchestrator.GetAll(providerId);

            return View(model);
        }

        public async Task<ActionResult> View(string hashedTaskId, long providerId)
        {
            var model = await _taskOrchestrator.GetTask(hashedTaskId, providerId);

            return View(model);
        }
    }
}