# Teacher Attendance Feature Exploration Analysis

This document contains the findings and architectural facts gathered during the exploration of the `SchoolsManagementSystem` codebase for implementing the Teacher Attendance feature.

---

## 1. Database Context, Models, and Enums

### Database Context
* **Class**: `ApplicationDbContext`
* **Path**: `MainSchoolsManagementSystem/Data/ApplicationDbContext.cs`
* **Namespace**: `MainSchoolsManagementSystem.Data`
* **Parent Class**: `IdentityDbContext<ApplicationUser>`
* **Relevant DbSets**:
  * `public DbSet<Attendance> Attendances { get; set; }`
  * `public DbSet<SystemSetting> SystemSettings { get; set; }`
* **Entity Relationships**:
  * `Attendance` is linked to `ApplicationUser` (Teacher) via foreign key `TeacherId` with a cascade delete behavior.

### Attendance Model
* **Class**: `Attendance`
* **Path**: `MainSchoolsManagementSystem/Data/Attendance.cs`
* **Namespace**: `MainSchoolsManagementSystem.Data`
* **Properties**:
  * `public int Id { get; set; }` — Primary key.
  * `public string TeacherId { get; set; } = string.Empty;` — Foreign key to `ApplicationUser`.
  * `public ApplicationUser? Teacher { get; set; }` — Navigation property.
  * `public DateTime Date { get; set; }` — Represents the date of the attendance record (stored as the local date at midnight, e.g., `DateTime.Today`).
  * `public DateTime? CheckedInAt { get; set; }` — The exact timestamp when the teacher checked in (stored in UTC).
  * `public AttendanceStatus Status { get; set; } = AttendanceStatus.Absent;` — The status of the attendance record.

### AttendanceStatus Enum
* **Enum**: `AttendanceStatus`
* **Path**: `MainSchoolsManagementSystem/Data/Attendance.cs`
* **Namespace**: `MainSchoolsManagementSystem.Data`
* **Values**:
  * `Present = 0`
  * `Late = 1`
  * `Absent = 2`

### SystemSetting Model
* **Class**: `SystemSetting`
* **Path**: `MainSchoolsManagementSystem/Data/SystemSetting.cs`
* **Namespace**: `MainSchoolsManagementSystem.Data`
* **Properties**:
  * `public int Id { get; set; }` — Primary key.
  * `public TimeSpan DailyDeadline { get; set; } = new TimeSpan(8, 30, 0);` — The local daily time cutoff (defaulting to 08:30:00) before which a check-in is marked `Present`, and after which it is marked `Late`.
  * `public bool MaintenanceMode { get; set; } = false;` — Indicates if the system is in maintenance mode.

---

## 2. Current User ID Retrieval in Blazor

In Blazor components, the logged-in user's details and ID are retrieved using ASP.NET Core Identity services injected into the component.

### Injection
```razor
@inject AuthenticationStateProvider AuthStateProvider
@inject UserManager<ApplicationUser> UserManager
```

### Retrieval Logic (Example from `TeacherDashboard.razor`)
```csharp
protected override async Task OnInitializedAsync()
{
    var authState = await AuthStateProvider.GetAuthenticationStateAsync();
    var user = authState.User;

    if (user.Identity?.IsAuthenticated == true)
    {
        // Option A: Fetch full ApplicationUser entity from database (needed for SchoolId or FullName)
        var appUser = await UserManager.GetUserAsync(user);
        if (appUser != null)
        {
            string teacherId = appUser.Id;
            int? schoolId = appUser.SchoolId;
            string fullName = appUser.FullName;
        }

        // Option B: Retrieve only the User ID string from claims (faster, no database roundtrip)
        string teacherIdFromClaims = UserManager.GetUserId(user);
    }
}
```

---

## 3. Timezones and Local Time Handling

The application stores timestamps and performs date-time comparisons using a mix of UTC and local times:

1. **Date Field (`Attendance.Date`)**:
   * Stored as a local date without time (midnight). For example, `DateTime.Today` is used to seed and record the attendance day.
2. **Check-In Timestamp (`Attendance.CheckedInAt`)**:
   * Stored in UTC using `DateTime.UtcNow`.
3. **Daily Deadline (`SystemSetting.DailyDeadline`)**:
   * Stored as a `TimeSpan` representing local time of day (e.g., `08:30:00` for 8:30 AM).

### Comparison Logic for Check-in
To determine whether a check-in is `Present` or `Late` relative to `DailyDeadline`:
1. Convert the UTC check-in timestamp (`CheckedInAt`) to the local server time using `.ToLocalTime()`:
   ```csharp
   DateTime localCheckIn = checkedInAtUtc.ToLocalTime();
   ```
2. Extract the time-of-day component from the local check-in:
   ```csharp
   TimeSpan checkInTimeOfDay = localCheckIn.TimeOfDay;
   ```
3. Compare with the daily deadline:
   ```csharp
   if (checkInTimeOfDay > dailyDeadline)
   {
       status = AttendanceStatus.Late;
   }
   else
   {
       status = AttendanceStatus.Present;
   }
   ```
4. In the UI, display the check-in time converted to local time:
   ```razor
   @(attendance.CheckedInAt?.ToLocalTime().ToString("hh:mm tt") ?? "—")
   ```

---

## 4. TeacherLayout and Navigation Structure

* **Path**: `MainSchoolsManagementSystem/Components/Layout/TeacherLayout.razor`
* **Layout Structure**:
  * Leverages a sidebar navigation panel (`<aside class='admin-sidebar'>`) and a main content panel (`<main class='admin-main'>`).
  * Hamburger menu is controlled by the `isSidebarOpen` boolean and toggled using the `@onclick='ToggleSidebar'` handler on the `.mobile-nav-toggle` button in the header.
* **Navigation Links**:
  * Links are declared in `<nav class='sidebar-nav'>` using the `<NavLink>` component.
  * **Note**: The link for `/teacher/attendance` is **already defined** in the layout file:
    ```razor
    <NavLink class='nav-item' href='teacher/attendance' @onclick='CloseSidebar'>
        <svg width='18' height='18' viewBox='0 0 24 24' fill='none' stroke='currentColor' stroke-width='2' stroke-linecap='round' stroke-linejoin='round'><rect x='3' y='3' width='7' height='9'></rect><rect x='14' y='3' width='7' height='5'></rect><rect x='14' y='12' width='7' height='9'></rect><rect x='3' y='16' width='7' height='5'></rect></svg>
        Attendance
    </NavLink>
    ```
    Therefore, no layout modifications are required to enable the menu item. We only need to implement the page component at `/teacher/attendance`.

---

## 5. Premium Design System & Existing Attendance Pages

The premium design system classes are defined in `MainSchoolsManagementSystem/wwwroot/app.css`.

### Existing Attendance Page
* **Path**: `MainSchoolsManagementSystem/Components/Pages/Headmaster/Attendance.razor`
* **Route**: `/headmaster/attendance` (conforms to the `AGENTS.md` naming convention using `headmaster` instead of `admin`).

### Design Patterns & Classes
To build a consistent UI for the Teacher Attendance page, the following design elements from `app.css` should be used:

| CSS Class | Usage / Description | Example |
|---|---|---|
| `glass-panel` | Glassmorphic container panel for forms, tables, or sections. | `<div class="glass-panel">...</div>` |
| `dashboard-grid` | Responsive grid layout for cards. | `<div class="dashboard-grid">...</div>` |
| `stat-card` | Cards displaying key metrics, with status modifiers. | `<div class="stat-card success">` (Present)<br>`<div class="stat-card warning">` (Late)<br>`<div class="stat-card danger">` (Absent) |
| `stat-label` | Small label for stat cards. | `<span class="stat-label">Present</span>` |
| `stat-value` | Large bold number inside stat cards. | `<span class="stat-value">12</span>` |
| `table-container` | Wrapper to enable horizontal scrolling on mobile tables. | `<div class="table-container">...</div>` |
| `premium-table` | Stylized table with hover transitions. | `<table class="premium-table">...</table>` |
| `badge` | Small rounded status pill. | `<span class="badge badge-present">Present</span>`<br>`<span class="badge badge-late">Late</span>`<br>`<span class="badge badge-absent">Absent</span>` |
| `btn-premium` | Button styled with a purple-to-indigo gradient and shadow. | `<button class="btn-premium">Check In</button>` |
| `btn-secondary-custom` | Outline button for secondary actions. | `<button class="btn-premium btn-secondary-custom">Cancel</button>` |

---

## 6. Existing Tests and Execution

* **Test Frameworks**: There are no unit, integration, or E2E testing frameworks (such as xUnit, NUnit, MSTest, Playwright, or Cypress) configured in the solution.
* **Custom Integration Test Runner**:
  * **Path**: `MainSchoolsManagementSystem/Data/DbTestRunner.cs`
  * **Type**: Programmatic database relationship integration tests.
  * **Operation**:
    1. Seeds a temporary school, teacher, department, subject, class, assignment, lesson plan, attendance, and leave request.
    2. Executes complex queries with `.Include()` statements to verify all database relationships are functioning properly.
    3. Deletes the seeded records.
    4. Outputs successes and errors to the standard console output.
  * **How to Run**:
    Currently, `DbTestRunner.RunTestsAsync` is not called anywhere. To run it:
    1. Temporarily add the following line in `MainSchoolsManagementSystem/Program.cs` before `app.Run();`:
       ```csharp
       await MainSchoolsManagementSystem.Data.DbTestRunner.RunTestsAsync(app.Services);
       ```
    2. Run the application from the command line:
       ```powershell
       dotnet run --project MainSchoolsManagementSystem
       ```
    3. Observe the output in the console.
