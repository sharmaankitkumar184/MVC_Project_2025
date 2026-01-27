using MVC_Project.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVC_Project.Services.Repositories.IRepository
{
     public interface IDepartmentRepository
    {
        Task<IQueryable<Department>> GetAllDepartment();
        Task<Department?> GetDepartmentById(int? id);
        Task<Department> AddDepartment(Department department);
        Task<Department> EditDepartment(int? id, Department updatedDepartment);
        Task<Department> DeleteDepartment(int? id);

        Task<Department> GetBenchDepartmentAsync();

        Task ReassignEmployeesToBenchAsync(int? departmentId);
    }
}
