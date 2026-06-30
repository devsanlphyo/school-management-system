# Handoff Report — Teacher Attendance Feature Exploration

This handoff report summarizes the findings from the read-only investigation of the `SchoolsManagementSystem` codebase, preparing the team for implementing the Teacher Attendance feature.

## 1. Observation
* **Database Context & Models**:
  * `ApplicationDbContext` is at `MainSchoolsManagementSystem/Data/ApplicationDbContext.cs` and exposes `DbSet<Attendance> Attendances` and `DbSet<SystemSetting> SystemSettings`.
  * `Attendance` is at `MainSchoolsManagementSystem/Data/Attendance.cs` with properties: `Id` (int), `TeacherId` (string), `Teacher` (`ApplicationUser`), `Date` (`DateTime`), `CheckedInAt` (`DateTime?`), and `Status` (`AttendanceStatus`).
  * `AttendanceStatus` is an enum in `Attendance.cs` containing `Present = 0`, `Late = 1`, `Absent = 2`.
  * `SystemSetting` is at `MainSchoolsManagementSystem/Data/SystemSetting.cs` with `DailyDeadline` (`TimeSpan`, default `8:30 AM`).
* **User ID Retrieval**:
  * `TeacherDashboard.razor` retrieves the logged-in user by injecting `AuthenticationStateProvider` and `UserManager<ApplicationUser>` and calling `await UserManager.GetUserAsync(user)`.
* **Timezone / Local Times**:
  * `Attendance.Date` is stored as local date (midnight).
  * `Attendance.CheckedInAt` is stored as UTC (`DateTime.UtcNow`).
  * `Headmaster/Attendance.razor` (line 90) renders the local check-in time using `CheckedInAt?.ToLocalTime().ToString("hh:mm tt")`.
* **Navigation / Layout**:
  * `TeacherLayout` is at `MainSchoolsManagementSystem/Components/Layout/TeacherLayout.razor`.
  * The link for `/teacher/attendance` is already defined in `TeacherLayout.razor` (lines 17-19).
* **Premium Design System**:
  * The premium design system classes (such as `.glass-panel`, `.stat-card`, `.badge-present`, `.btn-premium`) are defined in `MainSchoolsManagementSystem/wwwroot/app.css`.
  * Usage examples are observed in `MainSchoolsManagementSystem/Components/Pages/Headmaster/Attendance.razor`.
* **Tests**:
  * No xUnit/NUnit tests exist. A custom static test runner is at `MainSchoolsManagementSystem/Data/DbTestRunner.cs` but is not invoked in `Program.cs`.

## 2. Logic Chain
* **Retrieving User Context**: The teacher's database record (including their `SchoolId`) can be retrieved by getting the `ClaimsPrincipal` from `AuthenticationStateProvider`, then calling `UserManager.GetUserAsync(claimsPrincipal)`.
* **Determining Late Status**:
  * To determine if a teacher's check-in is late, we must compare the time component of their local check-in time with the `DailyDeadline` `TimeSpan`.
  * Since `CheckedInAt` is stored in UTC, we convert it to the local timezone of the server using `.ToLocalTime()` and extract `.TimeOfDay`.
  * Comparing `localCheckedInAt.TimeOfDay > DailyDeadline` gives the exact attendance status (`Late` vs `Present`).
* **Menu Routing**:
  * Since `TeacherLayout.razor` already contains the `<NavLink href="teacher/attendance">`, we only need to create a new page component with `@page "/teacher/attendance"` and `@layout TeacherLayout`.

## 3. Caveats
* **Timezones**: The timezone conversions via `.ToLocalTime()` rely on the server's local system timezone. If the server is in a different timezone than the school, or if multiple schools are in different timezones, this local conversion could produce incorrect results. However, this is the current behavior in the codebase (e.g., in `Headmaster/Attendance.razor`).
* **DailyDeadline**: The `SystemSetting` table has a single row for the entire system. It is assumed that all schools share the same daily deadline.

## 4. Conclusion
The codebase is fully prepared for the implementation of the Teacher Attendance feature. The page should be placed at `/teacher/attendance` using the `TeacherLayout`. It should retrieve the teacher's ID, allow them to check in by creating/updating an `Attendance` record for today (`DateTime.Today`), and determine their status (`Present` or `Late`) by comparing their local check-in time with `SystemSetting.DailyDeadline`. The UI should follow the premium glassmorphic cards and tables defined in `app.css`.

## 5. Verification Method
* To verify the database models and relationships, add a call to `DbTestRunner.RunTestsAsync(app.Services)` in `Program.cs` and run the application:
  ```powershell
  dotnet run --project MainSchoolsManagementSystem
  ```
  Check that the console outputs `[SUCCESS]` for all database relationships.
