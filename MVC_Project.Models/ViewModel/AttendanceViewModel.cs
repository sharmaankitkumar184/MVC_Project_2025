using MVC_Project.Models.Enums;
using MVC_Project.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVC_Project.Models.ViewModel
{
    public class AttendanceViewModel
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeCode { get; set; }
        public DateTime AttendanceDate { get; set; }

        public TimeSpan? PunchInTime { get; set; }
        public TimeSpan? PunchOutTime { get; set; }

        // Punch Sessions
        public List<PunchSession> PunchSessions { get; set; } = new();

        public string PunchInTimeString => PunchInTime?.ToString(@"hh\:mm") ?? "—";
        public string PunchOutTimeString => PunchOutTime?.ToString(@"hh\:mm") ?? "—";

        public TimeSpan WorkingHours
        {
            get
            {
                if (PunchSessions.Count > 0)
                {
                    TimeSpan total = TimeSpan.Zero;
                    foreach (var session in PunchSessions.Where(s => s.PunchOutTime.HasValue))
                    {
                        var duration = session.PunchOutTime.Value - session.PunchInTime;
                        if (duration < TimeSpan.Zero)
                            duration = duration.Add(TimeSpan.FromHours(24));
                        total = total.Add(duration);
                    }
                    return total;
                }

                // Fallback
                if (PunchInTime.HasValue && PunchOutTime.HasValue)
                {
                    var duration = PunchOutTime.Value - PunchInTime.Value;
                    return duration < TimeSpan.Zero ? duration.Add(TimeSpan.FromHours(24)) : duration;
                }
                return TimeSpan.Zero;
            }
        }

        public string WorkingHoursString => 
            WorkingHours.TotalHours > 0 
                ? $"{(int)WorkingHours.TotalHours}h {WorkingHours.Minutes}m" 
                : "—";

        public AttendanceStatus Status { get; set; }
        public LeaveType? LeaveType { get; set; }
        public string LeaveReason { get; set; }

        public string LocationCheckIn { get; set; }
        public string LocationCheckOut { get; set; }
        public string DeviceCheckIn { get; set; }
        public string DeviceCheckOut { get; set; }

        public bool? IsApproved { get; set; }
        public string ApprovalRemark { get; set; }
        public int? ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // UI Helper Properties
        public string StatusBadgeColor
        {
            get
            {
                return Status switch
                {
                    AttendanceStatus.Present => "success",      // Green
                    AttendanceStatus.Absent => "danger",        // Red
                    AttendanceStatus.Leave => "warning",        // Orange
                    AttendanceStatus.HalfDay => "info",         // Light Blue
                    AttendanceStatus.OnAnnualLeave => "warning",
                    AttendanceStatus.OnWeekly => "secondary",
                    AttendanceStatus.WeeklyOff => "danger",
                    AttendanceStatus.OnCasual => "info",
                    AttendanceStatus.OnSickLeave => "warning",
                    AttendanceStatus.OnCompOff => "info",
                    _ => "secondary"
                };
            }
        }

        public string StatusDisplayName
        {
            get
            {
                return Status switch
                {
                    AttendanceStatus.Present => "Present",
                    AttendanceStatus.Absent => "Absent",
                    AttendanceStatus.Leave => "Leave",
                    AttendanceStatus.HalfDay => "Half Day",
                    AttendanceStatus.OnAnnualLeave => "On Annual Leave",
                    AttendanceStatus.OnWeekly => "On Weekly",
                    AttendanceStatus.WeeklyOff => "Weekly Off",
                    AttendanceStatus.OnCasual => "On Casual Leave",
                    AttendanceStatus.OnSickLeave => "On Sick Leave",
                    AttendanceStatus.OnCompOff => "On Comp Off",
                    _ => "Unknown"
                };
            }
        }
    }

    public class AttendanceCalendarViewModel
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public string MonthName => new DateTime(Year, Month, 1).ToString("MMMM yyyy");

        public List<AttendanceViewModel> Attendances { get; set; } = new();
        public Dictionary<int, AttendanceViewModel> AttendancesByDate { get; set; } = new();

        // Statistics
        public int PresentDays { get; set; }
        public int AbsentDays { get; set; }
        public int LeaveDays { get; set; }
        public int HalfDays { get; set; }
        public int WeeklyOffs { get; set; }
        public double AverageWorkingHours { get; set; }

        // Navigation
        public DateTime? PreviousMonth => Month > 1 ? new DateTime(Year, Month - 1, 1) : new DateTime(Year - 1, 12, 1);
        public DateTime? NextMonth => Month < 12 ? new DateTime(Year, Month + 1, 1) : new DateTime(Year + 1, 1, 1);
    }

    public class AttendanceFilterViewModel
    {
        public int EmployeeId { get; set; }
        public int? Year { get; set; } = DateTime.Now.Year;
        public int? Month { get; set; } = DateTime.Now.Month;
        public AttendanceStatus? Status { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string ViewMode { get; set; } = "month"; // month, week, day
    }
}
