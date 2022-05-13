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
        private readonly IProviderCommitmentsLogger _logger;
        private readonly ProviderUrlHelper.ILinkGenerator _providerUrlhelper;


        public BulkUploadController(BulkUploadOrchestrator bulkUploadOrchestrator, ICookieStorageService<FlashMessageViewModel> flashMessage,
            IProviderCommitmentsLogger logger,
            ProviderUrlHelper.LinkGenerator providerUrlhelper) : base(flashMessage)
        {
            _bulkUploadOrchestrator = bulkUploadOrchestrator;
            _logger = logger;
            _providerUrlhelper = providerUrlhelper;
        }

        [HttpGet]
        [OutputCache(CacheProfile = "NoCache")]
        [Route("{hashedCommitmentId}/UploadApprenticeships")]
        [Deprecated]
        public ActionResult UploadApprenticeships(long providerid, string hashedcommitmentid)
        {   
            _logger.Info($"To track V1 Bulk Upload (UploadApprenticeships) UrlReferrer Request: {HttpContext.Request.UrlReferrer} Request to Page: {HttpContext.Request.RawUrl}");
            return Redirect(_providerUrlhelper.ProviderCommitmentsLink($"{providerid}/unapproved/add/file-upload/start"));            
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
                return RedirectToAction("UploadApprenticeshipsUnsuccessful", new { model.ProviderId, model.HashedCommitmentId, result.BulkUploadReference, model.BlackListed });
            }
                
            // ToDo: Flash message, or other feedback to customer
            return RedirectToAction("Details", "Commitment", new { model.ProviderId, model.HashedCommitmentId });
        }

        [Route("{hashedCommitmentId}/UploadApprenticeships/Unsuccessful/{bulkUploadReference}")]
        [Deprecated]
        public ActionResult UploadApprenticeshipsUnsuccessful(long providerId, string hashedCommitmentId, string bulkUploadReference, bool blackListed)
        {   
            _logger.Info($"To track V1 Bulk Upload (UploadApprenticeshipsUnsuccessful) UrlReferrer Request: {HttpContext.Request.UrlReferrer} Request to Page: {HttpContext.Request.RawUrl}");
            return Redirect(_providerUrlhelper.ProviderCommitmentsLink($"{providerId}/unapproved/add/file-upload/start"));         
        }
    }
}