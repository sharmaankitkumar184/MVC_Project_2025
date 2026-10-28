using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVC_Project.Models.Models
{
    /// <summary>
    /// Represents a single punch session (punch in + punch out pair)
    /// Multiple sessions can exist for a single day
    /// </summary>
    public class PunchSession
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int AttendanceId { get; set; }

        [ForeignKey("AttendanceId")]
        public Attendance Attendance { get; set; }

        [Required]
        public int SessionNumber { get; set; } // 1st punch, 2nd punch, etc.

        [Required]
        public DateTime PunchInTime { get; set; }

        public DateTime? PunchOutTime { get; set; }

        // Working duration for this session
        [NotMapped]
        public TimeSpan SessionDuration
        {
            get
            {
                if (PunchOutTime.HasValue)
                {
                    var duration = PunchOutTime.Value - PunchInTime;
                    return duration < TimeSpan.Zero ? duration.Add(TimeSpan.FromHours(24)) : duration;
                }
                return TimeSpan.Zero;
            }
        }

        // Location and Device Info
        [StringLength(100)]
        public string? LocationCheckIn { get; set; }

        [StringLength(100)]
        public string? LocationCheckOut { get; set; }

        [StringLength(100)]
        public string? DeviceCheckIn { get; set; }

        [StringLength(100)]
        public string? DeviceCheckOut { get; set; }

        // Break details
        [StringLength(500)]
        public string? BreakReason { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
