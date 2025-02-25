using Microsoft.EntityFrameworkCore;
using MVC_Project.Models.Models;
using MVC_Project.Services.Data;
using MVC_Project.Services.Repositories.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVC_Project.Services.Repositories
{
    public class DepartmentRepository : IDepartmentRepository
    {

        private readonly ApplicationDbContext _db;

        public DepartmentRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Department>> GetAllDepartment()
        {
            return await Task.FromResult(_db.Departments.ToList());
        }
        public Task<Department> AddDepartment(Department department)
        {
            throw new NotImplementedException();
        }

        public Task<Department> DeleteDepartment(int? id)
        {
            throw new NotImplementedException();
        }

        public Task<Department> EditDepartment(int? id, Department department)
        {
            throw new NotImplementedException();
        }

        public Task<Department?> GetDepartmentById(int? id)
        {
            throw new NotImplementedException();
        }
    }
}
