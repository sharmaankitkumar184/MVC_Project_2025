using Microsoft.Extensions.Configuration;
using MVC_Project.Models.Models;
using MVC_Project.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVC_Project.Services.Repositories.IRepository
{
    public interface IUserRepository
    {
        Task<IQueryable<UserData>> GetAllUserAsync();
        Task<(LoginResult Result, UserData User)> AuthenticateUserAsync(string email, string password);
        Task<UserData> GetUserDetailsByEmailAsync(string email);

        Task<UserData> GetUserDetailsByPhoneAsync(string PhoneNumber);

        Task<UserData> GetByResetTokenAsync(string token, string email);

        Task UpdateAsync(UserData user);

        Task<int> adminCountAsync();
    }
}
