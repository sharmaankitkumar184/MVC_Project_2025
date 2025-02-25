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
        Task<IEnumerable<Department>> GetAllDepartment();
        Task<Department?> GetDepartmentById(int? id);
        Task<Department> AddDepartment(Department department);
        Task<Department> EditDepartment(int? id, Department department);
        Task<Department> DeleteDepartment(int? id);
    }
}
