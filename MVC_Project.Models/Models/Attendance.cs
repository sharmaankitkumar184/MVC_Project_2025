using MVC_Project.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVC_Project.Models.Models
{
    public class Attendance
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int EmployeeId { get; set; }

        [ForeignKey("EmployeeId")]
        public Employee Employee { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime AttendanceDate { get; set; }

        // Punch In/Out Times (kept for backward compatibility)
        public TimeSpan? PunchInTime { get; set; }
        public TimeSpan? PunchOutTime { get; set; }

        // Collection of punch sessions for the day
        public ICollection<PunchSession> PunchSessions { get; set; } = new List<PunchSession>();

        // Working Hours (calculated from all sessions)
        [NotMapped]
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

                // Fallback to old method if no sessions
                if (PunchInTime.HasValue && PunchOutTime.HasValue)
                {
                    var duration = PunchOutTime.Value - PunchInTime.Value;
                    return duration < TimeSpan.Zero ? duration.Add(TimeSpan.FromHours(24)) : duration;
                }
                return TimeSpan.Zero;
            }
        }

        // Status and Leave Information
        [Required]
        public AttendanceStatus Status { get; set; }

        public LeaveType? LeaveType { get; set; }

        // Leave Remarks
        [StringLength(500)]
        public string? LeaveReason { get; set; }

        // Location Info
        [StringLength(100)]
        public string? LocationCheckIn { get; set; }

        [StringLength(100)]
        public string? LocationCheckOut { get; set; }

        // Device Info
        [StringLength(100)]
        public string? DeviceCheckIn { get; set; }

        [StringLength(100)]
        public string? DeviceCheckOut { get; set; }

        // Approval Info
        public bool? IsApproved { get; set; }

        [StringLength(500)]
        public string? ApprovalRemark { get; set; }

        public int? ApprovedBy { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? ApprovedDate { get; set; }

        // Timestamps
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
