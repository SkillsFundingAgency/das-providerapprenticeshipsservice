using System.Web.Mvc;

using SFA.DAS.ProviderApprenticeshipsService.Web.Models;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Controllers
{
    [RoutePrefix("{providerId}/apprentices")]
    public class BulkUploadController : Controller
    {
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
            // ToDo: sent to orchestrator
            // branch on returning model error count
            if (uploadApprenticeshipsViewModel.Attachment != null)
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