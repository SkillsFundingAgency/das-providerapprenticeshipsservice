﻿using System.Threading.Tasks;
using System.Web.Mvc;

using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Controllers
{
    [Authorize]
    [RoutePrefix("{providerId}/apprentices/manage")]
    public class ManageApprenticesController : BaseController
    {
        private readonly ManageApprenticesOrchestrator _orchestrator;

        public ManageApprenticesController(ManageApprenticesOrchestrator orchestrator)
        {
            _orchestrator = orchestrator;
        }

        [HttpGet]
        [Route("all")]
        [OutputCache(CacheProfile = "NoCache")]
        public async Task<ActionResult> Index(long providerId)
        {
            var model = await _orchestrator.GetApprenticeships(providerId);
            return View(model);
        }
    }
}