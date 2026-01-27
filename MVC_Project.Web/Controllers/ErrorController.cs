using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MVC_Project.Web.Models;

namespace MVC_Project.Web.Controllers
{
    public class ErrorController : Controller
    {
        [Route("Error")]
        public IActionResult Index()
        {
            var exception = HttpContext.Features
                .Get<IExceptionHandlerFeature>()?.Error;

            var model = new ErrorViewModel
            {
                RequestId = HttpContext.TraceIdentifier
            };

            switch (exception)
            {
                case InvalidOperationException:
                    model.Title = "Action Not Allowed";
                    model.Message = exception.Message;
                    break;

                case UnauthorizedAccessException:
                    model.Title = "Access Denied";
                    model.Message = "You do not have permission to perform this action.";
                    break;

                default:
                    model.Title = "Unexpected Error";
                    model.Message = "An unexpected error occurred. Please try again later.";
                    break;
            }

            return View("~/Views/Shared/Error.cshtml", model);
        }
    }

}
