using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MVC_Project.Models.Models;
using MVC_Project.Services.Repositories.IRepository;

namespace MVC_Project.Web.Controllers
{
    [Authorize(Roles = "Admin,Manager")]
    public class ProjectsController : Controller
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly ILogger<ProjectsController> _logger;

        public ProjectsController(
            ILogger<ProjectsController> logger,
            IProjectRepository projectRepository,
            IDepartmentRepository departmentRepository)
        {
            _logger = logger;
            _projectRepository = projectRepository;
            _departmentRepository = departmentRepository;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                IQueryable projectData = await _projectRepository.GetAllProjects();
                return View(projectData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving projects");
                return View(new List<Project>());
            }
        }

        public async Task<IActionResult> Details(int? id)
        {
            try
            {
                if (id == null)
                    return NotFound();

                var projectData = await _projectRepository.GetProjectById(id);
                if (projectData == null)
                    return NotFound();

                ViewBag.EmployeeCount = projectData.AssignedEmployees?.Count ?? 0;
                return View(projectData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving project details");
                return NotFound();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            try
            {
                var departments = await _departmentRepository.GetAllDepartment();
                ViewBag.Departments = new SelectList(departments, "Id", "Name");
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading create form");
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Project project)
        {
            try
            {
                ModelState.Remove("Department");
                if (!ModelState.IsValid)
                {
                    var departments = await _departmentRepository.GetAllDepartment();
                    ViewBag.Departments = new SelectList(departments, "Id", "Name");
                    return View(project);
                }

                await _projectRepository.AddProject(project);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating project");
                var departments = await _departmentRepository.GetAllDepartment();
                ViewBag.Departments = new SelectList(departments, "Id", "Name");
                ModelState.AddModelError("", "Error creating project. Please try again.");
                return View(project);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            try
            {
                if (id == null)
                    return NotFound();

                var project = await _projectRepository.GetProjectById(id);
                if (project == null)
                    return NotFound();

                var departments = await _departmentRepository.GetAllDepartment();
                ViewBag.Departments = new SelectList(departments, "Id", "Name", project.DepartmentId);
                return View(project);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading edit form");
                return NotFound();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, Project project)
        {
            try
            {
                // Remove Salary from the model state validation if needed
                ModelState.Remove("Department");
                if (!ModelState.IsValid)
                {
                    var departments = await _departmentRepository.GetAllDepartment();
                    ViewBag.Departments = new SelectList(departments, "Id", "Name");
                    return View(project);
                }

                var result = await _projectRepository.EditProject(id, project);
                if (result == null)
                    return NotFound();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating project");
                var departments = await _departmentRepository.GetAllDepartment();
                ViewBag.Departments = new SelectList(departments, "Id", "Name");
                ModelState.AddModelError("", "Error updating project. Please try again.");
                return View(project);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var project = await _projectRepository.GetProjectById(id);

                if (project == null)
                    return NotFound();

                await _projectRepository.DeleteProject(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting project");
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public async Task<IActionResult> Active()
        {
            try
            {
                IQueryable projectData = await _projectRepository.GetActiveProjects();
                return View("Index", projectData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving active projects");
                return View("Index", new List<Project>());
            }
        }
    }
}
