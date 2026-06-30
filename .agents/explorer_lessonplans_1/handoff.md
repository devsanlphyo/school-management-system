# Handoff Report — LessonPlans Feature Investigation

This report summarizes the findings from the investigation of the `SchoolsManagementSystem` codebase, focusing on the DbContext definition, SystemSetting queries, and teacher/school identity retrieval.

---

## 1. Observation

### DbContext Definition & Injection
* **File Path**: `c:\Users\sanlphyo\workspace\csharpworkspace\SchoolsManagementSystem\MainSchoolsManagementSystem\Data\ApplicationDbContext.cs`
  ```csharp
  public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
  {
      public DbSet<School> Schools { get; set; }
      public DbSet<LessonPlan> LessonPlans { get; set; }
      public DbSet<SystemSetting> SystemSettings { get; set; }
      ...
  }
  ```
* **Registration** in `c:\Users\sanlphyo\workspace\csharpworkspace\SchoolsManagementSystem\MainSchoolsManagementSystem\Program.cs`:
  ```csharp
  builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
      options.UseSqlServer(connectionString));
  ```
* **Query Patterns**:
  * **Pattern A (Factory)**: Used in `Components/Pages/Admin/Settings.razor`:
    ```razor
    @inject IDbContextFactory<ApplicationDbContext> DbFactory
    ...
    using var context = await DbFactory.CreateDbContextAsync();
    settings = await context.SystemSettings.FirstOrDefaultAsync();
    ```
  * **Pattern B (Direct Injection)**: Used in `Components/Pages/Headmaster/Settings.razor`:
    ```razor
    @inject MainSchoolsManagementSystem.Data.ApplicationDbContext DbContext
    ...
    settings = await DbContext.SystemSettings.FirstOrDefaultAsync();
    ```

### SystemSetting & DailyDeadline
* **File Path**: `c:\Users\sanlphyo\workspace\csharpworkspace\SchoolsManagementSystem\MainSchoolsManagementSystem\Data\SystemSetting.cs`
  ```csharp
  public class SystemSetting
  {
      public int Id { get; set; }
      public TimeSpan DailyDeadline { get; set; } = new TimeSpan(8, 30, 0);
      public bool MaintenanceMode { get; set; } = false;
  }
  ```
* **Seeding Logic** in `c:\Users\sanlphyo\workspace\csharpworkspace\SchoolsManagementSystem\MainSchoolsManagementSystem\Data\DbSeeder.cs`:
  ```csharp
  if (!await context.SystemSettings.AnyAsync())
  {
      context.SystemSettings.Add(new SystemSetting { DailyDeadline = new TimeSpan(8, 30, 0) });
      await context.SaveChangesAsync();
  }
  ```
* **Query Pattern**:
  * In both `Admin/Settings.razor` and `Headmaster/Settings.razor`, settings are retrieved via:
    ```csharp
    settings = await context.SystemSettings.FirstOrDefaultAsync();
    ```

### Logged-in Teacher & School ID Retrieval
* **User Entity Path**: `c:\Users\sanlphyo\workspace\csharpworkspace\SchoolsManagementSystem\MainSchoolsManagementSystem\Data\ApplicationUser.cs`
  ```csharp
  public class ApplicationUser : IdentityUser
  {
      public string FullName { get; set; } = "";
      public int? SchoolId { get; set; }
      public School? School { get; set; }
      ...
  }
  ```
* **Retrieval Pattern** in `Components/Pages/Headmaster/Attendance.razor` (lines 139–147) and `Components/Pages/TeacherDashboard.razor` (lines 40–47):
  ```csharp
  var authState = await AuthStateProvider.GetAuthenticationStateAsync();
  var user = authState.User;

  if (user.Identity?.IsAuthenticated == true)
  {
      var appUser = await UserManager.GetUserAsync(user);
      if (appUser != null)
      {
          string teacherId = appUser.Id;
          int? schoolId = appUser.SchoolId;
      }
  }
  ```

---

## 2. Logic Chain

1. **DbContext Availability**:
   * Since `Program.cs` registers the database context with `AddDbContextFactory<ApplicationDbContext>()`, both `IDbContextFactory<ApplicationDbContext>` and `ApplicationDbContext` are registered in the Dependency Injection (DI) container.
   * This explains why components in the `Admin` namespace can use the factory pattern (`Pattern A`), while components in the `Headmaster` namespace inject the context directly (`Pattern B`).

2. **SystemSetting Singleton Nature**:
   * The `DbSeeder.cs` class inserts exactly one `SystemSetting` record if the table is empty.
   * All pages querying `SystemSettings` use `FirstOrDefaultAsync()` without filtering by a specific ID (such as `Id == 1` or filtering by school).
   * Therefore, the application is designed to have a single, system-wide global configuration record in the `SystemSettings` table. All schools share this same `DailyDeadline`.

3. **User/School Context Resolution**:
   * `ApplicationUser` extends `IdentityUser` (which uses a `string` primary key `Id`).
   * The relation between the user and their school is established via the nullable foreign key `SchoolId` (`int?`).
   * By injecting `AuthenticationStateProvider` and `UserManager<ApplicationUser>`, a component can resolve the current claims principal, fetch the corresponding `ApplicationUser` record, and access `appUser.Id` (teacher's ID) and `appUser.SchoolId` (school ID).

---

## 3. Caveats

* **Concurrency Risk**: Although injecting `ApplicationDbContext` directly (`Pattern B`) is simpler, doing so in highly interactive Blazor Server pages with multiple concurrent async calls (e.g., file uploads running in parallel with UI polling) can cause EF Core thread-safety issues. It is recommended to use `IDbContextFactory` (`Pattern A`) for the new `LessonPlans` feature.
* **Global vs. School-Specific Settings**: Currently, `SystemSetting` is a single global record. If a future requirement demands that different schools have different deadlines, the database schema and query logic will need to be refactored to support school-specific settings.

---

## 4. Conclusion

1. **DbContext**: Defined as `ApplicationDbContext` in `Data/ApplicationDbContext.cs`. For new components, use `@inject IDbContextFactory<ApplicationDbContext> DbFactory` and instantiate the context via a `using` block.
2. **DailyDeadline**: Stored as a `TimeSpan` in `SystemSetting`. There is always a single global record in the `SystemSettings` table, retrieved using `.FirstOrDefaultAsync()`.
3. **Teacher & School ID**: Retrieved by fetching the authenticated `ApplicationUser` using `AuthenticationStateProvider` and `UserManager<ApplicationUser>`. `Id` (string) represents the teacher's ID, and `SchoolId` (int?) represents their school.

---

## 5. Verification Method

To verify these findings and ensure the environment compiles:
1. Navigate to the project directory:
   ```powershell
   cd c:\Users\sanlphyo\workspace\csharpworkspace\SchoolsManagementSystem\MainSchoolsManagementSystem
   ```
2. Build the project to confirm there are no compilation errors:
   ```powershell
   dotnet build
   ```
3. Inspect `DbTestRunner.cs` to verify that the entity relationships (including `LessonPlan` and `ApplicationUser`) are validated during database testing.
