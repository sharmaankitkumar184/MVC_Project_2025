using Microsoft.EntityFrameworkCore;
using MVC_Project.Models.Models;
using MVC_Project.Models.Enums;
using MVC_Project.Services.Data;
using MVC_Project.Services.Repositories.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVC_Project.Services.Repositories
{
    public class AttendanceRepository : IAttendanceRepository
    {
        private readonly ApplicationDbContext _context;

        public AttendanceRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IQueryable<Attendance>> GetAllAttendance()
        {
            return _context.Attendances
                .Include(a => a.Employee)
                .Include(a => a.PunchSessions)
                .AsQueryable();
        }

        public async Task<Attendance> GetAttendanceById(int id)
        {
            return await _context.Attendances
                .Include(a => a.Employee)
                .Include(a => a.PunchSessions.OrderBy(p => p.SessionNumber))
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IQueryable<Attendance>> GetEmployeeAttendance(int employeeId)
        {
            return _context.Attendances
                .Include(a => a.PunchSessions)
                .Where(a => a.EmployeeId == employeeId)
                .OrderByDescending(a => a.AttendanceDate)
                .AsQueryable();
        }

        public async Task<IQueryable<Attendance>> GetEmployeeAttendanceByMonth(int employeeId, int year, int month)
        {
            return _context.Attendances
                .Include(a => a.PunchSessions)
                .Where(a => a.EmployeeId == employeeId
                    && a.AttendanceDate.Year == year
                    && a.AttendanceDate.Month == month)
                .OrderBy(a => a.AttendanceDate)
                .AsQueryable();
        }

        public async Task<IQueryable<Attendance>> GetEmployeeAttendanceByDateRange(int employeeId, DateTime startDate, DateTime endDate)
        {
            return _context.Attendances
                .Include(a => a.PunchSessions)
                .Where(a => a.EmployeeId == employeeId
                    && a.AttendanceDate >= startDate.Date
                    && a.AttendanceDate <= endDate.Date)
                .OrderBy(a => a.AttendanceDate)
                .AsQueryable();
        }

        public async Task<Attendance> GetEmployeeAttendanceByDate(int employeeId, DateTime date)
        {
            return await _context.Attendances
                .Include(a => a.PunchSessions.OrderBy(p => p.SessionNumber))
                .FirstOrDefaultAsync(a => a.EmployeeId == employeeId
                    && a.AttendanceDate.Date == date.Date);
        }

        public async Task<Attendance> AddAttendance(Attendance attendance)
        {
            attendance.CreatedAt = DateTime.UtcNow;
            attendance.UpdatedAt = DateTime.UtcNow;
            _context.Attendances.Add(attendance);
            await _context.SaveChangesAsync();
            return attendance;
        }

        public async Task<Attendance> UpdateAttendance(Attendance attendance)
        {
            attendance.UpdatedAt = DateTime.UtcNow;
            _context.Attendances.Update(attendance);
            await _context.SaveChangesAsync();
            return attendance;
        }

        public async Task<bool> DeleteAttendance(int id)
        {
            var attendance = await GetAttendanceById(id);
            if (attendance == null)
                return false;

            _context.Attendances.Remove(attendance);
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Create a new punch-in session
        /// </summary>
        public async Task<PunchSession> PunchIn(int employeeId, string location, string device)
        {
            var today = DateTime.Now.Date;
            var todayDateTime = DateTime.Now;

            // Get or create today's attendance record
            var attendance = await _context.Attendances
                .Include(a => a.PunchSessions)
                .FirstOrDefaultAsync(a => a.EmployeeId == employeeId && a.AttendanceDate.Date == today);

            if (attendance == null)
            {
                // Create new attendance record for today
                attendance = new Attendance
                {
                    EmployeeId = employeeId,
                    AttendanceDate = today,
                    Status = AttendanceStatus.Present,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                _context.Attendances.Add(attendance);
                await _context.SaveChangesAsync();

                // Reload to get the Id
                attendance = await _context.Attendances
                    .Include(a => a.PunchSessions)
                    .FirstOrDefaultAsync(a => a.EmployeeId == employeeId && a.AttendanceDate.Date == today);
            }

            // Get next session number
            int nextSessionNumber = (attendance.PunchSessions?.Count ?? 0) + 1;

            // Create new punch session
            var punchSession = new PunchSession
            {
                AttendanceId = attendance.Id,
                SessionNumber = nextSessionNumber,
                PunchInTime = todayDateTime,
                LocationCheckIn = location,
                DeviceCheckIn = device,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.PunchSessions.Add(punchSession);
            await _context.SaveChangesAsync();

            return punchSession;
        }

        /// <summary>
        /// Punch out from the current session (most recent punch-in without punch-out)
        /// </summary>
        public async Task<PunchSession> PunchOut(int employeeId, string location, string device)
        {
            var today = DateTime.Now.Date;
            var todayDateTime = DateTime.Now;

            // Get today's attendance
            var attendance = await _context.Attendances
                .Include(a => a.PunchSessions)
                .FirstOrDefaultAsync(a => a.EmployeeId == employeeId && a.AttendanceDate.Date == today);

            if (attendance == null)
                return null;

            // Find the latest punch session without punch out
            var activePunchSession = attendance.PunchSessions
                .OrderByDescending(p => p.SessionNumber)
                .FirstOrDefault(p => !p.PunchOutTime.HasValue);

            if (activePunchSession == null)
                return null;

            // Update punch out time
            activePunchSession.PunchOutTime = todayDateTime;
            activePunchSession.LocationCheckOut = location;
            activePunchSession.DeviceCheckOut = device;
            activePunchSession.UpdatedAt = DateTime.UtcNow;

            _context.PunchSessions.Update(activePunchSession);

            // Update attendance record with latest times for backward compatibility
            attendance.PunchInTime = attendance.PunchSessions.First().PunchInTime.TimeOfDay;
            attendance.PunchOutTime = todayDateTime.TimeOfDay;
            attendance.UpdatedAt = DateTime.UtcNow;

            _context.Attendances.Update(attendance);
            await _context.SaveChangesAsync();

            return activePunchSession;
        }

        /// <summary>
        /// Get all punch sessions for today
        /// </summary>
        public async Task<List<PunchSession>> GetTodayPunchSessions(int employeeId)
        {
            var today = DateTime.Now.Date;
            return await _context.PunchSessions
                .Where(p => p.Attendance.EmployeeId == employeeId
                    && p.Attendance.AttendanceDate.Date == today)
                .OrderBy(p => p.SessionNumber)
                .ToListAsync();
        }

        /// <summary>
        /// Get all punch sessions for a specific attendance date
        /// </summary>
        public async Task<List<PunchSession>> GetAttendancePunchSessions(int attendanceId)
        {
            return await _context.PunchSessions
                .Where(p => p.AttendanceId == attendanceId)
                .OrderBy(p => p.SessionNumber)
                .ToListAsync();
        }

        /// <summary>
        /// Check if employee has an active punch session (punched in but not punched out)
        /// </summary>
        public async Task<bool> HasActivePunchSession(int employeeId)
        {
            var today = DateTime.Now.Date;
            return await _context.PunchSessions
                .AnyAsync(p => p.Attendance.EmployeeId == employeeId
                    && p.Attendance.AttendanceDate.Date == today
                    && !p.PunchOutTime.HasValue);
        }

        public async Task<int> GetPresentDaysCount(int employeeId, int year, int month)
        {
            return await _context.Attendances
                .Where(a => a.EmployeeId == employeeId
                    && a.AttendanceDate.Year == year
                    && a.AttendanceDate.Month == month
                    && a.Status == AttendanceStatus.Present)
                .CountAsync();
        }

        public async Task<int> GetAbsentDaysCount(int employeeId, int year, int month)
        {
            return await _context.Attendances
                .Where(a => a.EmployeeId == employeeId
                    && a.AttendanceDate.Year == year
                    && a.AttendanceDate.Month == month
                    && a.Status == AttendanceStatus.Absent)
                .CountAsync();
        }

        public async Task<int> GetLeaveDaysCount(int employeeId, int year, int month)
        {
            return await _context.Attendances
                .Where(a => a.EmployeeId == employeeId
                    && a.AttendanceDate.Year == year
                    && a.AttendanceDate.Month == month
                    && (a.Status == AttendanceStatus.Leave
                        || a.Status == AttendanceStatus.OnAnnualLeave
                        || a.Status == AttendanceStatus.OnCasual
                        || a.Status == AttendanceStatus.OnSickLeave))
                .CountAsync();
        }

        public async Task<double> GetAverageWorkingHours(int employeeId, int year, int month)
        {
            var attendances = await _context.Attendances
                .Include(a => a.PunchSessions)
                .Where(a => a.EmployeeId == employeeId
                    && a.AttendanceDate.Year == year
                    && a.AttendanceDate.Month == month
                    && a.Status == AttendanceStatus.Present)
                .ToListAsync();

            if (attendances.Count == 0)
                return 0;

            double totalHours = 0;
            int daysWithSessions = 0;

            foreach (var att in attendances)
            {
                if (att.PunchSessions.Count > 0)
                {
                    daysWithSessions++;
                    foreach (var session in att.PunchSessions.Where(s => s.PunchOutTime.HasValue))
                    {
                        var duration = session.PunchOutTime.Value - session.PunchInTime;
                        if (duration < TimeSpan.Zero)
                            duration = duration.Add(TimeSpan.FromHours(24));
                        totalHours += duration.TotalHours;
                    }
                }
            }

            return daysWithSessions > 0 ? Math.Round(totalHours / daysWithSessions, 2) : 0;
        }

        public async Task<IQueryable<Attendance>> GetLeaveRequests(int year, int month)
        {
            return _context.Attendances
                .Include(a => a.Employee)
                .Where(a => a.AttendanceDate.Year == year
                    && a.AttendanceDate.Month == month
                    && a.IsApproved == null
                    && (a.Status == AttendanceStatus.Leave
                        || a.Status == AttendanceStatus.OnAnnualLeave
                        || a.Status == AttendanceStatus.OnCasual
                        || a.Status == AttendanceStatus.OnSickLeave))
                .OrderByDescending(a => a.AttendanceDate)
                .AsQueryable();
        }

        public async Task<bool> ApproveLeave(int attendanceId, bool isApproved, string remark)
        {
            var attendance = await GetAttendanceById(attendanceId);
            if (attendance == null)
                return false;

            attendance.IsApproved = isApproved;
            attendance.ApprovalRemark = remark;
            attendance.ApprovedDate = DateTime.UtcNow;
            attendance.UpdatedAt = DateTime.UtcNow;

            _context.Attendances.Update(attendance);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> HasPunchedInToday(int employeeId)
        {
            var today = DateTime.Now.Date;
            return await _context.PunchSessions
                .AnyAsync(p => p.Attendance.EmployeeId == employeeId
                    && p.Attendance.AttendanceDate.Date == today);
        }

        public async Task<bool> HasPunchedOutToday(int employeeId)
        {
            var today = DateTime.Now.Date;
            return await _context.PunchSessions
                .AnyAsync(p => p.Attendance.EmployeeId == employeeId
                    && p.Attendance.AttendanceDate.Date == today
                    && p.PunchOutTime.HasValue);
        }
    }
}
