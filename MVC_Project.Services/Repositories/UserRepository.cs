using Microsoft.EntityFrameworkCore;
using MVC_Project.Models.Models;
using MVC_Project.Services.Data;
using MVC_Project.Services.Repositories.IRepository;


namespace MVC_Project.Services.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IQueryable<UserData>> GetAllUserAsync()
        {
            return _context.Users.Include(e => e.Address);
        }

        public async Task<(LoginResult Result, UserData User)> AuthenticateUserAsync(string email, string password)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
                return (LoginResult.EmailNotFound, null);

            bool isValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);

            if (!isValid)
                return (LoginResult.InvalidPassword, null);

            return (LoginResult.Success, user);
        }


        public async Task<UserData> GetUserDetailsByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<UserData> GetUserDetailsByPhoneAsync(string PhoneNumber)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.PhoneNumber == PhoneNumber);
        }

        public async Task<UserData> GetByResetTokenAsync(string token, string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u =>
                u.Email == email &&
                u.PasswordResetToken == token
            );
        }

        public async Task UpdateAsync(UserData user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task<int> adminCountAsync()
        {
            var adminCount = await _context.Users
    .CountAsync(u => u.Role == UserRole.Admin);
            return adminCount;
        }


    }
}
