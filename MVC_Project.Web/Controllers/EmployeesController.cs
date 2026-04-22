using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVC_Project.Models.Models;
using MVC_Project.Models.ViewModel;
using MVC_Project.Services.Repositories.IRepository;
using System.Data;
using System.Drawing.Printing;
using System.Security.Claims;
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

            //For role based data
            var loggedInRole = User.FindFirst(ClaimTypes.Role)?.Value;

            var employeesQuery = await _emprepo.GetAllEmployee(); // Await the result

            // 🔥 Filter BEFORE pagination
            if (loggedInRole == "Manager")
            {
                employeesQuery = employeesQuery
                    .Where(e => e.User.Role == UserRole.Employee);
            }
            else if (loggedInRole == "Admin")
            {
                employeesQuery = employeesQuery
                    .Where(e => e.User.Role == UserRole.Employee ||
                                e.User.Role == UserRole.Manager ||
                                e.User.Role == UserRole.Admin);
            }
            // THEN paginate
            var employees = employeesQuery
                .OrderBy(e => e.Id)
                .ToPagedList(pageNumber, pageSize);

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
            var model = new EmployeeUserVm
            {
                DateOfBirth = DateTime.Today.AddYears(-18),
                address = new Address()
            };

            return View(model);
        }

        // POST: Employees/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Email,Phone,DateOfBirth,Gender,Designation,DepartmentId,AddressId")] EmployeeUserVm employee)
        {
            // ✅ Remove nested validation properly
            ModelState.Remove("Salary");
            ModelState.Remove("EmployeeCode");
            ModelState.Remove("address");
            ModelState.Remove("ProfileImagePath");

            if (!ModelState.IsValid)
            {
                // 🔥 MUST re-populate dropdowns
                ViewBag.Departments = new SelectList(
                    await _deptrepo.GetAllDepartment(), "Id", "Name");

                ViewBag.Addresses = new SelectList(
                    await _addrepo.GetAllAddress(), "Id", "Street");

                return View(employee);
            }
            if (await _emprepo.EmailAvailable(employee.Email))
            {
                ModelState.AddModelError("Email", "Employee email already exists.");
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
            bool isAdmin = User.IsInRole("Admin");
            if (id != employee.Id)
            {
                return NotFound();
            }

            // Remove Salary from the model state validation if needed
            ModelState.Remove("Salary");
            ModelState.Remove("address");
            ModelState.Remove("ProfileImagePath");
            ModelState.Remove("EmployeeCode");
            if (ModelState.IsValid)
            { 
                ViewBag.Departments = new SelectList(_deptrepo.GetAllDepartment().Result, "Id", "Name");
                ViewBag.Addresses = new SelectList(_addrepo.GetAllAddress().Result, "Id", "Street");
                var existingEmployee = await _emprepo.GetEmployeeById(id);
                if (isAdmin && existingEmployee.User.Role == UserRole.Admin && employee.Role != UserRole.Admin)
                {
                    var adminCount = await _userrepo.adminCountAsync();

                    if (adminCount <= 1)
                    {
                        ModelState.AddModelError(string.Empty,
                            "System must have at least one Admin.");
                        return View(employee);
                    }
                }
                await _emprepo.EditEmployee(id, employee, isAdmin);
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Edit));
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
            var employee = await _emprepo.GetEmployeeById(id);

            if (employee == null)
                return NotFound();

            var currentUserId = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier).Value);

            //Prevent self delete
            if (employee.UserId == currentUserId)
            {
                ModelState.AddModelError(string.Empty,
                    "You cannot delete your own account.");

                return View(employee);
            }

            //Prevent deleting last admin
            if (employee.User.Role == UserRole.Admin)
            {
                var adminCount = await _userrepo.adminCountAsync();

                if (adminCount <= 1)
                {
                    ModelState.AddModelError(string.Empty,
                        "System must have at least one Admin.");

                    return View(employee);
                }
            }

            await _emprepo.DeleteEmployee(id);

            return RedirectToAction(nameof(Index));

        }

        // POST: Employees/Create
        [HttpGet, HttpPost]
        [ValidateAntiForgeryToken]
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
