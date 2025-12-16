using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC_Project.Services.Repositories.IRepository;
using System.Drawing.Printing;
using X.PagedList.Extensions;
using MVC_Project.Models.ViewModel;
using MVC_Project.Services.Repositories;
using Microsoft.AspNetCore.Authorization;


namespace MVC_Project.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<EmployeesController> _logger;
        private readonly IEmployeeRepository _emprepo;
        private readonly IDepartmentRepository _deptrepo;
        private readonly IAddressRepository _addrepo;
        private readonly HomeDashboardRepository _dashboardRepo;

        public HomeController(ILogger<EmployeesController> logger, IEmployeeRepository emprepo, IDepartmentRepository deptrepo, IAddressRepository addrepo, HomeDashboardRepository dashboardRepo)
        {
            _logger = logger;
            _emprepo = emprepo;
            _deptrepo = deptrepo;
            _addrepo = addrepo;
            _dashboardRepo = dashboardRepo;
        }

        
        public async Task<IActionResult> HomePage()
        {
            var recentEmployees = await _dashboardRepo.GetRecentEmployees();
            var (deptNames, projectCounts) = await _dashboardRepo.GetProjectCountByDepartment();

            var model = new HomeDashboardViewModel
            {
                EmployeeCount = await _dashboardRepo.GetEmployeeCount(),
                DepartmentCount = await _dashboardRepo.GetDepartmentCount(),
                ProjectCount = await _dashboardRepo.GetProjectCount(),
                RecentEmployees = recentEmployees,
                ProjectDepartmentNames = deptNames,
                ProjectCounts = projectCounts
            };

            return View(model);
        }



    }
}
