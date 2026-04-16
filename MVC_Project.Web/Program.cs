using Microsoft.EntityFrameworkCore;
using MVC_Project.Services.Data;
using MVC_Project.Services.Repositories;
using MVC_Project.Services.Repositories.IRepository;
using MVC_Project.Services.Services;
using MVC_Project.Services.Services.IService;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// MVC + DI
builder.Services.AddControllersWithViews();
builder.Services.AddTransient<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddTransient<IDepartmentRepository, DepartmentRepository>();
builder.Services.AddTransient<IAddressRepository, AddressRepository>();
builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddTransient<HomeDashboardRepository>();
builder.Services.AddTransient<ISalaryRepository, SalaryRepository>();

builder.Services.AddAuthentication("MyCookieAuth")
    .AddCookie("MyCookieAuth", options =>
    {
        options.Cookie.Name = "MyAppAuthCookie";
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(2);
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization();


// DB context
var connectionString = builder.Configuration.GetConnectionString("MVCPracticeDB");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString)
);

// Build and middleware
var app = builder.Build();

app.UseExceptionHandler("/Error");
if (!app.Environment.IsDevelopment())
{
   
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // 👈 Always before Authorization
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Homepage}/{id?}");

app.Run();
