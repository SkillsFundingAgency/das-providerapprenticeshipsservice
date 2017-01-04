using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.BulkUpload;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Controllers
{
    [RoutePrefix("{providerId}/apprentices")]
    public class BulkUploadController : Controller
    {
        private readonly BulkUploadOrchestrator _bulkUploadController;

        public BulkUploadController(BulkUploadOrchestrator bulkUploadOrchestrator)
        {
            if (bulkUploadOrchestrator == null)
                throw new ArgumentNullException(nameof(bulkUploadOrchestrator));
            _bulkUploadController = bulkUploadOrchestrator;
        }

        [HttpGet]
        [Route("{hashedCommitmentId}/UploadApprenticeships")]
        public async Task<ActionResult> UploadApprenticeships(long providerid, string hashedcommitmentid)
        {
            var viewModel = await _bulkUploadController.GetUploadModel(providerid, hashedcommitmentid);
            return View(viewModel);
        }

        [HttpPost]
        [Route("{hashedCommitmentId}/UploadApprenticeships")]
        public async Task<ActionResult> UploadApprenticeships(UploadApprenticeshipsViewModel uploadApprenticeshipsViewModel)
        {
            var result = await _bulkUploadController.UploadFileAsync(uploadApprenticeshipsViewModel);
            if (!result.Errors.Any())
            {
                // ToDo: Flash message, or other feedback to customer
                return RedirectToAction("Details", "Commitment", new { uploadApprenticeshipsViewModel.ProviderId, uploadApprenticeshipsViewModel.HashedCommitmentId });
            }

            TempData["errors"] = result.Errors.ToList();
            return RedirectToAction("UploadApprenticeshipsUnsuccessful", new { uploadApprenticeshipsViewModel.ProviderId, uploadApprenticeshipsViewModel.HashedCommitmentId , errors = result.Errors });
        }


        [Route("{hashedCommitmentId}/UploadApprenticeships/Unsuccessful")]
        public ActionResult UploadApprenticeshipsUnsuccessful(long providerId, string hashedCommitmentId, IEnumerable<UploadError> errors)
        {
            var model = new UploadApprenticeshipsViewModel
                            {
                                ProviderId = providerId,
                                HashedCommitmentId = hashedCommitmentId,
                                Errors = TempData["errors"] as List<UploadError>
            };

            return View(model);
        }
    }
}