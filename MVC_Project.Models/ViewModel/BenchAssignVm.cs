using System.ComponentModel.DataAnnotations;

namespace MVC_Project.Models.ViewModel
{
    public class BenchAssignVm
    {
        [Required]
        public int EmployeeId { get; set; }

        [Required]
        public int DepartmentId { get; set; }
    }
}

