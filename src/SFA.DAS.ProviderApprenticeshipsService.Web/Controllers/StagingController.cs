using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using SFA.DAS.ProviderApprenticeshipsService.Application;
using SFA.DAS.ProviderApprenticeshipsService.Web.Attributes;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Types;
using System.Security.Claims;
using SFA.DAS.NLog.Logger;
using SFA.DAS.ProviderApprenticeshipsService.Application.Domain.Commitment;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Controllers
{
    [Authorize]
    [ProviderUkPrnCheck]
    [RoutePrefix("{providerId}/staging")]
    public class StagingController : BaseController
    {
        private const string LastCohortPageCookieKey = "sfa-das-providerapprenticeshipsservice-lastCohortPage";
        private readonly ICookieStorageService<string> _lastCohortCookieStorageService;

        private readonly CommitmentOrchestrator _commitmentOrchestrator;
        private readonly ILog _logger;
    
        public StagingController(CommitmentOrchestrator commitmentOrchestrator, ILog logger, ICookieStorageService<FlashMessageViewModel> flashMessage, ICookieStorageService<string> lastCohortCookieStorageService) : base(flashMessage)
        {
            _commitmentOrchestrator = commitmentOrchestrator;
            _logger = logger;
            _lastCohortCookieStorageService = lastCohortCookieStorageService;
        }

        
        [HttpGet]
        [Route("")]
        [OutputCache(CacheProfile = "NoCache")]
        public ActionResult Index(long providerId)
        {
            return View();
        }

        [HttpGet]
        [Route("upload")]
        [OutputCache(CacheProfile = "NoCache")]
        public ActionResult Upload(long providerId)
        {
            return View();
        }

        [HttpGet]
        [Route("upload-done")]
        [OutputCache(CacheProfile = "NoCache")]
        public ActionResult UploadDone(long providerId)
        {
            return View();
        }


        [HttpGet]
        [Route("select-reservation")]
        [OutputCache(CacheProfile = "NoCache")]
        public ActionResult SelectReservation(long providerId)
        {
            return View();
        }

        [HttpGet]
        [OutputCache(CacheProfile = "NoCache")]
        [Route("{uploadId}")]
        public ActionResult Details(long providerId, long uploadId)
        {
            var model = StagingData.StagedBulkUploads.Single(x => x.Id == uploadId);
            return View(model);
        }



    }

    public static class StagingData
    {
        public static List<StagedBulkUpload> StagedBulkUploads { get; set; }

        static StagingData()
        {
            StagedBulkUploads = new List<StagedBulkUpload>
            {
                new StagedBulkUpload
                {
                    Id = 0,
                    Created = "15 Mar 2019",
                    Employer = "Rapid Logistics",
                    FileName = "rapid1.csv",
                    IsStructurallyValid = false
                },
                new StagedBulkUpload
                {
                    Id = 4,
                    Employer = "Rapid Logistics",
                    FileName = "rapid2.csv",
                    Created = "19 Mar 2019",
                    StagedBulkUploadRows = new List<StagedBulkUploadRow>
                    {
                        new StagedBulkUploadRow
                        {
                            FirstName = "Lucy",
                            LastName = "Harper",
                            Uln = "1234567890",
                            Course = "",
                            IsValid = false,
                            StartDate = "Sep 2019"
                        }
                    },
                    AutoReservation = true
                },
                new StagedBulkUpload
                {
                    Id = 1,
                    Employer = "Rapid Logistics",
                    FileName = "rapid3.csv",
                    Created = "19 Mar 2019",
                    StagedBulkUploadRows = new List<StagedBulkUploadRow>
                    {
                        new StagedBulkUploadRow
                        {
                            FirstName = "Thomas",
                            LastName = "Johnson",
                            Uln = "1234567890",
                            Course = "123 - Engineering",
                            StartDate = "Sep 2019",
                            Reservation = "Engineering Aug-Oct 2019"
                        },
                        new StagedBulkUploadRow
                        {
                            FirstName = "Smith",
                            LastName = "James",
                            Uln = "2345678901",
                            Course = "456 - Animal Husbandry",
                            StartDate = "Nov 2019"
                        }
                    }
                },
                new StagedBulkUpload
                {
                    Id = 2,
                    Employer = "Rapid Logistics",
                    FileName = "rapid4.csv",
                    Created = "19 Mar 2019",
                    StagedBulkUploadRows = new List<StagedBulkUploadRow>
                    {
                        new StagedBulkUploadRow
                        {
                            FirstName = "Leo",
                            LastName = "Faulkner",
                            Uln = "1234567890",
                            Course = "123 - Engineering",
                            StartDate = "Sep 2019",
                            Reservation = "Engineering Aug-Oct 2019"
                        },
                        new StagedBulkUploadRow
                        {
                            FirstName = "James",
                            LastName = "Everson",
                            Uln = "2345678901",
                            Course = "123 - Engineering",
                            StartDate = "Sep 2019",
                            Reservation = "Engineering Aug-Oct 2019"
                        }
                    }
                },
                new StagedBulkUpload
                {
                    Id = 3,
                    Employer = "Mega Corp Bank",
                    FileName = "megacorp1.csv",
                    Created = "19 Mar 2019",
                    StagedBulkUploadRows = new List<StagedBulkUploadRow>
                    {
                        new StagedBulkUploadRow
                        {
                            FirstName = "Lucy",
                            LastName = "Harper",
                            Uln = "1234567890",
                            Course = "123 - Engineering",
                            StartDate = "Sep 2019"
                        },
                        new StagedBulkUploadRow
                        {
                            FirstName = "Amy",
                            LastName = "Reid",
                            Uln = "2345678901",
                            Course = "456 - Animal Husbandry",
                            StartDate = "Nov 2019"
                        }
                    },
                    AutoReservation = true
                }
            };

        }
        
    }

    public class StagedBulkUpload
    {
        public StagedBulkUpload()
        {
            IsStructurallyValid = true;
        }

        public long Id { get; set; }
        public string Employer { get; set; }
        public string FileName { get; set; }
        public string Created { get; set; } 
        public List<StagedBulkUploadRow> StagedBulkUploadRows { get; set; }
        public int Rows { get => StagedBulkUploadRows.Count; }
        public bool AutoReservation { get;set; }
        public bool IsStructurallyValid { get; set; }
    }

    public class StagedBulkUploadRow
    {
        public StagedBulkUploadRow()
        {
            IsValid = true;
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Uln { get; set; }
        public string Course { get; set; }
        public string StartDate { get; set; }
        public bool IsValid { get; set; }
        public string Reservation { get; set; }
        public bool HasReservation { get => !String.IsNullOrWhiteSpace(Reservation); }
    }
}