using MVC_Project.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVC_Project.Services.Repositories.IRepository
{
    public interface IEmployeeRepository
    {

        Task<IQueryable<Employee>> GetAllEmployee();
        Task<Employee?> GetEmployeeById(int? id);
        Task<Employee> AddEmployee(Employee employee);
        Task<Employee> EditEmployee(int? id,Employee employee);
        Task<Employee> DeleteEmployee(int? id);
        Task<IQueryable<Employee>?> SearchEmployee(string empName);
    }
}
