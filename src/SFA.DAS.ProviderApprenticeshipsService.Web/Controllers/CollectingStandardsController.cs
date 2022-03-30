using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Types;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Controllers
{
    public class CollectingStandardsController : BaseController
    {

        public CollectingStandardsController(ICookieStorageService<FlashMessageViewModel> flashMessage) : base(flashMessage)
        {
        }


        [Authorize]
        [Route("~/Check-your-record", Name = "Check-your-record")]
        public ActionResult Index()
        {

            return View();
        }

    }
}
