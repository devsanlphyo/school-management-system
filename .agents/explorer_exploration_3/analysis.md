# Analysis of Teacher Attendance Feature

## 1. Database Context and Models
The database context is `ApplicationDbContext` located in `MainSchoolsManagementSystem.Data`.

### ApplicationDbContext
- **Path**: `MainSchoolsManagementSystem/Data/ApplicationDbContext.cs`
- **Namespace**: `MainSchoolsManagementSystem.Data`
- **Relevant DbSets**:
  - `public DbSet<SystemSetting> SystemSettings { get; set; }`
  - `public DbSet<Attendance> Attendances { get; set; }`
- **Model Configuration (OnModelCreating)**:
  - Configures a cascade-delete relationship between `Attendance` and `ApplicationUser` (Teacher):
    ```csharp
    builder.Entity<Attendance>()
        .HasOne(a => a.Teacher)
        .WithMany()
        .HasForeignKey(a => a.TeacherId)
        .OnDelete(DeleteBehavior.Cascade);
    ```

### Attendance Model & Status Enum
- **Path**: `MainSchoolsManagementSystem/Data/Attendance.cs`
- **Namespace**: `MainSchoolsManagementSystem.Data`
- **Attendance Status Enum**:
  ```csharp
  public enum AttendanceStatus
  {
      Present = 0,
      Late = 1,
      Absent = 2
  }
  ```
- **Attendance Class Properties**:
  - `public int Id { get; set; }` — Primary key.
  - `public string TeacherId { get; set; } = string.Empty;` — Foreign key to `ApplicationUser`.
  - `public ApplicationUser? Teacher { get; set; }` — Navigation property.
  - `public DateTime Date { get; set; }` — Represents the date of attendance (usually normalized to local midnight / date-only).
  - `public DateTime? CheckedInAt { get; set; }` — UTC timestamp of the actual check-in. Nullable to support `Absent` status which has no check-in time.
  - `public AttendanceStatus Status { get; set; } = AttendanceStatus.Absent;` — Status of attendance.

### SystemSetting Model
- **Path**: `MainSchoolsManagementSystem/Data/SystemSetting.cs`
- **Namespace**: `MainSchoolsManagementSystem.Data`
- **SystemSetting Class Properties**:
  - `public int Id { get; set; }` — Primary key.
  - `public TimeSpan DailyDeadline { get; set; } = new TimeSpan(8, 30, 0);` — The time-of-day deadline (e.g., 08:30:00) for check-in.
  - `public bool MaintenanceMode { get; set; } = false;` — System-wide maintenance flag.

---

## 2. Logged-In User Retrieval in Blazor
To retrieve the currently logged-in user's details and ID in Blazor components, the application uses ASP.NET Core Identity authentication state injection.

### Injected Services
```razor
@inject AuthenticationStateProvider AuthStateProvider
@inject UserManager<ApplicationUser> UserManager
```

### Retrieval Logic
Inside the component's `@code` block (usually in `OnInitializedAsync`):
```csharp
var authState = await AuthStateProvider.GetAuthenticationStateAsync();
var user = authState.User;

if (user.Identity?.IsAuthenticated == true)
{
    var appUser = await UserManager.GetUserAsync(user);
    if (appUser != null)
    {
        string userId = appUser.Id;
        int? schoolId = appUser.SchoolId;
        string fullName = appUser.FullName;
        // Proceed with using user information
    }
}
```

---

## 3. Timezone and Local Time Handling
- **Database Storage**: The application stores timestamps in UTC (e.g., `CheckedInAt` is populated using `DateTime.UtcNow`).
- **UI Display**: To present timestamps in local time to the user, the application calls `.ToLocalTime()` on the UTC `DateTime` values (e.g., `record.Attendance?.CheckedInAt?.ToLocalTime().ToString("hh:mm tt")`).
- **Comparing Checked-In Time with Daily Deadline**:
  To determine if a teacher is `Present` or `Late`:
  1. Retrieve the local time of check-in:
     `DateTime localCheckedIn = CheckedInAt.Value.ToLocalTime();` (or `DateTime.Now` at the moment of check-in).
  2. Extract the time-of-day component as a `TimeSpan`:
     `TimeSpan checkInTime = localCheckedIn.TimeOfDay;`
  3. Compare `checkInTime` against the `DailyDeadline` from `SystemSetting`:
     ```csharp
     var systemSetting = await DbContext.SystemSettings.FirstOrDefaultAsync();
     var deadline = systemSetting?.DailyDeadline ?? new TimeSpan(8, 30, 0);
     
     AttendanceStatus status = (checkInTime <= deadline) 
         ? AttendanceStatus.Present 
         : AttendanceStatus.Late;
     ```
  4. Save the normalized local date for the `Date` column:
     `Date = DateTime.Today` (or `localCheckedIn.Date`).

---

## 4. TeacherLayout and Navigation
- **Path**: `MainSchoolsManagementSystem/Components/Layout/TeacherLayout.razor`
- **Navigation Menu Structure**:
  - The layout uses a sidebar navigation container: `<aside class='admin-sidebar @(isSidebarOpen ? "open" : "")'>` containing a `<nav class='sidebar-nav'>`.
  - Sidebar toggling on mobile devices is managed via `isSidebarOpen` (boolean) which is triggered by `@onclick='ToggleSidebar'` on the hamburger menu button (`mobile-nav-toggle`) in the header and on the backdrop overlay (`sidebar-overlay`).
  - **Existing Link**: The navigation menu already contains the link to the Teacher Attendance page. It is defined on lines 17-19:
    ```razor
    <NavLink class='nav-item' href='teacher/attendance' @onclick='CloseSidebar'>
        <svg width='18' height='18' viewBox='0 0 24 24' fill='none' stroke='currentColor' stroke-width='2' stroke-linecap='round' stroke-linejoin='round'><rect x='3' y='3' width='7' height='9'></rect><rect x='14' y='3' width='7' height='5'></rect><rect x='14' y='12' width='7' height='9'></rect><rect x='3' y='16' width='7' height='5'></rect></svg>
        Attendance
    </NavLink>
    ```
  - **No changes** are required to the navigation menu layout to expose the attendance link, as it is already configured.

---

## 5. Premium Design System Usage
The main stylesheet is `MainSchoolsManagementSystem/wwwroot/app.css`. The premium design classes available for the Teacher Attendance page include:

- **Containers & Panels**:
  - `.glass-panel`: Serves as a transparent container with borders and drop shadows (replaces standard bootstrap cards).
  - `.dashboard-grid`: A CSS Grid layout for stat cards.
- **Stat Cards**:
  - `.stat-card`: Card with a subtle glow overlay (`::after`) and hover translation.
  - `.stat-label`: Small muted text for the label.
  - `.stat-value`: Large bold font for the numeric metric.
  - Card variants: `success`, `warning`, `danger` (handled via parent styles or custom coloring).
- **Tables**:
  - `.table-container`: Wraps the table to enable horizontal scrolling on small screens.
  - `.premium-table`: Styled table with hover states on rows.
- **Badges**:
  - `.badge`: Inline rounded pill with a dot prefix.
  - `.badge-present` / `.badge-green`: Green background/text.
  - `.badge-late` / `.badge-amber`: Yellow background/text.
  - `.badge-absent` / `.badge-rose`: Red background/text.
- **Buttons**:
  - `.btn-premium`: Primary action button with a purple/indigo linear gradient background and a glowing box shadow.
  - `.btn-secondary-custom`: Secondary action button with a neutral border and transparent background.
- **Form Controls**:
  - `.form-group`: Standard label and input spacing.
  - `.form-label`: Small uppercase muted label text.
  - `.form-control-custom`: Styled input fields with focus glow effects.

*Note: The class `card-premium` used in placeholder files is not defined in `app.css` and can be replaced with `glass-panel` to align with the active styling.*

---

## 6. Project Testing
- **Project Structure**: The solution `SchoolsManagementSystem.sln` contains only one project, `MainSchoolsManagementSystem.csproj`. There are no dedicated unit or integration test projects (e.g., xUnit or NUnit).
- **Database Relationship Tests**:
  - **File**: `MainSchoolsManagementSystem/Data/DbTestRunner.cs`
  - **Mechanism**: Contains `DbTestRunner.RunTestsAsync(IServiceProvider serviceProvider)` which performs programmatic CRUD operations on a test school, teacher, department, subject, class, lesson plan, attendance, and leave request to verify foreign keys and relationships.
  - **How to Run**:
    1. Temporarily hook up the runner in `MainSchoolsManagementSystem/Program.cs` before `app.Run()`:
       ```csharp
       await MainSchoolsManagementSystem.Data.DbTestRunner.RunTestsAsync(app.Services);
       ```
    2. Run the application from the command line:
       ```powershell
       dotnet run --project MainSchoolsManagementSystem
       ```
    3. Observe the output in the console log.
