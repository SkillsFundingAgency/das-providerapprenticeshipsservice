using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Web.Attributes;
using SFA.DAS.ProviderApprenticeshipsService.Web.Authentication;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Types;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Controllers
{
    [ProviderUkPrnCheck]
    [RoleAuthorize(Roles = nameof(RoleNames.HasContributorOrAbovePermission))]
    [RoutePrefix("{providerId}/apprentices")]
    public class BulkUploadController : BaseController
    {
        private readonly BulkUploadOrchestrator _bulkUploadOrchestrator;

        public BulkUploadController(BulkUploadOrchestrator bulkUploadOrchestrator, ICookieStorageService<FlashMessageViewModel> flashMessage) : base(flashMessage)
        {
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

            var result = await _bulkUploadOrchestrator.UploadFile(CurrentUserId, model, GetSignedInUser());

            if (result.HasFileLevelErrors)
            {
                var error = result.FileLevelErrors.FirstOrDefault();
                ModelState.AddModelError("Attachment", error?.Message);
                return View(model);
            }

            if (result.HasRowLevelErrors)
            {
                return RedirectToAction("UploadApprenticeshipsUnsuccessful", new { model.ProviderId, model.HashedCommitmentId, result.BulkUploadReference });
            }
                
            // ToDo: Flash message, or other feedback to customer
            return RedirectToAction("Details", "Commitment", new { model.ProviderId, model.HashedCommitmentId });
        }

        [Route("{hashedCommitmentId}/UploadApprenticeships/Unsuccessful/{bulkUploadReference}")]
        public async Task<ActionResult> UploadApprenticeshipsUnsuccessful(long providerId, string hashedCommitmentId, string bulkUploadReference)
        {
            var model = await _bulkUploadOrchestrator.GetUnsuccessfulUpload(providerId, hashedCommitmentId, bulkUploadReference);
            return View(model);
        }
    }
}