using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MVC_Project.Models.ViewModel;
using MVC_Project.Services.Repositories.IRepository;

namespace MVC_Project.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class BenchController : Controller
    {
        private readonly IBenchRepository _benchRepo;
        private readonly IDepartmentRepository _deptRepo;

        public BenchController(IBenchRepository benchRepo, IDepartmentRepository deptRepo)
        {
            _benchRepo = benchRepo;
            _deptRepo = deptRepo;
        }

        public async Task<IActionResult> Index()
        {
            var benchEmployees = await _benchRepo.GetBenchEmployees();
            return View(benchEmployees);
        }

        public async Task<IActionResult> Details(int? id)
        {
            var employee = await _benchRepo.GetBenchEmployeeById(id);
            if (employee == null) return NotFound();
            return View(employee);
        }

        [HttpGet]
        public async Task<IActionResult> Assign(int? id)
        {
            if (id == null) return NotFound();
            var employee = await _benchRepo.GetBenchEmployeeById(id);
            if (employee == null) return NotFound();

            var departments = await _deptRepo.GetAllDepartment();
            var deptList = departments
                .Where(d => d.IsActive && d.Name != "Bench")
                .Select(d => new SelectListItem
                {
                    Value = d.Id.ToString(),
                    Text = d.Name
                })
                .ToList();

            ViewBag.EmployeeName = employee.Name;
            ViewBag.DepartmentList = deptList;

            return View(new BenchAssignVm { EmployeeId = employee.Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Assign(BenchAssignVm vm)
        {
            if (!ModelState.IsValid)
            {
                var departments = await _deptRepo.GetAllDepartment();
                ViewBag.DepartmentList = departments
                    .Where(d => d.IsActive && d.Name != "Bench")
                    .Select(d => new SelectListItem { Value = d.Id.ToString(), Text = d.Name })
                    .ToList();
                return View(vm);
            }

            await _benchRepo.AssignEmployeeToDepartment(vm.EmployeeId, vm.DepartmentId);
            TempData["Success"] = "Employee assigned successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}

