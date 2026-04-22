using MVC_Project.Models.Models;

namespace MVC_Project.Services.Repositories.IRepository
{
    public interface IBenchRepository
    {
        Task<IQueryable<Employee>> GetBenchEmployees();
        Task<Employee?> GetBenchEmployeeById(int? id);
        Task AssignEmployeeToDepartment(int employeeId, int departmentId);
    }
}

