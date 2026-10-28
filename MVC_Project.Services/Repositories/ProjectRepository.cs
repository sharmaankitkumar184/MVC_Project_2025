using Microsoft.EntityFrameworkCore;
using MVC_Project.Models.Models;
using MVC_Project.Services.Data;
using MVC_Project.Services.Repositories.IRepository;

namespace MVC_Project.Services.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly ApplicationDbContext _db;

        public ProjectRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IQueryable<Project>> GetAllProjects()
        {
            return _db.Projects.Include(p => p.Department).Include(p => p.AssignedEmployees);
        }

        public async Task<Project?> GetProjectById(int? id)
        {
            if (id == 0 || id == null)
            {
                throw new ArgumentNullException(nameof(id), "Project Id cannot be null");
            }
            var projectData = _db.Projects
                .Include(p => p.Department)
                .Include(p => p.AssignedEmployees)
                .FirstOrDefault(i => i.Id == id);

            return projectData;
        }

        public async Task<Project> AddProject(Project project)
        {
            if (project == null) 
                throw new ArgumentNullException(nameof(project), "Project cannot be null");

            project.CreatedAt = DateTime.Now;
            project.UpdatedAt = DateTime.UtcNow;

            _db.Projects.Add(project);
            await _db.SaveChangesAsync();
            return project;
        }

        public async Task<Project?> EditProject(int? id, Project updatedProject)
        {
            if (updatedProject == null)
                throw new ArgumentNullException(nameof(updatedProject));

            var project = await _db.Projects.FindAsync(id);

            if (project == null)
                return null;

            project.Name = updatedProject.Name;
            project.Code = updatedProject.Code;
            project.Description = updatedProject.Description;
            project.StartDate = updatedProject.StartDate;
            project.EndDate = updatedProject.EndDate;
            project.Status = updatedProject.Status;
            project.ProgressPercentage = updatedProject.ProgressPercentage;
            project.Budget = updatedProject.Budget;
            project.CostIncurred = updatedProject.CostIncurred;
            project.DepartmentId = updatedProject.DepartmentId;
            project.IsActive = updatedProject.IsActive;
            project.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            return project;
        }

        public async Task<Project?> DeleteProject(int? id)
        {
            if (id == null) 
                throw new ArgumentNullException("Project ID cannot be null");

            var projectData = await _db.Projects.FirstOrDefaultAsync(i => i.Id == id);
            if (projectData != null)
            {
                _db.Projects.Remove(projectData);
            }
            await _db.SaveChangesAsync();
            return projectData;
        }

        public async Task<IQueryable<Project>> GetProjectsByDepartmentId(int? departmentId)
        {
            if (departmentId == null)
                throw new ArgumentNullException(nameof(departmentId));

            return _db.Projects
                .Where(p => p.DepartmentId == departmentId)
                .Include(p => p.Department)
                .Include(p => p.AssignedEmployees);
        }

        public async Task<IQueryable<Project>> GetActiveProjects()
        {
            return _db.Projects
                .Where(p => p.IsActive && p.Status != "Completed")
                .Include(p => p.Department)
                .Include(p => p.AssignedEmployees);
        }
    }
}
