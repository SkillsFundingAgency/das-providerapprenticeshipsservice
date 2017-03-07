using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Mvc;

using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.BulkUpload;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Controllers
{
    [RoutePrefix("{providerId}/apprentices")]
    public class BulkUploadController : BaseController
    {
        private readonly BulkUploadOrchestrator _bulkUploadOrchestrator;

        private readonly string uploadErrorsTempDataKey = "UploadErrors";

        public BulkUploadController(BulkUploadOrchestrator bulkUploadOrchestrator)
        {
            if (bulkUploadOrchestrator == null)
                throw new ArgumentNullException(nameof(bulkUploadOrchestrator));
            _bulkUploadOrchestrator = bulkUploadOrchestrator;
        }

        [HttpGet]
        [OutputCache(CacheProfile = "NoCache")]
        [Route("{hashedCommitmentId}/UploadApprenticeships")]
        public async Task<ActionResult> UploadApprenticeships(long providerid, string hashedcommitmentid)
        {
            var viewModel = await _bulkUploadOrchestrator.GetUploadModel(providerid, hashedcommitmentid);
            return View(viewModel);
        }

        [HttpPost]
        [Route("{hashedCommitmentId}/UploadApprenticeships")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UploadApprenticeships(UploadApprenticeshipsViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _bulkUploadOrchestrator.UploadFile(CurrentUserId, model);

            if (result.HasFileLevelErrors)
            {
                var error = result.FileLevelErrors.FirstOrDefault();
                ModelState.AddModelError("Attachment", error?.Message);
                return View(model);
            }

            if (result.HasRowLevelErrors)
            {
                TempData[uploadErrorsTempDataKey] = result.RowLevelErrors;
                return RedirectToAction("UploadApprenticeshipsUnsuccessful", new { model.ProviderId, model.HashedCommitmentId });
            }
                
            // ToDo: Flash message, or other feedback to customer
            return RedirectToAction("Details", "Commitment", new { model.ProviderId, model.HashedCommitmentId });
        }


        [Route("{hashedCommitmentId}/UploadApprenticeships/Unsuccessful")]
        public async Task<ActionResult> UploadApprenticeshipsUnsuccessful(long providerId, string hashedCommitmentId)
        {
            var errors = ((IEnumerable<UploadError>)TempData[uploadErrorsTempDataKey])?.ToList()
                ?? new List<UploadError>();

            if (!errors.Any())
                return RedirectToAction("UploadApprenticeships", new { providerId, hashedCommitmentId });

            var model = await _bulkUploadOrchestrator.GetUnsuccessfulUpload(errors, providerId, hashedCommitmentId);

            return View(model);
        }
    }
}