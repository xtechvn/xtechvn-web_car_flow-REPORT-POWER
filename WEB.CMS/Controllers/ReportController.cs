using Microsoft.AspNetCore.Mvc;
using WEB.CMS.Customize;

namespace WEB.CMS.Controllers
{
    [CustomAuthorize]
    public class ReportController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
