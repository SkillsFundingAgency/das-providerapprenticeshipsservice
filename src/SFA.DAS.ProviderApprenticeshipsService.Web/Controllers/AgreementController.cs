using System;
using System.Threading.Tasks;
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
        private const string FileName = "organisations_and_agreements";
        private const string CsvContentType = "text/csv";
        private const string ExcelContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

        public AgreementController(AgreementOrchestrator orchestrator, ICookieStorageService<FlashMessageViewModel> flashMessage) : base(flashMessage)
        {
            _orchestrator = orchestrator;
        }

        [HttpGet]
        [Route("")]
        public async Task<ActionResult> Agreements(long providerId)
        {
            var model = await _orchestrator.GetAgreementsViewModel(providerId);
            return View(model);
        }

        [HttpGet]
        [Route("download-csv")]
        public async Task<FileResult> DownloadCsv(long providerId)
        {
            var fileContents = await _orchestrator.GetAgreementsAsCsv(providerId);
            return File(fileContents, CsvContentType, $"{FileName}_{DateTime.Now:s}.csv");
        }

        [HttpGet]
        [Route("download-excel")]
        public async Task<FileResult> DownloadExcel(long providerId)
        {
            var fileContents = await _orchestrator.GetAgreementsAsExcel(providerId);
            return File(fileContents, ExcelContentType, $"{FileName}_{DateTime.Now:s}.xlsx"); 
        }
    }
}