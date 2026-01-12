using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVC_Project.Services.Services.IService
{
    public interface IEmailService
    {
        Task SendPasswordResetEmail(string toEmail, string resetLink);
    }
}
