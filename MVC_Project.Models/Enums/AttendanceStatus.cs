namespace MVC_Project.Models.Enums
{
    public enum AttendanceStatus
    {
        Present = 0,
        Absent = 1,
        Leave = 2,
        HalfDay = 3,
        OnAnnualLeave = 4,
        OnWeekly = 5,
        WeeklyOff = 6,
        OnCasual = 7,
        OnSickLeave = 8,
        OnCompOff = 9
    }

    public enum LeaveType
    {
        Annual = 0,
        Casual = 1,
        Sick = 2,
        Maternity = 3,
        Paternity = 4,
        CompensatoryOff = 5,
        HalfDay = 6,
        CarryForward = 7
    }
}
