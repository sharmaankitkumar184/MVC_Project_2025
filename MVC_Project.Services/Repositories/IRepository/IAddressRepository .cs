using MVC_Project.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVC_Project.Services.Repositories.IRepository
{
     public interface IAddressRepository
    {
        Task<IEnumerable<Address>> GetAllAddress();
        Task<Address?> GetAddressById(int? id);
        Task<Address> AddAddress(Address address);
        Task<Address> EditAddress(int? id, Address address);
        Task<Address> DeleteAddress(int? id);
    }
}
