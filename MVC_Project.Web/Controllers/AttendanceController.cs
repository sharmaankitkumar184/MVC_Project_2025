using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MVC_Project.Models.Enums;
using MVC_Project.Models.Models;
using MVC_Project.Models.ViewModel;
using MVC_Project.Services.Repositories.IRepository;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace MVC_Project.Web.Controllers
{
    [Authorize(Roles = "Employee,Admin")]
    public class AttendanceController : Controller
    {
        private readonly IAttendanceRepository _attendanceRepo;
        private readonly IEmployeeRepository _employeeRepo;
        private readonly ILogger<AttendanceController> _logger;

        public AttendanceController(
            IAttendanceRepository attendanceRepo,
            IEmployeeRepository employeeRepo,
            ILogger<AttendanceController> logger)
        {
            _attendanceRepo = attendanceRepo;
            _employeeRepo = employeeRepo;
            _logger = logger;
        }

        private int GetLoggedInUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return int.Parse(userIdClaim?.Value ?? "0");
        }

        // GET: Attendance/Index or Attendance/ViewAttendance
        [HttpGet]
        public async Task<IActionResult> Index(int? year, int? month, string viewMode = "month")
        {
            try
            {
                int userId = GetLoggedInUserId();
                var employee = await _employeeRepo.GetEmployeeByUserId(userId);

                if (employee == null)
                    return NotFound("Employee not found");

                year = year ?? DateTime.Now.Year;
                month = month ?? DateTime.Now.Month;

                // Get attendance records for the month
                var attendances = await _attendanceRepo.GetEmployeeAttendanceByMonth(employee.Id, year.Value, month.Value);
                var attendanceList = await attendances.ToListAsync();

                // Build calendar view model
                var calendarModel = new AttendanceCalendarViewModel
                {
                    EmployeeId = employee.Id,
                    EmployeeName = employee.Name,
                    Year = year.Value,
                    Month = month.Value,
                    Attendances = attendanceList.Select(a => new AttendanceViewModel
                    {
                        Id = a.Id,
                        EmployeeId = a.EmployeeId,
                        EmployeeName = a.Employee.Name,
                        EmployeeCode = a.Employee.EmployeeCode,
                        AttendanceDate = a.AttendanceDate,
                        PunchInTime = a.PunchInTime,
                        PunchOutTime = a.PunchOutTime,
                        Status = a.Status,
                        LeaveType = a.LeaveType,
                        LeaveReason = a.LeaveReason,
                        LocationCheckIn = a.LocationCheckIn,
                        LocationCheckOut = a.LocationCheckOut,
                        DeviceCheckIn = a.DeviceCheckIn,
                        DeviceCheckOut = a.DeviceCheckOut,
                        IsApproved = a.IsApproved,
                        ApprovalRemark = a.ApprovalRemark
                    }).ToList(),
                    PresentDays = await _attendanceRepo.GetPresentDaysCount(employee.Id, year.Value, month.Value),
                    AbsentDays = await _attendanceRepo.GetAbsentDaysCount(employee.Id, year.Value, month.Value),
                    LeaveDays = await _attendanceRepo.GetLeaveDaysCount(employee.Id, year.Value, month.Value),
                    AverageWorkingHours = await _attendanceRepo.GetAverageWorkingHours(employee.Id, year.Value, month.Value)
                };

                // Build attendance dictionary for calendar rendering
                calendarModel.AttendancesByDate = calendarModel.Attendances
                    .GroupBy(a => a.AttendanceDate.Day)
                    .ToDictionary(g => g.Key, g => g.First());

                return View("ViewAttendance", calendarModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in attendance index");
                return BadRequest("Error loading attendance");
            }
        }

        // GET: Attendance/ViewAttendance
        [HttpGet]
        public async Task<IActionResult> ViewAttendance(int? year, int? month, string viewMode = "month")
        {
            return await Index(year, month, viewMode);
        }

        // POST: Attendance/PunchIn
        [HttpPost]
        public async Task<IActionResult> PunchIn(string location = "Office", string device = "Web")
        {
            try
            {
                int userId = GetLoggedInUserId();
                var employee = await _employeeRepo.GetEmployeeByUserId(userId);

                if (employee == null)
                    return Json(new { success = false, message = "Employee not found" });

                // Create new punch session (allow multiple punches per day)
                var punchSession = await _attendanceRepo.PunchIn(employee.Id, location, device);

                return Json(new
                {
                    success = true,
                    message = "Punched in successfully",
                    data = new
                    {
                        sessionId = punchSession.Id,
                        sessionNumber = punchSession.SessionNumber,
                        punchInTime = punchSession.PunchInTime.ToString("HH:mm:ss"),
                        location = punchSession.LocationCheckIn
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in punch in");
                return Json(new { success = false, message = "Error during punch in" });
            }
        }

        // POST: Attendance/PunchOut
        [HttpPost]
        public async Task<IActionResult> PunchOut(string location = "Office", string device = "Web")
        {
            try
            {
                int userId = GetLoggedInUserId();
                var employee = await _employeeRepo.GetEmployeeByUserId(userId);

                if (employee == null)
                    return Json(new { success = false, message = "Employee not found" });

                // Check if there's an active punch session
                bool hasActive = await _attendanceRepo.HasActivePunchSession(employee.Id);
                if (!hasActive)
                    return Json(new { success = false, message = "Please punch in first" });

                // Punch out from current session
                var punchSession = await _attendanceRepo.PunchOut(employee.Id, location, device);

                if (punchSession == null)
                    return Json(new { success = false, message = "No active punch session found" });

                return Json(new
                {
                    success = true,
                    message = "Punched out successfully",
                    data = new
                    {
                        sessionId = punchSession.Id,
                        sessionNumber = punchSession.SessionNumber,
                        punchOutTime = punchSession.PunchOutTime?.ToString("HH:mm:ss"),
                        sessionDuration = FormatDuration(punchSession.SessionDuration)
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in punch out");
                return Json(new { success = false, message = "Error during punch out" });
            }
        }

        // POST: Attendance/ApplyLeave
        [HttpPost]
        public async Task<IActionResult> ApplyLeave(ApplyLeaveViewModel model)
        {
            try
            {
                int userId = GetLoggedInUserId();
                var employee = await _employeeRepo.GetEmployeeByUserId(userId);

                if (employee == null)
                    return Json(new { success = false, message = "Employee not found" });

                var attendance = new Attendance
                {
                    EmployeeId = employee.Id,
                    AttendanceDate = model.LeaveDate,
                    Status = (AttendanceStatus)model.LeaveTypeId,
                    LeaveType = model.LeaveType,
                    LeaveReason = model.LeaveReason,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _attendanceRepo.AddAttendance(attendance);

                return Json(new { success = true, message = "Leave applied successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error applying leave");
                return Json(new { success = false, message = "Error applying leave" });
            }
        }

        // GET: Attendance/GetAttendanceDetails
        [HttpGet]
        public async Task<IActionResult> GetAttendanceDetails(int attendanceId)
        {
            try
            {
                var attendance = await _attendanceRepo.GetAttendanceById(attendanceId);
                if (attendance == null)
                    return Json(new { success = false, message = "Record not found" });

                var viewModel = new AttendanceViewModel
                {
                    Id = attendance.Id,
                    EmployeeId = attendance.EmployeeId,
                    EmployeeName = attendance.Employee.Name,
                    EmployeeCode = attendance.Employee.EmployeeCode,
                    AttendanceDate = attendance.AttendanceDate,
                    PunchInTime = attendance.PunchInTime,
                    PunchOutTime = attendance.PunchOutTime,
                    Status = attendance.Status,
                    LeaveType = attendance.LeaveType,
                    LeaveReason = attendance.LeaveReason,
                    LocationCheckIn = attendance.LocationCheckIn,
                    LocationCheckOut = attendance.LocationCheckOut,
                    DeviceCheckIn = attendance.DeviceCheckIn,
                    DeviceCheckOut = attendance.DeviceCheckOut,
                    IsApproved = attendance.IsApproved,
                    ApprovalRemark = attendance.ApprovalRemark
                };

                return Json(new { success = true, data = viewModel });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching attendance details");
                return Json(new { success = false, message = "Error fetching details" });
            }
        }

        // GET: Attendance/TodayStatus
        [HttpGet]
        public async Task<IActionResult> TodayStatus()
        {
            try
            {
                int userId = GetLoggedInUserId();
                var employee = await _employeeRepo.GetEmployeeByUserId(userId);

                if (employee == null)
                    return Json(new { success = false, message = "Employee not found" });

                // Get today's punch sessions
                var sessions = await _attendanceRepo.GetTodayPunchSessions(employee.Id);

                if (sessions.Count == 0)
                    return Json(new
                    {
                        success = true,
                        status = "NotPunchedIn",
                        message = "Not punched in yet",
                        sessions = new List<object>(),
                        hasActivePunch = false
                    });

                // Calculate total working hours
                TimeSpan totalDuration = TimeSpan.Zero;
                foreach (var session in sessions.Where(s => s.PunchOutTime.HasValue))
                {
                    var duration = session.PunchOutTime.Value - session.PunchInTime;
                    if (duration < TimeSpan.Zero)
                        duration = duration.Add(TimeSpan.FromHours(24));
                    totalDuration = totalDuration.Add(duration);
                }

                // Check for active punch session
                var activeSess = sessions.FirstOrDefault(s => !s.PunchOutTime.HasValue);

                return Json(new
                {
                    success = true,
                    status = "Present",
                    totalSessions = sessions.Count,
                    hasActivePunch = activeSess != null,
                    totalWorkingHours = FormatDuration(totalDuration),
                    sessions = sessions.Select(s => new
                    {
                        sessionNumber = s.SessionNumber,
                        punchInTime = s.PunchInTime.ToString("HH:mm:ss"),
                        punchOutTime = s.PunchOutTime?.ToString("HH:mm:ss") ?? "—",
                        sessionDuration = s.PunchOutTime.HasValue ? FormatDuration(s.SessionDuration) : "Active"
                    }).ToList()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching today status");
                return Json(new { success = false, message = "Error fetching status" });
            }
        }

        // GET: Attendance/GetPunchSessions
        [HttpGet]
        public async Task<IActionResult> GetPunchSessions(int? attendanceId = null)
        {
            try
            {
                int userId = GetLoggedInUserId();
                var employee = await _employeeRepo.GetEmployeeByUserId(userId);

                if (employee == null)
                    return Json(new { success = false, message = "Employee not found" });

                List<PunchSession> sessions;

                if (attendanceId.HasValue)
                {
                    // Get sessions for specific attendance date
                    sessions = await _attendanceRepo.GetAttendancePunchSessions(attendanceId.Value);
                }
                else
                {
                    // Get today's sessions
                    sessions = await _attendanceRepo.GetTodayPunchSessions(employee.Id);
                }

                return Json(new
                {
                    success = true,
                    sessions = sessions.Select(s => new
                    {
                        id = s.Id,
                        sessionNumber = s.SessionNumber,
                        punchInTime = s.PunchInTime.ToString("HH:mm:ss"),
                        punchOutTime = s.PunchOutTime?.ToString("HH:mm:ss"),
                        sessionDuration = s.PunchOutTime.HasValue ? FormatDuration(s.SessionDuration) : "Active",
                        locationCheckIn = s.LocationCheckIn,
                        locationCheckOut = s.LocationCheckOut
                    }).ToList()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching punch sessions");
                return Json(new { success = false, message = "Error fetching sessions" });
            }
        }

        private string FormatDuration(TimeSpan duration)
        {
            if (duration.TotalHours < 1)
                return $"{duration.Minutes}m";
            else
                return $"{(int)duration.TotalHours}h {duration.Minutes}m";
        }

        // GET: Attendance/GetMonthlyData
        [HttpGet]
        public async Task<IActionResult> GetMonthlyData(int? year, int? month)
        {
            try
            {
                int userId = GetLoggedInUserId();
                var employee = await _employeeRepo.GetEmployeeByUserId(userId);

                if (employee == null)
                    return Json(new { success = false, message = "Employee not found" });

                year = year ?? DateTime.Now.Year;
                month = month ?? DateTime.Now.Month;

                var attendances = await _attendanceRepo.GetEmployeeAttendanceByMonth(employee.Id, year.Value, month.Value);
                var attendanceList = await attendances.ToListAsync();

                // Calculate monthly statistics
                TimeSpan totalWorkingHours = TimeSpan.Zero;
                int presentDays = 0;
                int absentDays = 0;
                int leaveDays = 0;
                int weekendDays = 0;
                int holidayDays = 0;

                var allDaysInMonth = DateTime.DaysInMonth(year.Value, month.Value);
                for (int day = 1; day <= allDaysInMonth; day++)
                {
                    var date = new DateTime(year.Value, month.Value, day);

                    // Check if weekend or holiday
                    if (MVC_Project.Models.Constants.IndianHolidays.IsWeekend(date))
                    {
                        weekendDays++;
                    }
                    else if (MVC_Project.Models.Constants.IndianHolidays.IsHoliday(date))
                    {
                        holidayDays++;
                    }
                }

                foreach (var att in attendanceList)
                {
                    if (att.Status == AttendanceStatus.Present && att.PunchSessions.Count > 0)
                    {
                        presentDays++;
                        foreach (var session in att.PunchSessions.Where(s => s.PunchOutTime.HasValue))
                        {
                            var duration = session.PunchOutTime.Value - session.PunchInTime;
                            if (duration < TimeSpan.Zero)
                                duration = duration.Add(TimeSpan.FromHours(24));
                            totalWorkingHours = totalWorkingHours.Add(duration);
                        }
                    }
                    else if (att.Status == AttendanceStatus.Absent)
                    {
                        absentDays++;
                    }
                    else if (att.Status == AttendanceStatus.Leave || att.Status == AttendanceStatus.OnAnnualLeave || 
                             att.Status == AttendanceStatus.OnCasual || att.Status == AttendanceStatus.OnSickLeave)
                    {
                        leaveDays++;
                    }
                }

                return Json(new
                {
                    success = true,
                    monthYear = new DateTime(year.Value, month.Value, 1).ToString("MMMM yyyy"),
                    statistics = new
                    {
                        presentDays,
                        absentDays,
                        leaveDays,
                        weekendDays,
                        holidayDays,
                        totalWorkingHours = $"{(int)totalWorkingHours.TotalHours}h {totalWorkingHours.Minutes}m",
                        totalWorkingHoursDecimal = Math.Round(totalWorkingHours.TotalHours, 2)
                    },
                    attendances = attendanceList.Select(a => new
                    {
                        id = a.Id,
                        date = a.AttendanceDate.ToString("yyyy-MM-dd"),
                        dayName = a.AttendanceDate.ToString("dddd"),
                        status = a.Status.ToString(),
                        statusDisplay = a.Status.ToString(),
                        totalSessions = a.PunchSessions.Count,
                        totalHours = CalculateAttendanceTotalHours(a),
                        sessions = a.PunchSessions.Select(s => new
                        {
                            sessionNumber = s.SessionNumber,
                            punchInTime = s.PunchInTime.ToString("HH:mm"),
                            punchOutTime = s.PunchOutTime?.ToString("HH:mm") ?? "—",
                            duration = s.PunchOutTime.HasValue ? FormatDuration(s.SessionDuration) : "Ongoing"
                        }).ToList()
                    }).ToList()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching monthly data");
                return Json(new { success = false, message = "Error fetching monthly data" });
            }
        }

        // GET: Attendance/GetDayStatusSummary - For calendar display
        [HttpGet]
        public async Task<IActionResult> GetDayStatusSummary(string date)
        {
            try
            {
                int userId = GetLoggedInUserId();
                var employee = await _employeeRepo.GetEmployeeByUserId(userId);

                if (employee == null)
                    return Json(new { success = false, message = "Employee not found" });

                if (!DateTime.TryParse(date, out var attendanceDate))
                    return Json(new { success = false, message = "Invalid date" });

                var attendance = await _attendanceRepo.GetEmployeeAttendanceByDate(employee.Id, attendanceDate);

                if (attendance == null)
                {
                    // Check if weekend or holiday
                    if (MVC_Project.Models.Constants.IndianHolidays.IsWeekend(attendanceDate))
                        return Json(new { success = true, status = "WeeklyOff", displayText = "Weekly Off" });

                    if (MVC_Project.Models.Constants.IndianHolidays.IsHoliday(attendanceDate))
                        return Json(new { success = true, status = "Holiday", displayText = MVC_Project.Models.Constants.IndianHolidays.GetHolidayName(attendanceDate) });

                    return Json(new { success = true, status = "NoRecord", displayText = "No record" });
                }

                var totalDuration = attendance.PunchSessions
                    .Where(s => s.PunchOutTime.HasValue)
                    .Aggregate(TimeSpan.Zero, (acc, s) => 
                    {
                        var duration = s.PunchOutTime.Value - s.PunchInTime;
                        return acc.Add(duration < TimeSpan.Zero ? duration.Add(TimeSpan.FromHours(24)) : duration);
                    });

                return Json(new
                {
                    success = true,
                    status = attendance.Status.ToString(),
                    statusDisplay = attendance.Status.ToString(),
                    punchInTime = attendance.PunchSessions.FirstOrDefault()?.PunchInTime.ToString("HH:mm"),
                    totalHours = $"{(int)totalDuration.TotalHours}h {totalDuration.Minutes}m",
                    totalSessions = attendance.PunchSessions.Count
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching day status");
                return Json(new { success = false, message = "Error fetching day status" });
            }
        }

        private string CalculateAttendanceTotalHours(Attendance attendance)
        {
            if (attendance.PunchSessions.Count == 0)
                return "—";

            var total = attendance.PunchSessions
                .Where(s => s.PunchOutTime.HasValue)
                .Aggregate(TimeSpan.Zero, (acc, s) =>
                {
                    var duration = s.PunchOutTime.Value - s.PunchInTime;
                    return acc.Add(duration < TimeSpan.Zero ? duration.Add(TimeSpan.FromHours(24)) : duration);
                });

            return $"{(int)total.TotalHours}h {total.Minutes}m";
        }
    }

    // Helper View Models
    public class ApplyLeaveViewModel
    {
        public DateTime LeaveDate { get; set; }
        public int LeaveTypeId { get; set; }
        public LeaveType LeaveType { get; set; }
        public string LeaveReason { get; set; }
    }
}
