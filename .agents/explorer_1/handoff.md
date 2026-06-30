# Handoff Report — LessonPlans Feature Investigation

## 1. Observation
* **File Upload Helpers**: A `grep_search` in `MainSchoolsManagementSystem` for `InputFile` returned no matches. A search for `Path.Combine` returned no matches.
* **SystemSetting & DailyDeadline**:
  * In `MainSchoolsManagementSystem/Components/Pages/Admin/Settings.razor` (lines 85-92):
    ```csharp
    settings = await context.SystemSettings.FirstOrDefaultAsync();
    if (settings == null)
    {
        settings = new SystemSetting { DailyDeadline = new TimeSpan(8, 30, 0), MaintenanceMode = false };
        context.SystemSettings.Add(settings);
        await context.SaveChangesAsync();
    }
    ```
  * In `MainSchoolsManagementSystem/Data/DbSeeder.cs` (line 49):
    ```csharp
    context.SystemSettings.Add(new SystemSetting { DailyDeadline = new TimeSpan(8, 30, 0) });
    ```
* **LessonPlan Model**:
  * In `MainSchoolsManagementSystem/Data/LessonPlan.cs` (lines 11-22):
    ```csharp
    public class LessonPlan
    {
        public int Id { get; set; }
        public string TeacherId { get; set; } = string.Empty;
        public ApplicationUser? Teacher { get; set; }
        public DateTime UploadedAt { get; set; }
        public bool IsLate { get; set; }
        public string? JustificationText { get; set; }
        public bool HasJustificationAttachment { get; set; }
        public LessonPlanStatus Status { get; set; } = LessonPlanStatus.Pending;
        public string? Feedback { get; set; }
    }
    ```
* **DbContext & Blazor Concurrency Patterns**:
  * In `design-system.md` (Section 10.1):
    ```markdown
    ### 10.1 DbContext — Always Use Factory
    Never inject the scoped `ApplicationDbContext` directly into Blazor components.
    ```
  * In `MainSchoolsManagementSystem/Components/Pages/Headmaster/LessonPlans.razor` (line 6):
    ```razor
    @inject MainSchoolsManagementSystem.Data.ApplicationDbContext DbContext
    ```
* **Teacher Layout & CSS Styling**:
  * In `MainSchoolsManagementSystem/Components/Layout/TeacherLayout.razor` (lines 5-10):
    ```razor
    <div class='admin-layout'>
        <div class='sidebar-overlay @(isSidebarOpen ? "show" : "")' @onclick='ToggleSidebar'></div>
        <aside class='admin-sidebar @(isSidebarOpen ? "open" : "")'>
            <div class='sidebar-header'>
                <span class='brand-title'>HelloTwo Teacher</span>
            </div>
    ```
  * The classes match those defined in `design-system.md` under section 4.2 (`.admin-layout`, `.admin-sidebar`, `.sidebar-nav`, `.admin-main`, etc.).
* **`wwwroot/uploads` Directory**:
  * Running `list_dir` on `c:\Users\sanlphyo\workspace\csharpworkspace\SchoolsManagementSystem\MainSchoolsManagementSystem\wwwroot` returned:
    ```json
    {"name":"app.css", "sizeBytes":"15453"}
    Summary: This directory contains 0 subdirectories and 1 files.
    ```

---

## 2. Logic Chain
1. **File Upload**: Since there are no matches for `InputFile` or `Path.Combine` in the codebase (Observation 1), we conclude that no file upload logic or helper is currently implemented. The implementer must build this from scratch.
2. **SystemSetting**: The seeder and the settings page only ever retrieve or update the first record in the `SystemSettings` table via `FirstOrDefaultAsync()` (Observation 2). Therefore, the application assumes a single global settings record.
3. **File Extensions**: The `LessonPlan` model does not store file paths or extensions (Observation 3). To support multiple file types (PDF, Word, Images, etc.) without altering the database schema, we must scan the storage directory for files prefixed with the record's ID (e.g., `lessonplan_{id}.*` and `justification_{id}.*`) using `Directory.EnumerateFiles(...)` and resolve their extensions at runtime.
4. **DbContext Pattern**: The design system explicitly mandates using `IDbContextFactory` to avoid concurrency issues (Observation 4). However, `Headmaster/LessonPlans.razor` violates this by injecting `ApplicationDbContext` directly. To ensure thread safety and follow the correct architectural design, the new Teacher LessonPlans component must use `IDbContextFactory<ApplicationDbContext>`.
5. **Styling**: `TeacherLayout.razor` uses the same premium layout shell classes (Observation 5) as the Admin and Headmaster portals. Consequently, any new Teacher portal pages must use the design tokens and component classes (such as `.glass-panel`, `.premium-table`, and `.btn-premium`) specified in `design-system.md`.
6. **Uploads Directory**: The directory listing of `wwwroot` (Observation 6) shows that `wwwroot/uploads` does not exist. It must be created dynamically during file upload or application startup.

---

## 3. Caveats
* **Storage Location Security**: Storing files in `wwwroot/uploads` makes them publicly accessible to anyone who can guess the file name / URL (e.g., `http://localhost/uploads/lessonplan_1.pdf`). If lesson plans contain sensitive information, it is highly recommended to store them outside of `wwwroot` (e.g., in a root-level `uploads/` folder) and serve them via an authorized endpoint.

---

## 4. Conclusion
1. No file upload helpers exist.
2. `SystemSettings` has a single row, and `DailyDeadline` is retrieved via `FirstOrDefaultAsync()`.
3. File extensions should be resolved dynamically by scanning the uploads directory for `lessonplan_{id}.*` and `justification_{id}.*` and served via a secure, authorized endpoint.
4. Use `IDbContextFactory<ApplicationDbContext>` in the new Blazor components instead of injecting the DbContext directly.
5. Apply the HelloTwo premium design system classes (`.glass-panel`, `.premium-table`, `.btn-premium`) on the Teacher portal pages.
6. The `wwwroot/uploads` (or alternative `uploads`) directory does not exist and must be created dynamically.

---

## 5. Verification Method
1. **File Upload Absence**: Run `git grep -i "InputFile"` in the project root to confirm no matches.
2. **System Settings Table**: Inspect `MainSchoolsManagementSystem/Components/Pages/Admin/Settings.razor` around line 86 to verify that `FirstOrDefaultAsync` is used.
3. **Directory Check**: Run `Test-Path "c:\Users\sanlphyo\workspace\csharpworkspace\SchoolsManagementSystem\MainSchoolsManagementSystem\wwwroot\uploads"` in PowerShell to verify it returns `False`.
4. **Build and Test**: Run `dotnet build` in `c:\Users\sanlphyo\workspace\csharpworkspace\SchoolsManagementSystem\MainSchoolsManagementSystem` to ensure the project builds successfully in its current state.
