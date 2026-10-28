using MVC_Project.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVC_Project.Services.Repositories.IRepository
{
    public interface IProjectRepository
    {
        Task<IQueryable<Project>> GetAllProjects();
        Task<Project?> GetProjectById(int? id);
        Task<Project> AddProject(Project project);
        Task<Project?> EditProject(int? id, Project updatedProject);
        Task<Project?> DeleteProject(int? id);
        Task<IQueryable<Project>> GetProjectsByDepartmentId(int? departmentId);
        Task<IQueryable<Project>> GetActiveProjects();
    }
}
