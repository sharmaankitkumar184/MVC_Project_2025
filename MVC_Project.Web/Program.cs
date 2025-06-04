using Microsoft.EntityFrameworkCore;
using MVC_Project.Services.Data;
using MVC_Project.Services.Repositories;
using MVC_Project.Services.Repositories.IRepository;
using System;

var builder = WebApplication.CreateBuilder(args);

// Enable nullable reference types context
#nullable enable // Enable nullable context to avoid warnings

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddTransient<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddTransient<IDepartmentRepository, DepartmentRepository>();
builder.Services.AddTransient<IAddressRepository, AddressRepository>();

//Entity Database connectivity
var ConnectionString = builder.Configuration.GetConnectionString("MVCPracticeDB");
builder.Services.AddDbContext<ApplicationDbContext>(option =>
option.UseSqlServer(ConnectionString)
);


var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Homepage}/{id?}");

app.Run();
