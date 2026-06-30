# Teacher Attendance Feature Exploration & Analysis

## 1. Database Context & Models

### ApplicationDbContext
- **Namespace**: `MainSchoolsManagementSystem.Data`
- **File Path**: `c:\Users\sanlphyo\workspace\csharpworkspace\SchoolsManagementSystem\MainSchoolsManagementSystem\Data\ApplicationDbContext.cs`
- **Inheritance**: `IdentityDbContext<ApplicationUser>`
- **Key Relationships**:
  - `Attendance` is linked to `ApplicationUser` (Teacher) via foreign key `TeacherId` with cascade delete behavior configured in `OnModelCreating`.

### Attendance Model & AttendanceStatus Enum
- **Namespace**: `MainSchoolsManagementSystem.Data`
- **File Path**: `c:\Users\sanlphyo\workspace\csharpworkspace\SchoolsManagementSystem\MainSchoolsManagementSystem\Data\Attendance.cs`
- **Enum definition**:
  ```csharp
  public enum AttendanceStatus
  {
      Present = 0,
      Late = 1,
      Absent = 2
  }
  ```
- **Model properties**:
  - `int Id`: Primary key.
  - `string TeacherId`: Foreign key to `ApplicationUser` (defaults to `string.Empty`).
  - `ApplicationUser? Teacher`: Navigation property to the teacher.
  - `DateTime Date`: The date of the attendance record (stored as local or midnight date).
  - `DateTime? CheckedInAt`: The exact timestamp when the teacher checked in (stored in UTC).
  - `AttendanceStatus Status`: Current attendance status (defaults to `AttendanceStatus.Absent`).

### SystemSetting Model
- **Namespace**: `MainSchoolsManagementSystem.Data`
- **File Path**: `c:\Users\sanlphyo\workspace\csharpworkspace\SchoolsManagementSystem\MainSchoolsManagementSystem\Data\SystemSetting.cs`
- **Model properties**:
  - `int Id`: Primary key.
  - `TimeSpan DailyDeadline`: The cutoff time for a teacher to check in (defaults to `08:30:00` - 8:30 AM).
  - `bool MaintenanceMode`: Flag indicating if the application is under maintenance (defaults to `false`).
- **Note**: `SystemSetting` is a single global record retrieved using `context.SystemSettings.FirstOrDefaultAsync()`.

---

## 2. Currently Logged-in User Retrieval in Blazor

The application uses Blazor Server. The currently logged-in user's identity is retrieved via `AuthenticationStateProvider`.

### Recommended Design System Approach (Section 10.3 in `design-system.md`)
To get the user's ID directly from claims:
```razor
@inject AuthenticationStateProvider AuthStateProvider
@using System.Security.Claims

@code {
    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;
        var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }
}
```

### Fetching the `ApplicationUser` Entity
To fetch the full `ApplicationUser` from the database while complying with the Blazor EF Core thread-safety rules:
1. **Never inject the scoped `ApplicationDbContext` or `UserManager` directly into Blazor components.**
2. Inject `IDbContextFactory<ApplicationDbContext>` and `IServiceProvider`.
3. Retrieve `UserManager` inside a service scope:
```razor
@inject IDbContextFactory<ApplicationDbContext> DbFactory
@inject IServiceProvider ServiceProvider

@code {
    private ApplicationUser? currentUser;

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;
        var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!string.IsNullOrEmpty(userId))
        {
            using var scope = ServiceProvider.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            currentUser = await userManager.FindByIdAsync(userId);
        }
    }
}
```

---

## 3. Timezone & Local Time Handling

- **Database Storage**: Timestamps (like `CheckedInAt`, `SubmittedAt`, `UploadedAt`) are stored in **UTC** (`DateTime.UtcNow`).
- **UI Presentation**: To display times in the local timezone, the application uses `.ToLocalTime()` on the UTC `DateTime` values (e.g., `CheckedInAt?.ToLocalTime().ToString("hh:mm tt")`). This relies on the local timezone of the server running the Blazor Server application.
- **Deadline Comparison**:
  - `DailyDeadline` is stored as a `TimeSpan` representing the local cutoff time of day (e.g., `08:30:00`).
  - To compare a teacher's check-in time against the deadline:
    1. Convert the UTC check-in time (`CheckedInAt`, which is `DateTime.UtcNow`) to local time using `.ToLocalTime()`.
    2. Extract the local time of day (`TimeOfDay`).
    3. Compare `TimeOfDay` with the `DailyDeadline`.
    ```csharp
    var localCheckIn = checkedInAtUtc.ToLocalTime();
    var isLate = localCheckIn.TimeOfDay > dailyDeadline;
    var status = isLate ? AttendanceStatus.Late : AttendanceStatus.Present;
    ```

---

## 4. Navigation & TeacherLayout Structure

### TeacherLayout
- **File Path**: `c:\Users\sanlphyo\workspace\csharpworkspace\SchoolsManagementSystem\MainSchoolsManagementSystem\Components\Layout\TeacherLayout.razor`
- **Structure**:
  - **Sidebar Navigation**: Uses `<nav class='sidebar-nav'>` containing Blazor `<NavLink>` elements with the class `nav-item` and an `@onclick="CloseSidebar"` handler.
  - **Hamburger Toggle**: A mobile navigation toggle `<button class='mobile-nav-toggle' @onclick='ToggleSidebar'>` in the header controls the `isSidebarOpen` sidebar visibility state.
- **Attendance Link**: The link for `/teacher/attendance` is **already implemented** on lines 17-19 of `TeacherLayout.razor`:
  ```razor
  <NavLink class='nav-item' href='teacher/attendance' @onclick='CloseSidebar'>
      <svg width='18' height='18' viewBox='0 0 24 24' fill='none' stroke='currentColor' stroke-width='2' stroke-linecap='round' stroke-linejoin='round'><rect x='3' y='3' width='7' height='9'></rect><rect x='14' y='3' width='7' height='5'></rect><rect x='14' y='12' width='7' height='9'></rect><rect x='3' y='16' width='7' height='5'></rect></svg>
      Attendance
  </NavLink>
  ```

---

## 5. Premium Design System Analysis

The existing Headmaster Attendance page (`MainSchoolsManagementSystem/Components/Pages/Headmaster/Attendance.razor`) provides a reference for styling.

### Key Classes & UI Elements
- **Containers**:
  - Outer page container: `<div class="d-flex flex-column gap-4">`
  - Section panels: `<div class="glass-panel">` (uses `--bg-surface`, `--border-color`, `--radius-lg`, and `--shadow`).
  - Table wrappers: `<div class="glass-panel" style="padding: 0; overflow: hidden;">` to clip corners.
- **Stats**:
  - Grid: `<div class="dashboard-grid" style="grid-template-columns: repeat(auto-fit, minmax(180px, 1fr));">`
  - Stat Card: `<div class="stat-card success">` (or `warning`, `danger`) containing `<span class="stat-label">` and `<span class="stat-value">`.
- **Tables**:
  - `<table class="premium-table">` with `<thead>` and `<tbody>`.
- **Badges**:
  - Status display:
    - Present: `<span class="badge badge-present">Present</span>`
    - Late: `<span class="badge badge-late">Late</span>`
    - Absent: `<span class="badge badge-absent">Absent</span>` or `<span class="badge badge-absent">Absent (No Log)</span>`
- **Buttons**:
  - Action buttons: `<button class="btn-premium">`
  - Secondary/unselected buttons: `<button class="btn-premium btn-secondary-custom">`
- **Feather Icons**:
  - Inline SVGs with `viewBox="0 0 24 24"`, `fill="none"`, `stroke="currentColor"`, and `stroke-width="2"`.

---

## 6. Testing Strategy & Execution

### Existing Tests
- There is only one test file in the codebase: `MainSchoolsManagementSystem/Data/DbTestRunner.cs`.
- It contains a static class/method `DbTestRunner.RunTestsAsync(IServiceProvider serviceProvider)`.
- It performs database integration and relationship validation:
  1. Creates a school, teacher, department, subject, class, assignment, lesson plan, attendance, and leave request.
  2. Saves changes to the database.
  3. Queries the database using `.Include(...)` to verify that all foreign keys and navigation properties map correctly.
  4. Deletes the test records and saves changes again.

### How Tests are Run
- The project **does not** have standard unit test projects (such as xUnit or NUnit) configured in `SchoolsManagementSystem.sln`.
- Running `dotnet test` will not execute anything since there are no test projects.
- `DbTestRunner.RunTestsAsync` is not called in the default codebase startup (`Program.cs`).
- To execute these tests, a developer must temporarily invoke this method in `Program.cs` before `app.Run()`:
  ```csharp
  // Run DB relationship tests on startup
  await MainSchoolsManagementSystem.Data.DbTestRunner.RunTestsAsync(app.Services);
  ```
  The test results are printed directly to the console output during application startup.
