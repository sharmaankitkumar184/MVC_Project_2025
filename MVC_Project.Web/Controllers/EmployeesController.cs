using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVC_Project.Models.Models;
using MVC_Project.Models.ViewModel;
using MVC_Project.Services.Repositories.IRepository;
using System.Data;
using System.Drawing.Printing;
using X.PagedList.Extensions;

namespace MVC_Project.Web.Controllers
{
    [Authorize(Roles= "Admin ,Manager")]
    public class EmployeesController : Controller
    {
        private readonly ILogger<EmployeesController> _logger;
        private readonly IEmployeeRepository _emprepo;
        private readonly IDepartmentRepository _deptrepo;
        private readonly IAddressRepository _addrepo;
        private readonly IUserRepository _userrepo;
        public EmployeesController(ILogger<EmployeesController> logger,IEmployeeRepository emprepo, IDepartmentRepository deptrepo, IAddressRepository addrepo, IUserRepository userrepo)
        {
            _logger = logger;
            _emprepo = emprepo;
            _deptrepo = deptrepo;
            _addrepo = addrepo;
            _userrepo = userrepo;
        }
        
        // GET: Employees
        public async Task<IActionResult> Index(int? page, int pageSize = 9)
        {

            int pageNumber = page ?? 1; // Default to first page

            var employeesQuery = await _emprepo.GetAllEmployee(); // Await the result

            var employees = employeesQuery
                .OrderBy(e => e.Id)
                .ToPagedList(pageNumber, pageSize); // Use ToPagedList (not async, since it's already awaited)
            ViewBag.PageSize = pageSize; // Pass page size to the view
            return View(employees);
        }

        // GET: Employees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _emprepo.GetEmployeeById((int)id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // GET: Employees/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Departments = new SelectList(_deptrepo.GetAllDepartment().Result, "Id", "Name");
            ViewBag.Addresses = new SelectList(_addrepo.GetAllAddress().Result, "Id", "Street");
            return View();
        }

        // POST: Employees/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,EmployeeCode,Email,Phone,DateOfBirth,Gender,Designation,DepartmentId,AddressId")] Employee employee)
        {
            // ✅ Remove nested validation properly
            ModelState.Remove("Salary");

            if (!ModelState.IsValid)
            {
                // 🔥 MUST re-populate dropdowns
                ViewBag.Departments = new SelectList(
                    await _deptrepo.GetAllDepartment(), "Id", "Name");

                ViewBag.Addresses = new SelectList(
                    await _addrepo.GetAllAddress(), "Id", "Street");

                return View(employee);
            }

            // ✅ Save logic
            await _emprepo.AddEmployee(employee);

            return RedirectToAction(nameof(Index));
        }


        // GET: Employees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            ViewBag.Departments = new SelectList(_deptrepo.GetAllDepartment().Result, "Id", "Name");
            ViewBag.Addresses = new SelectList(_addrepo.GetAllAddress().Result, "Id", "Street");
            var employee = await _emprepo.GetEmployeeById(id);
            if (employee == null)
            {
                return NotFound();
            }
            // 🔥 MAP ENTITY → VIEWMODEL
            var EmployeeVmmodel = new EmployeeUserVm
            {
                Id = employee.Id,
                Name = employee.Name,
                EmployeeCode = employee.EmployeeCode,
                Email = employee.Email,
                Phone = employee.Phone,
                DateOfBirth = employee.DateOfBirth,
                Gender = employee.Gender,
                Designation=employee.Designation,
                DepartmentId = employee.DepartmentId,
                AddressId = employee.AddressId,

                // User field
                Role = employee.User.Role
            };
            return View(EmployeeVmmodel);
        }

        // POST: Employees/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,EmployeeCode,Email,Phone,DateOfBirth,Gender,Designation,DepartmentId,AddressId,Role")] EmployeeUserVm employee)
        {

            if (id != employee.Id)
            {
                return NotFound();
            }

            // Remove Salary from the model state validation if needed
            ModelState.Remove("Salary");

            if (ModelState.IsValid)
            {
                await _emprepo.EditEmployee(id,employee);
                return RedirectToAction(nameof(Index));
            }
            return View(employee);
        }

        //GET: Employees/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _emprepo.GetEmployeeById(id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _emprepo.DeleteEmployee(id);
            return RedirectToAction(nameof(Index));
        }

        // POST: Employees/Create
        [HttpGet, HttpPost]
        public async Task<IActionResult> Search(int? page, string empName, int pageSize = 9)
        {
            if (ModelState.IsValid)
            {
                int pageNumber = page ?? 1; // Default to first page
                ViewBag.SearchQuery = empName;

                var searchedEmployee = await _emprepo.SearchEmployee(empName);

                if(searchedEmployee == null)
                {
                    return NotFound();
                }
                var pagedResult = searchedEmployee.OrderBy(e => e.Id).ToPagedList(pageNumber, pageSize);

                ViewBag.PageSize = pageSize;

                return View("Index", pagedResult); // ✅ Important: pass model here
            }

            return RedirectToAction(nameof(Index));
        }


    }
}
