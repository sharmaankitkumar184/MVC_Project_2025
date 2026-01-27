namespace MVC_Project.Web.Models
{
    public class ErrorViewModel
    {
        public string Title { get; set; } = "Unexpected Error";
        public string Message { get; set; }
            = "Something went wrong. Please try again later.";

        public string? RequestId { get; set; }
    }

}
