using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MVC_Project.Models.Models;
using MVC_Project.Services.Repositories.IRepository;
using System.Threading.Tasks;

namespace MVC_Project.Web.Controllers
{
    [Authorize(Roles = "Admin,Manager")]
    public class SalariesController : Controller
    {
        private readonly ISalaryRepository _salaryRepo;
        private readonly IEmployeeRepository _employeeRepo;

        public SalariesController(ISalaryRepository salaryRepo, IEmployeeRepository employeeRepo)
        {
            _salaryRepo = salaryRepo;
            _employeeRepo = employeeRepo;
        }

        public async Task<IActionResult> Index()
        {
            var salaries = await _salaryRepo.GetAllSalaries();
            return View(salaries);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var salary = await _salaryRepo.GetSalaryById(id);
            if (salary == null) return NotFound();
            return View(salary);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Employees = await _employeeRepo.GetAllEmployee();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Salaries salary)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Employees = await _employeeRepo.GetAllEmployee();
                return View(salary);
            }
            await _salaryRepo.AddSalary(salary);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var salary = await _salaryRepo.GetSalaryById(id);
            if (salary == null) return NotFound();
            ViewBag.Employees = await _employeeRepo.GetAllEmployee();
            return View(salary);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Salaries salary)
        {
            if (id != salary.Id) return NotFound();
            if (!ModelState.IsValid)
            {
                ViewBag.Employees = await _employeeRepo.GetAllEmployee();
                return View(salary);
            }
            await _salaryRepo.EditSalary(id, salary);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var salary = await _salaryRepo.GetSalaryById(id);
            if (salary == null) return NotFound();
            return View(salary);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _salaryRepo.DeleteSalary(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
