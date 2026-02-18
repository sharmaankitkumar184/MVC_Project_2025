using MVC_Project.Models.Models;
using MVC_Project.Models.ViewModel;
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
        Task<EmployeeUserVm> AddEmployee(EmployeeUserVm employee);
        Task<Employee> EditEmployee(int? id, EmployeeUserVm updatedEmployee,bool isAdmin);
        Task<Employee> DeleteEmployee(int? id);
        Task<IQueryable<Employee>?> SearchEmployee(string empName);

        Task<int> GenerateNextEmployeeCode();
        Task<bool> EmailAvailable(string Email);

    }
}
