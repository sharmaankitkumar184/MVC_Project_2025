using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MVC_Project.Models.Models;
using MVC_Project.Services.Data;
using MVC_Project.Services.Repositories.IRepository;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace MVC_Project.Web.Controllers
{
    
    public class DepartmentsController : Controller
    {
        private readonly IDepartmentRepository _departmentRepository;
        private readonly ILogger<DepartmentsController> _logger;

        public DepartmentsController(ILogger<DepartmentsController> logger, IDepartmentRepository departmentRepository) 
        {
            _logger = logger;
            _departmentRepository = departmentRepository;
        }
        public async Task<IActionResult> Index()
        {
            IQueryable department_data = await _departmentRepository.GetAllDepartment();
            return View(department_data);
        }
        public async Task<IActionResult> Details(int? id)
        {
            var department_data=await _departmentRepository.GetDepartmentById(id);
            ViewBag.EmployeeCount = department_data.Employees.Count;
            ViewBag.ProjectCount = department_data.Projects.Count;
            return View(department_data);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Department dept)
        {
            if (!ModelState.IsValid)
            {
                return View(dept);
            }

            dept.CreatedAt = DateTime.Now;
            dept.UpdatedAt = DateTime.Now;

            await _departmentRepository.AddDepartment(dept);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var department = await _departmentRepository.GetDepartmentById(id);
            if (department == null)
            {
                return NotFound();
            }

            return View(department);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, Department department)
        {
            if (!ModelState.IsValid)
            {
                return View(department);
            }

            await _departmentRepository.EditDepartment(id, department);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
           
                var department = await _departmentRepository.GetDepartmentById(id);

                if (department == null)
                    return NotFound();
                if (department.Name == "Bench")
                {
                    throw new InvalidOperationException(
                        "Bench department cannot be deleted.");
                }


                await _departmentRepository.DeleteDepartment(id);

                TempData["Success"] = "Department deleted. Employees moved to Bench.";
                return RedirectToAction(nameof(Index));
          
        }

    }
}
