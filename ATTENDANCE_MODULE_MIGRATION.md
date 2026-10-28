# Attendance Module - Database Migration Instructions

## Steps to Create and Apply Migration

### 1. Open Package Manager Console
- In Visual Studio, go to: `Tools` → `NuGet Package Manager` → `Package Manager Console`

### 2. Create Migration
Run the following command:

```powershell
Add-Migration AddAttendanceModule -Project MVC_Project.Services
```

### 3. Update Database
```powershell
Update-Database -Project MVC_Project.Services
```

---

## Alternative: Using .NET CLI

If you prefer using the command line:

### 1. Open Terminal in Project Directory
```powershell
cd D:\MVC_PROJECT_2025\MVC_Project.Web
```

### 2. Create Migration
```bash
dotnet ef migrations add AddAttendanceModule --project MVC_Project.Services
```

### 3. Update Database
```bash
dotnet ef database update --project MVC_Project.Services
```

---

## What Gets Created

The migration will create the following:

### Tables:
1. **Attendances** - Main attendance records table
   - Id (PK)
   - EmployeeId (FK to Employees)
   - AttendanceDate
   - PunchInTime, PunchOutTime
   - Status, LeaveType
   - Location and Device info
   - Approval fields
   - Timestamps

### Indexes:
- Unique index on (EmployeeId, AttendanceDate) for performance

---

## Using the Attendance Module

### URL Routes:
- View Attendance: `/Attendance/ViewAttendance` or `/Attendance/Index`
- Punch In: `POST /Attendance/PunchIn`
- Punch Out: `POST /Attendance/PunchOut`
- Apply Leave: `POST /Attendance/ApplyLeave`
- Get Details: `GET /Attendance/GetAttendanceDetails?attendanceId={id}`
- Today Status: `GET /Attendance/TodayStatus`

### Features Implemented:
✅ Dynamic calendar view (month/week/day modes ready)
✅ Punch In/Out functionality
✅ Leave application system
✅ Attendance statistics (present, absent, leave days)
✅ Average working hours calculation
✅ Leave approval workflow (for managers)
✅ Responsive design
✅ Modal dialogs for details and leave application

---

## Controller Authorization

The AttendanceController requires:
- Authentication: Yes
- Roles: Employee, Admin

---

## Database Relationships

```
Employee (1) -----(N) Attendance
  ├─ One employee can have many attendance records
  └─ Attendance records cannot exist without an employee
```

---

## View Models Provided

1. **AttendanceViewModel** - Single record display
2. **AttendanceCalendarViewModel** - Calendar view with statistics
3. **AttendanceFilterViewModel** - For filtering records
4. **ApplyLeaveViewModel** - Leave application form

---

## Notes

- All timestamps are in UTC
- Working hours are calculated automatically from punch in/out times
- The calendar shows status with color coding:
  - Green: Present
  - Red: Absent/Weekly Off
  - Orange/Yellow: Leave
  - Blue: Half Day
- Employee must punch in before punching out
- Leave records can be approved/rejected by managers

---

## Next Steps

1. Create and apply the migration
2. Update route registration in routing if needed
3. Add menu links to the attendance page in your navigation
4. Test punch in/out functionality
5. Configure leave approval workflow in manager dashboard

---

## Support

If you encounter any migration errors:
1. Check that MVC_Project.Services project is built successfully
2. Verify connection string in appsettings.json
3. Ensure all using statements are correct
4. Run `Get-Migrations` to see existing migrations

