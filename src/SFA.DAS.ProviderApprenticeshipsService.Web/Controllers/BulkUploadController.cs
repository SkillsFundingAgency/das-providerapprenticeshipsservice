using System;
using System.Linq;
using System.Web.Mvc;

using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
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
        public ActionResult UploadApprenticeships(long providerid, string hashedcommitmentid)
        {
            var model = new UploadApprenticeshipsViewModel
            {
                ProviderId = providerid,
                HashedCommitmentId = hashedcommitmentid
            };
            return View(model);
        }

        [HttpPost]
        [Route("{hashedCommitmentId}/UploadApprenticeships")]
        public ActionResult UploadApprenticeships(UploadApprenticeshipsViewModel uploadApprenticeshipsViewModel)
        {
            var b = _bulkUploadController.UploadFile(uploadApprenticeshipsViewModel);
            // ToDo: sent to orchestrator
            // branch on returning model error count
            if (b.Any())
            {
                // ToDo: Flash message, or other feedback to customer
                return RedirectToAction("Details", "Commitment", new { uploadApprenticeshipsViewModel.ProviderId, uploadApprenticeshipsViewModel.HashedCommitmentId });
            }

            // ToDo: Need to return errors
            return RedirectToAction("UploadApprenticeshipsUnsuccessful", uploadApprenticeshipsViewModel);
        }


        [Route("{hashedCommitmentId}/UploadApprenticeships/Unsuccessful")]
        public ActionResult UploadApprenticeshipsUnsuccessful(long providerid, string hashedcommitmentid)
        {
            return View();
        }
    }
}