using MVC_Project.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVC_Project.Services.Repositories.IRepository
{
    public interface IAttendanceRepository
    {
        // Get attendance records
        Task<IQueryable<Attendance>> GetAllAttendance();
        Task<Attendance> GetAttendanceById(int id);
        Task<IQueryable<Attendance>> GetEmployeeAttendance(int employeeId);
        Task<IQueryable<Attendance>> GetEmployeeAttendanceByMonth(int employeeId, int year, int month);
        Task<IQueryable<Attendance>> GetEmployeeAttendanceByDateRange(int employeeId, DateTime startDate, DateTime endDate);
        Task<Attendance> GetEmployeeAttendanceByDate(int employeeId, DateTime date);

        // Add/Edit/Delete operations
        Task<Attendance> AddAttendance(Attendance attendance);
        Task<Attendance> UpdateAttendance(Attendance attendance);
        Task<bool> DeleteAttendance(int id);

        // Punch Session operations (Multiple punch in/out per day)
        Task<PunchSession> PunchIn(int employeeId, string location, string device);
        Task<PunchSession> PunchOut(int employeeId, string location, string device);
        Task<List<PunchSession>> GetTodayPunchSessions(int employeeId);
        Task<List<PunchSession>> GetAttendancePunchSessions(int attendanceId);
        Task<bool> HasActivePunchSession(int employeeId); // Check if currently punched in

        // Statistics and Reports
        Task<int> GetPresentDaysCount(int employeeId, int year, int month);
        Task<int> GetAbsentDaysCount(int employeeId, int year, int month);
        Task<int> GetLeaveDaysCount(int employeeId, int year, int month);
        Task<double> GetAverageWorkingHours(int employeeId, int year, int month);

        // Leave management
        Task<IQueryable<Attendance>> GetLeaveRequests(int year, int month);
        Task<bool> ApproveLeave(int attendanceId, bool isApproved, string remark);

        // Verification methods
        Task<bool> HasPunchedInToday(int employeeId);
        Task<bool> HasPunchedOutToday(int employeeId);
    }
}
