# Handoff Report — Explorer 3

This report outlines the facts gathered from the `SchoolsManagementSystem` codebase to support the implementation of the Teacher Attendance feature.

---

## 1. Observation
I observed the following files and code snippets in the repository:

### A. Database Models and Context
- **`MainSchoolsManagementSystem/Data/ApplicationDbContext.cs`**:
  - Line 10: `public DbSet<SystemSetting> SystemSettings { get; set; }`
  - Line 11: `public DbSet<Attendance> Attendances { get; set; }`
  - Lines 44-48:
    ```csharp
    builder.Entity<Attendance>()
        .HasOne(a => a.Teacher)
        .WithMany()
        .HasForeignKey(a => a.TeacherId)
        .OnDelete(DeleteBehavior.Cascade);
    ```
- **`MainSchoolsManagementSystem/Data/Attendance.cs`**:
  - Lines 5-10:
    ```csharp
    public enum AttendanceStatus
    {
        Present = 0,
        Late = 1,
        Absent = 2
    }
    ```
  - Lines 12-20:
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
- **`MainSchoolsManagementSystem/Data/SystemSetting.cs`**:
  - Lines 5-10:
    ```csharp
    public class SystemSetting
    {
        public int Id { get; set; }
        public TimeSpan DailyDeadline { get; set; } = new TimeSpan(8, 30, 0);
        public bool MaintenanceMode { get; set; } = false;
    }
    ```

### B. User Identity Retrieval
- **`MainSchoolsManagementSystem/Components/Pages/TeacherDashboard.razor`**:
  - Line 3: `@inject AuthenticationStateProvider AuthStateProvider`
  - Line 4: `@inject UserManager<ApplicationUser> UserManager`
  - Lines 40-47:
    ```csharp
    var authState = await AuthStateProvider.GetAuthenticationStateAsync();
    var user = authState.User;

    if (user.Identity?.IsAuthenticated == true)
    {
        var appUser = await UserManager.GetUserAsync(user);
        userDisplayName = appUser?.FullName ?? user.Identity.Name ?? "Teacher";
    }
    ```

### C. Timezone and Local Time Handling
- **`MainSchoolsManagementSystem/Components/Pages/Headmaster/Attendance.razor`**:
  - Line 90:
    ```razor
    @(record.Attendance?.CheckedInAt?.ToLocalTime().ToString("hh:mm tt") ?? "—")
    ```
  - Line 208:
    ```csharp
    CheckedInAt = (status == AttendanceStatus.Present || status == AttendanceStatus.Late) ? DateTime.UtcNow : null
    ```
  - Line 223:
    ```csharp
    record.Attendance.CheckedInAt = DateTime.UtcNow;
    ```

### D. Navigation Menu Structure
- **`MainSchoolsManagementSystem/Components/Layout/TeacherLayout.razor`**:
  - Lines 17-19:
    ```razor
    <NavLink class='nav-item' href='teacher/attendance' @onclick='CloseSidebar'>
        <svg width='18' height='18' viewBox='0 0 24 24' fill='none' stroke='currentColor' stroke-width='2' stroke-linecap='round' stroke-linejoin='round'><rect x='3' y='3' width='7' height='9'></rect><rect x='14' y='3' width='7' height='5'></rect><rect x='14' y='12' width='7' height='9'></rect><rect x='3' y='16' width='7' height='5'></rect></svg>
        Attendance
    </NavLink>
    ```

### E. Premium Design Styling
- **`MainSchoolsManagementSystem/wwwroot/app.css`**:
  - Lines 310-316: `.glass-panel` style rule.
  - Lines 319-323: `.dashboard-grid` style rule.
  - Lines 326-376: `.stat-card` and its nested/hover styles.
  - Lines 379-403: `.badge` and status-specific badges (`.badge-present`, `.badge-late`, `.badge-absent`).
  - Lines 410-440: `.premium-table` and its nested/hover styles.
  - Lines 458-474: `.form-control-custom` and its focus styles.
  - Lines 477-510: `.btn-premium` and `.btn-secondary-custom` buttons.

### F. Project Testing
- **`SchoolsManagementSystem.sln`**: Contains only the `MainSchoolsManagementSystem` project.
- **`MainSchoolsManagementSystem/Data/DbTestRunner.cs`**:
  - Lines 10-12: `public static class DbTestRunner { public static async Task RunTestsAsync(IServiceProvider serviceProvider) { ... } }`

---

## 2. Logic Chain
1. **Database Access**: By inspecting `ApplicationDbContext.cs` and `Attendance.cs`, we see that we can read and write `Attendance` records using `DbContext.Attendances`. Each attendance record links to a teacher via `TeacherId` and has a `Date` (representing the local day) and `CheckedInAt` (UTC timestamp).
2. **Logged-In User**: As shown in `TeacherDashboard.razor` and `Headmaster/Attendance.razor`, the currently logged-in teacher's `ApplicationUser` object can be fetched asynchronously using `UserManager.GetUserAsync(user)` where `user` is obtained from `AuthStateProvider.GetAuthenticationStateAsync()`.
3. **Timezone Conversion**: `Headmaster/Attendance.razor` demonstrates that the application uses `DateTime.UtcNow` for storing check-in times and `.ToLocalTime()` to convert them to local time for display. Comparing check-in time with the daily deadline requires:
   - Converting the check-in time (UTC) to local time: `var localTime = CheckedInAt.Value.ToLocalTime();`
   - Extracting the time of day: `TimeSpan checkInTime = localTime.TimeOfDay;`
   - Comparing `checkInTime` with `DailyDeadline` (a `TimeSpan` stored in `SystemSetting`).
4. **Navigation Menu**: By reviewing `TeacherLayout.razor`, we find that the `/teacher/attendance` link is already present in the sidebar navigation. No code changes are needed to expose it.
5. **Design System**: By analyzing `app.css` and `Headmaster/Attendance.razor`, we see that the UI should use `.glass-panel` for containers, `.stat-card` inside a `.dashboard-grid` for statistics, `.premium-table` for tables, and `.btn-premium` / `.btn-secondary-custom` for buttons to maintain UI consistency.
6. **Testing**: Since there are no unit test projects in the solution, testing is done manually by running the application, and database relationships can be verified by executing the `DbTestRunner.RunTestsAsync` method by temporarily adding it to `Program.cs`.

---

## 3. Caveats
- **Timezones**: The `.ToLocalTime()` method relies on the server's local timezone. If the server and the teachers are in different timezones, this could lead to incorrect deadline comparisons. However, this is the current behavior of the application.
- **Database Initializer**: The `DbTestRunner` modifies the database and then cleans up. It is not a unit test in the traditional sense, but rather an integration sanity check.
- **`card-premium` Class**: This class is present in several placeholder pages but is not styled in `app.css`. The actual container style to use is `glass-panel`.

---

## 4. Conclusion
We have gathered all necessary facts to implement the Teacher Attendance feature.
- **DB Model**: `Attendance` and `SystemSetting` are configured and ready.
- **User Context**: We can retrieve the logged-in teacher's ID and school via `AuthenticationStateProvider`.
- **Logic**: Check-in status (`Present`/`Late`) will be calculated by comparing the local time of check-in (`DateTime.UtcNow.ToLocalTime().TimeOfDay`) with `SystemSetting.DailyDeadline`.
- **Layout**: The link is already in `TeacherLayout.razor`.
- **Design**: The page will utilize `.glass-panel`, `.btn-premium`, and `.stat-card`.
- **Tests**: No unit test framework is set up; manual testing is required.

---

## 5. Verification Method
To verify these findings:
1. Open `MainSchoolsManagementSystem/Components/Layout/TeacherLayout.razor` and inspect line 17 to confirm the presence of the `/teacher/attendance` link.
2. Open `MainSchoolsManagementSystem/wwwroot/app.css` and search for `.glass-panel` and `.btn-premium` to verify their styling rules.
3. Hook up `DbTestRunner.RunTestsAsync` in `Program.cs` and run the application using `dotnet run` from `MainSchoolsManagementSystem` to verify database relationships.
