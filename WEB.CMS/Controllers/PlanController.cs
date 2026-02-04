using Entities.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Repositories.IRepositories;
using Utilities;
using WEB.CMS.Customize;
using WEB.CMS.Services;

namespace WEB.CMS.Controllers
{
    [CustomAuthorize]
    public class PlanController : Controller
    {
        private readonly IConfiguration _configuration;
    
        public PlanController( IConfiguration configuration)
        {
          
            _configuration = configuration;
          
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Search()
        {
           
            try
            {
               
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("Search - UserController: " + ex);
            }
            return PartialView();
        }
    }
}
