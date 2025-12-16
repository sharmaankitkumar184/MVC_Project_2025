using Microsoft.Extensions.Configuration;
using MVC_Project.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVC_Project.Services.Repositories.IRepository
{
    public interface IUserRepository
    {
        Task<bool> RegisterUserAsync(UserData user);
        Task<UserData> AuthenticateUserAsync(string email, string password);
        Task<bool> IsEmailRegisteredAsync(string email);
    }
}
