using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MVC_Project.Web.Controllers
{
    [Authorize(Roles = "Manager")]
    public class ManagerDashboardController : Controller
    {
        public IActionResult MyTeam()
        {
            // fetch employees where ManagerId == current user
            return View();
        }
    }
}
