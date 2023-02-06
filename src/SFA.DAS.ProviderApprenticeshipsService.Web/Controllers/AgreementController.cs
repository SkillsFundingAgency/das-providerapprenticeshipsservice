﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Web.Attributes;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Types;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Controllers
{
    [Authorize]
    [ServiceFilter(typeof(ProviderUkPrnCheckActionFilter))] //[ProviderUkPrnCheck]
    [Route("{providerId}/agreements")]
    public class AgreementController : BaseController
    {
        private readonly AgreementOrchestrator _orchestrator;

        public AgreementController(AgreementOrchestrator orchestrator, ICookieStorageService<FlashMessageViewModel> flashMessage) : base(flashMessage)
        {
            _orchestrator = orchestrator;
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Agreements(long providerId, string organisation = "")
        {
            var model = await _orchestrator.GetAgreementsViewModel(providerId, organisation);
            return View(model);
        }
    }
}