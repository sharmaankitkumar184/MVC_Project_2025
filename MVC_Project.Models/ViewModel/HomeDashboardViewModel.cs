using MVC_Project.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVC_Project.Models.ViewModel
{
    public class HomeDashboardViewModel
    {
        public int EmployeeCount { get; set; }
        public int DepartmentCount { get; set; }
        public int ProjectCount { get; set; }

        public List<Employee> RecentEmployees { get; set; } = new();
        public List<string> ProjectDepartmentNames { get; set; } = new();
        public List<int> ProjectCounts { get; set; } = new();
    }
}
