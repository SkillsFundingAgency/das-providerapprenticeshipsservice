using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using ClosedXML.Excel;
using ClosedXML.Extensions;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Web.Attributes;
using SFA.DAS.ProviderApprenticeshipsService.Web.Helpers;
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
        private const string FileName = "organisations_and_areements";
        
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
            var model = await _orchestrator.GetAgreementsViewModel(providerId);

            return CsvWriterFacade.Deliver(model.CommitmentAgreements, $"{FileName}_{DateTime.Now:s}.csv");
        }

        [HttpGet]
        [Route("download-excel")]
        public async Task<FileResult> DownloadExcel(long providerId)
        {
            var model = await _orchestrator.GetAgreementsViewModel(providerId);

            var workbook = new XLWorkbook();
            workbook.AddWorksheet(model.ToDataTable() ,"Agreements");

            return workbook.Deliver($"{FileName}_{DateTime.Now:s}.xlsx"); 
        }
    }
}