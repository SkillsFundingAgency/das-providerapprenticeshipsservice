﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Web.Attributes;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Types;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Controllers
{
    [Authorize]
    [ProviderUkPrnCheck]
    [RoutePrefix("{providerId}/agreements")]
    public class AgreementController : BaseController
    {
        private readonly AgreementOrchestrator _orchestrator;

        public AgreementController(AgreementOrchestrator orchestrator, ICookieStorageService<FlashMessageViewModel> flashMessage) : base(flashMessage)
        {
            _orchestrator = orchestrator;
        }
    }
}