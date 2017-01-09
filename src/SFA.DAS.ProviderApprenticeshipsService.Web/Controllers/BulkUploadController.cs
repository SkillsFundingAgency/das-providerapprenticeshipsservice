using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

using SFA.DAS.ProviderApprenticeshipsService.Web.Exceptions;
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
        [OutputCache(CacheProfile = "NoCache")]
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
            if (!ModelState.IsValid)
            {
                return View(uploadApprenticeshipsViewModel);
            }

            var result = await _bulkUploadController.UploadFileAsync(uploadApprenticeshipsViewModel);
            if (!result.Errors.Any())
            {
                // ToDo: Flash message, or other feedback to customer
                return RedirectToAction("Details", "Commitment", new { uploadApprenticeshipsViewModel.ProviderId, uploadApprenticeshipsViewModel.HashedCommitmentId });
            }

            return RedirectToAction("UploadApprenticeshipsUnsuccessful", new { uploadApprenticeshipsViewModel.ProviderId, uploadApprenticeshipsViewModel.HashedCommitmentId , errorCount = result.Errors.Count() });
        }

        [Route("{hashedCommitmentId}/UploadApprenticeships/Unsuccessful")]
        public ActionResult UploadApprenticeshipsUnsuccessful(long providerId, string hashedCommitmentId, int errorCount)
        {
            var model = new UploadApprenticeshipsViewModel
                            {
                                ProviderId = providerId,
                                HashedCommitmentId = hashedCommitmentId,
                                ErrorCount = errorCount
            };

            return View(model);
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            if (filterContext.Exception is InvalidStateException)
            {
                filterContext.ExceptionHandled = true;
                filterContext.Result = RedirectToAction("Index", "Account",
                    new { message = "You have been redirected from a page that is no longer accessible" });

            }
        }
    }
}