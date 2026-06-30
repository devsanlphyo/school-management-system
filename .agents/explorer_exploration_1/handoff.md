# Handoff Report — Teacher Attendance Exploration

## 1. Observation

- **Database Context & Models**:
  - `ApplicationDbContext` is located at `MainSchoolsManagementSystem/Data/ApplicationDbContext.cs` (lines 6-93). It contains `DbSet<Attendance> Attendances` and `DbSet<SystemSetting> SystemSettings`.
  - `Attendance` model is located at `MainSchoolsManagementSystem/Data/Attendance.cs` (lines 12-20):
    ```csharp
    public class Attendance
    {
        public int Id { get; set; }
        public string TeacherId { get; set; } = string.Empty;
        public ApplicationUser? Teacher { get; set; }
        public DateTime Date { get; set; }
        public DateTime? CheckedInAt { get; set; }
        public AttendanceStatus Status { get; set; } = AttendanceStatus.Absent;
    }
    ```
  - `AttendanceStatus` enum is declared in `MainSchoolsManagementSystem/Data/Attendance.cs` (lines 5-10):
    ```csharp
    public enum AttendanceStatus
    {
        Present = 0,
        Late = 1,
        Absent = 2
    }
    ```
  - `SystemSetting` model is located at `MainSchoolsManagementSystem/Data/SystemSetting.cs` (lines 5-11):
    ```csharp
    public class SystemSetting
    {
        public int Id { get; set; }
        public TimeSpan DailyDeadline { get; set; } = new TimeSpan(8, 30, 0);
        public bool MaintenanceMode { get; set; } = false;
    }
    ```

- **Logged-In User Retrieval**:
  - `design-system.md` (lines 560-567) documents:
    ```csharp
    @inject AuthenticationStateProvider AuthStateProvider
    @using System.Security.Claims

    var authState = await AuthStateProvider.GetAuthenticationStateAsync();
    var user = authState.User;
    var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    ```
  - `design-system.md` (lines 540-544) documents the rule to always use `IDbContextFactory` in Blazor components and never inject scoped context directly:
    ```csharp
    @inject IDbContextFactory<ApplicationDbContext> DbFactory

    using var db = await DbFactory.CreateDbContextAsync();
    ```

- **Timezone and Deadline Handling**:
  - `Headmaster/Attendance.razor` (line 90) uses `.ToLocalTime()` for UI display:
    ```razor
    @(record.Attendance?.CheckedInAt?.ToLocalTime().ToString("hh:mm tt") ?? "—")
    ```
  - `Headmaster/Attendance.razor` (line 208) saves `CheckedInAt` in UTC:
    ```csharp
    CheckedInAt = (status == AttendanceStatus.Present || status == AttendanceStatus.Late) ? DateTime.UtcNow : null
    ```

- **Navigation Link**:
  - `TeacherLayout.razor` at `MainSchoolsManagementSystem/Components/Layout/TeacherLayout.razor` (lines 17-19) already contains the link:
    ```razor
    <NavLink class='nav-item' href='teacher/attendance' @onclick='CloseSidebar'>
        <svg width='18' height='18' viewBox='0 0 24 24' ...>...</svg>
        Attendance
    </NavLink>
    ```

- **Existing Tests**:
  - `MainSchoolsManagementSystem/Data/DbTestRunner.cs` is the only test file. It is not referenced or called in `Program.cs`.

---

## 2. Logic Chain

1. **Database Access**: By analyzing `ApplicationDbContext.cs` and `design-system.md`, we know that the `Attendances` and `SystemSettings` tables are available, and any new Blazor page must use `IDbContextFactory<ApplicationDbContext>` rather than direct injection of `ApplicationDbContext`.
2. **Current User**: By analyzing `design-system.md` (Section 10.3) and `TeacherDashboard.razor`, we know we can retrieve the logged-in teacher's `UserId` via `AuthStateProvider.GetAuthenticationStateAsync()` and `ClaimTypes.NameIdentifier`.
3. **Check-in Time & Status Comparison**:
   - Observations show that `CheckedInAt` is stored in UTC, and `DailyDeadline` is stored as a `TimeSpan` (e.g. `08:30:00`) representing local time.
   - Therefore, to determine if a check-in is late:
     1. Convert `CheckedInAt` (UTC) to local time using `.ToLocalTime()`.
     2. Extract the local time of day (`TimeOfDay`).
     3. Compare `TimeOfDay` against `DailyDeadline` from `SystemSetting`.
4. **Navigation**: The `TeacherLayout.razor` already has the `/teacher/attendance` route hooked up, meaning no layout navigation modifications are needed.
5. **Testing**: Since there are no unit/integration test projects, verification of any implemented logic must be done by running the web application and optionally hooking up `DbTestRunner.RunTestsAsync` in `Program.cs`.

---

## 3. Caveats

- We assume the server's local time (via `.ToLocalTime()`) is the correct timezone context for the school. If the server is hosted in a different timezone than the school/users, `.ToLocalTime()` will reflect the server's timezone, not the school's. However, since there is no timezone configuration on `School` or `ApplicationUser` models, this is the current design pattern of the application.
- The `SystemSetting` is a single global record. All schools in the system share the same `DailyDeadline`.

---

## 4. Conclusion

All necessary models (`Attendance`, `SystemSetting`), navigation links (`TeacherLayout`), design patterns (`glass-panel`, `btn-premium`, `premium-table`), and identity retrieval methods are in place. The implementation of `/teacher/attendance` can proceed by creating/updating `MainSchoolsManagementSystem/Components/Pages/Teacher/Attendance.razor` to:
1. Retrieve the logged-in teacher's ID.
2. Query the teacher's attendance record for the current day.
3. Allow the teacher to check in (recording `DateTime.UtcNow` as `CheckedInAt`).
4. Query the global `SystemSetting` to compare the check-in time (`CheckedInAt.ToLocalTime().TimeOfDay`) with `DailyDeadline` to determine if the status is `Present` or `Late`.
5. Display a history of the teacher's past attendance records using a `premium-table` inside a `glass-panel`.

---

## 5. Verification Method

- **Files to Inspect**:
  - `MainSchoolsManagementSystem/Components/Pages/Teacher/Attendance.razor` (implementation target).
  - `MainSchoolsManagementSystem/Components/Layout/TeacherLayout.razor` (navigation).
  - `MainSchoolsManagementSystem/Data/DbTestRunner.cs` (database relationship validation).
- **Run Command**:
  - To build the project: `dotnet build MainSchoolsManagementSystem/MainSchoolsManagementSystem.csproj`
  - To run the application: `dotnet run --project MainSchoolsManagementSystem/MainSchoolsManagementSystem.csproj`
