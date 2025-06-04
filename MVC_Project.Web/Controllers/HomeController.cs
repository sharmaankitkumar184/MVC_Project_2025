using Microsoft.AspNetCore.Mvc;

namespace MVC_Project.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Homepage()
        {
            return View();
        }
    }
}
