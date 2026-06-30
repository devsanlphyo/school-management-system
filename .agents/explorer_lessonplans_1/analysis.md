# LessonPlans Feature Investigation and Design Analysis

This document provides a detailed analysis of the `SchoolsManagementSystem` codebase to support the design and implementation of the **LessonPlans** feature.

---

## 1. DbContext Definition & Query Patterns

### DbContext Definition
The application's database context is defined in:
* **Path**: `c:\Users\sanlphyo\workspace\csharpworkspace\SchoolsManagementSystem\MainSchoolsManagementSystem\Data\ApplicationDbContext.cs`

`ApplicationDbContext` inherits from `IdentityDbContext<ApplicationUser>` and exposes the following `DbSet` properties relevant to this feature:
* `public DbSet<LessonPlan> LessonPlans { get; set; }`
* `public DbSet<SystemSetting> SystemSettings { get; set; }`
* `public DbSet<School> Schools { get; set; }`

### Query Patterns in Blazor Components
The codebase uses two distinct patterns for database access within Razor components, depending on the section:

#### Pattern A: `IDbContextFactory<ApplicationDbContext>` (Admin Components)
In the `/Components/Pages/Admin/` directory (e.g., `Settings.razor`, `Users.razor`, `Schools.razor`), components inject the factory and create short-lived contexts:
```razor
@inject IDbContextFactory<ApplicationDbContext> DbFactory

@code {
    protected override async Task OnInitializedAsync()
    {
        using var context = await DbFactory.CreateDbContextAsync();
        var settings = await context.SystemSettings.FirstOrDefaultAsync();
    }
}
```
* **Pros**: Prevents concurrency and lifetime issues on long-lived Blazor Server circuits, especially when multiple async operations run in parallel.

#### Pattern B: Direct `ApplicationDbContext` Injection (Headmaster Components)
In the `/Components/Pages/Headmaster/` directory (e.g., `Settings.razor`, `Attendance.razor`, `LessonPlans.razor`), components inject the context directly as a scoped service:
```razor
@inject MainSchoolsManagementSystem.Data.ApplicationDbContext DbContext

@code {
    protected override async Task OnInitializedAsync()
    {
        var settings = await DbContext.SystemSettings.FirstOrDefaultAsync();
    }
}
```
* **Pros**: Simpler code syntax (no `using` blocks).
* **Cons**: Risk of EF Core concurrency exceptions if multiple async calls (e.g., event handlers or background updates) attempt to use the same `DbContext` instance simultaneously.

### Recommendation for `Teacher/LessonPlans.razor`
For the new teacher-facing LessonPlans component, it is highly recommended to use **Pattern A (`IDbContextFactory`)**. Since file uploads and form submissions involve asynchronous operations, using a short-lived context via `IDbContextFactory` ensures thread safety and prevents circuit crashes.

---

## 2. SystemSetting & DailyDeadline Retrieval

### Entity Definition
The `SystemSetting` entity is defined in:
* **Path**: `c:\Users\sanlphyo\workspace\csharpworkspace\SchoolsManagementSystem\MainSchoolsManagementSystem\Data\SystemSetting.cs`

```csharp
public class SystemSetting
{
    public int Id { get; set; }
    public TimeSpan DailyDeadline { get; set; } = new TimeSpan(8, 30, 0);
    public bool MaintenanceMode { get; set; } = false;
}
```

### Table Cardinality & Single-Record Constraint
* **Cardinality**: There is **always a single global record** in the `SystemSettings` table.
* **Seeding**: The database seeding logic in `DbSeeder.cs` (lines 46–49) ensures a default record exists:
  ```csharp
  if (!await context.SystemSettings.AnyAsync())
  {
      context.SystemSettings.Add(new SystemSetting { DailyDeadline = new TimeSpan(8, 30, 0) });
      await context.SaveChangesAsync();
  }
  ```
* **Querying**: Since there is only one global record, components query it without filtering by ID:
  ```csharp
  var settings = await context.SystemSettings.FirstOrDefaultAsync();
  ```
* **Initialization Fallback**: If for any reason the database query returns `null`, the code should fall back to a default value:
  ```csharp
  var deadline = (await context.SystemSettings.FirstOrDefaultAsync())?.DailyDeadline ?? new TimeSpan(8, 30, 0);
  ```

### Usage for Lesson Plan "Late" Evaluation
To determine if a lesson plan is submitted late, the current local time of the submission must be compared against `DailyDeadline`:
```csharp
// Get the current local time
DateTime localNow = DateTime.Now; 
TimeSpan submissionTime = localNow.TimeOfDay;

// Fetch the deadline
using var context = await DbFactory.CreateDbContextAsync();
var settings = await context.SystemSettings.FirstOrDefaultAsync();
TimeSpan deadline = settings?.DailyDeadline ?? new TimeSpan(8, 30, 0);

// Evaluate lateness
bool isLate = submissionTime > deadline;
```

---

## 3. Teacher and School ID Retrieval in Razor Components

To retrieve the currently logged-in teacher's details, the application uses ASP.NET Core Identity authentication state.

### Implementation Pattern
The standard pattern in the codebase is as follows:

1. **Inject Services**:
   ```razor
   @inject AuthenticationStateProvider AuthStateProvider
   @inject UserManager<ApplicationUser> UserManager
   @using Microsoft.AspNetCore.Components.Authorization
   @using Microsoft.AspNetCore.Identity
   ```

2. **Retrieve in `OnInitializedAsync`**:
   ```csharp
   private string? teacherId;
   private int? schoolId;
   private string? teacherName;

   protected override async Task OnInitializedAsync()
   {
       var authState = await AuthStateProvider.GetAuthenticationStateAsync();
       var user = authState.User;

       if (user.Identity?.IsAuthenticated == true)
       {
           var appUser = await UserManager.GetUserAsync(user);
           if (appUser != null)
           {
               teacherId = appUser.Id;          // string (Primary Key of ApplicationUser)
               schoolId = appUser.SchoolId;      // int? (Nullable Foreign Key to Schools)
               teacherName = appUser.FullName;  // string (Teacher's display name)
           }
       }
   }
   ```

---

## 4. Architectural Recommendations for the LessonPlans Feature

### A. Teacher Upload Page (`/teacher/lessonplans`)
1. **Context Lifecycle**: Use `@inject IDbContextFactory<ApplicationDbContext> DbFactory`.
2. **Access Control**: Add `@attribute [Authorize(Roles = "Teacher")]`.
3. **Data Scope**: 
   * Retrieve the `teacherId` using the pattern above.
   * Query the database to list only the lesson plans belonging to the logged-in teacher:
     ```csharp
     using var context = await DbFactory.CreateDbContextAsync();
     var myPlans = await context.LessonPlans
         .Where(lp => lp.TeacherId == teacherId)
         .OrderByDescending(lp => lp.UploadedAt)
         .ToListAsync();
     ```
4. **Lateness Evaluation**:
   * During upload, compare `DateTime.Now.TimeOfDay` against the global `SystemSetting.DailyDeadline`.
   * If `isLate` is true, display a warning in the UI, prompt the teacher for a **Justification Text**, and optionally allow uploading an attachment.

### B. Headmaster Review Page (`/headmaster/lessonplans`)
1. **Access Control**: Add `@attribute [Authorize(Roles = "Admin,Headmaster,Officer")]`.
2. **Data Scope**:
   * Retrieve the headmaster's `schoolId`.
   * Filter lesson plans to show only those submitted by teachers in the same school:
     ```csharp
     using var context = await DbFactory.CreateDbContextAsync();
     var schoolPlans = await context.LessonPlans
         .Include(lp => lp.Teacher)
         .Where(lp => lp.Teacher.SchoolId == schoolId)
         .OrderByDescending(lp => lp.UploadedAt)
         .ToListAsync();
     ```
3. **Action Capability**:
   * Allow the Headmaster to review pending plans (`Status == LessonPlanStatus.Pending`).
   * Provide a modal/form to enter `Feedback` and change `Status` to `LessonPlanStatus.Reviewed`.
