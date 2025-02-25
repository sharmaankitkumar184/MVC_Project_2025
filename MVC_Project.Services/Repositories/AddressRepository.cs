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
    public class AddressRepository : IAddressRepository
    {

        private readonly ApplicationDbContext _db;

        public AddressRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Address>> GetAllAddress()
        {
            return await Task.FromResult(_db.Addresses.ToList());
        }
        public async Task<Address> AddAddress(Address address)
        {
            if (address == null)
            {
                throw new ArgumentNullException(nameof(address), "Employee cannot be null");
            }

            _db.Addresses.Add(address);
            await _db.SaveChangesAsync();
            return address;
        }

        public Task<Address> DeleteAddress(int? id)
        {
            throw new NotImplementedException();
        }

        public Task<Address> EditAddress(int? id, Address address)
        {
            throw new NotImplementedException();
        }

        public Task<Address?> GetAddressById(int? id)
        {
            throw new NotImplementedException();
        }

    }
}
