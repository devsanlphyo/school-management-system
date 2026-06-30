# LessonPlans Feature Design Analysis

This report provides the technical findings and design recommendations for implementing the **LessonPlans** feature in the `SchoolsManagementSystem` codebase.

---

## 1. Existing File Upload Implementations or Helpers
* **Finding**: There are **no** existing file upload implementations or file system helpers in the project.
* **Evidence**:
  * A grep search for `InputFile` (the standard Blazor file upload component) returned zero results.
  * A grep search for `Path.Combine` and `Directory.Create` returned zero results.
  * The only reference to file-related actions is in `IdentityComponentsEndpointRouteBuilderExtensions.cs` (line 107) which returns a static personal data JSON using `TypedResults.File(fileBytes, ...)`.
* **Recommendation**: Implement file uploading from scratch in the Teacher's LessonPlans component using the Blazor `<InputFile>` component. The uploaded file streams should be read and written to the file system.

---

## 2. DailyDeadline in SystemSetting
* **Finding**: The `DailyDeadline` is queried from a single record in the `SystemSettings` table. There is always a single record representing global settings.
* **Evidence**:
  * In `Components/Pages/Admin/Settings.razor` (lines 85–92):
    ```csharp
    protected override async Task OnInitializedAsync()
    {
        using var context = await DbFactory.CreateDbContextAsync();
        settings = await context.SystemSettings.FirstOrDefaultAsync();
        if (settings == null)
        {
            settings = new SystemSetting { DailyDeadline = new TimeSpan(8, 30, 0), MaintenanceMode = false };
            context.SystemSettings.Add(settings);
            await context.SaveChangesAsync();
        }
        ...
    }
    ```
  * In `DbSeeder.cs` (line 49) and `DbInitializer.cs` (line 44), exactly one `SystemSetting` record is seeded:
    ```csharp
    context.SystemSettings.Add(new SystemSetting { DailyDeadline = new TimeSpan(8, 30, 0) });
    ```
* **Recommendation**: When a teacher uploads a lesson plan, query the deadline using `await context.SystemSettings.Select(s => s.DailyDeadline).FirstOrDefaultAsync()`. Compare the current local time of the submission against this deadline to determine if `IsLate` should be set to `true`.

---

## 3. Handling File Extensions and Serving Uploaded Files
* **Finding**: The `LessonPlan` model does not store the file extension or path. It only contains boolean flags: `HasJustificationAttachment` and `IsLate`.
* **Design Proposal**:
  1. **Storage Location**: Store files in a folder outside of `wwwroot` (e.g., `uploads/` in the project root) rather than `wwwroot/uploads/`. Files stored in `wwwroot` are served publicly by the static files middleware without authorization, which is a security risk for sensitive school files.
  2. **Naming Convention**: Save files using the unique `LessonPlan.Id`:
     * Lesson Plan file: `lessonplan_{id}.{extension}`
     * Justification file: `justification_{id}.{extension}`
  3. **Locating Files (Scanning)**:
     To find a file when its extension is unknown, use `Directory.EnumerateFiles` to search for the matching prefix:
     ```csharp
     var uploadsFolder = Path.Combine(builder.Environment.ContentRootPath, "uploads");
     var filePath = Directory.EnumerateFiles(uploadsFolder, $"lessonplan_{id}.*").FirstOrDefault();
     ```
  4. **Serving Files (Secure Endpoint)**:
     Do not expose direct file links. Instead, create a Minimal API or Controller endpoint (e.g., `/api/lesson-plans/{id}/download` and `/api/lesson-plans/{id}/justification`).
     The endpoint will:
     * Retrieve the `LessonPlan` from the database.
     * Verify authorization (e.g., ensure the logged-in user is the Teacher who uploaded it, or an Admin/Headmaster/Officer of the same school).
     * Locate the file on disk using the prefix scan.
     * Determine the MIME type using `FileExtensionContentTypeProvider`:
       ```csharp
       var provider = new FileExtensionContentTypeProvider();
       if (!provider.TryGetContentType(filePath, out var contentType))
       {
           contentType = "application/octet-stream";
       }
       return Results.File(filePath, contentType, fileDownloadName: Path.GetFileName(filePath));
       ```

---

## 4. DbContext Definition and Blazor Query Patterns
* **Finding**: `ApplicationDbContext` is defined in `MainSchoolsManagementSystem/Data/ApplicationDbContext.cs`.
* **Query Patterns**:
  * **Design System Rule**: According to `design-system.md` (Section 10.1), components should **always** use `IDbContextFactory<ApplicationDbContext>`:
    ```csharp
    @inject IDbContextFactory<ApplicationDbContext> DbFactory
    
    using var db = await DbFactory.CreateDbContextAsync();
    var schools = await db.Schools.ToListAsync();
    ```
  * **Inconsistency**: In the existing `Components/Pages/Headmaster/LessonPlans.razor` (line 6), `ApplicationDbContext` is injected directly:
    ```razor
    @inject MainSchoolsManagementSystem.Data.ApplicationDbContext DbContext
    ```
* **Recommendation**: For the new Teacher LessonPlans implementation, adhere strictly to the design system's rule by using `IDbContextFactory<ApplicationDbContext>`. Injecting the DbContext directly in Blazor Server components can lead to concurrency issues and memory leaks since Blazor components have longer lifetimes.

---

## 5. CSS Styling Conventions & Design Tokens for Teacher Portal
* **Finding**: The `TeacherLayout.razor` uses the exact same layout structure and classes as the Admin and Headmaster portals. Therefore, the premium design tokens and classes defined in `design-system.md` should be used.
* **Key Classes & Patterns**:
  * **Page Wrapper**:
    ```html
    <div class="d-flex flex-column" style="gap: 2rem;">
        <!-- Page Header -->
        <div style="display: flex; justify-content: space-between; align-items: center; flex-wrap: wrap; gap: 1rem;">
            <div>
                <h2 style="font-size: 1.85rem; font-weight: 900; color: var(--text-primary); margin-bottom: 0.4rem;">Lesson Plans</h2>
                <p style="color: var(--text-muted); font-size: 0.92rem;">Upload and manage your weekly lesson plans.</p>
            </div>
            <button class="btn-premium" @onclick="OpenUploadModal">
                <!-- SVG Icon --> Upload Lesson Plan
            </button>
        </div>
    </div>
    ```
  * **Cards (Panels)**: Use `.glass-panel` for containers.
  * **Tables**: Wrap in a `.glass-panel` with `style="padding: 0; overflow: hidden;"` and use `<table class="premium-table">`.
  * **Badges**:
    * On Time: `<span class="badge badge-present">On Time</span>`
    * Late: `<span class="badge badge-late">Late Submission</span>`
    * Pending: `<span class="badge badge-pending">Pending Review</span>`
    * Reviewed / Approved: `<span class="badge badge-present">Reviewed</span>`
  * **Buttons**:
    * Primary: `<button class="btn-premium">Submit</button>`
    * Secondary/Cancel: `<button class="btn-premium btn-secondary-custom">Cancel</button>`
  * **Forms**: Use `.form-group`, `.form-label`, and `.form-control-custom`.

---

## 6. Directory `wwwroot/uploads` Existence
* **Finding**: The `wwwroot/uploads` directory does **not** exist. The `wwwroot` folder currently only contains `app.css`.
* **Recommendation**:
  * The application should create the uploads directory dynamically before saving any files:
    ```csharp
    if (!Directory.Exists(uploadsFolder))
    {
        Directory.CreateDirectory(uploadsFolder);
    }
    ```
  * As discussed in Section 3, it is recommended to create this folder in the project root (`ContentRootPath`) rather than under `wwwroot` to ensure files are not publicly accessible without authentication.
