# Handoff Report — LessonPlans File Upload & Serving Investigation

## 1. Observation
- **File Upload Helpers & Code**:
  - A case-insensitive search for `InputFile`, `IFormFile`, and `System.IO` across the `MainSchoolsManagementSystem` codebase returned no matches.
  - The only file-serving reference is in `IdentityComponentsEndpointRouteBuilderExtensions.cs` (line 107):
    ```csharp
    TypedResults.File(fileBytes, "text/json", "PersonalData.json")
    ```
    which serves a static JSON file from memory without accessing the local disk.
- **Uploads Directory**:
  - The `wwwroot/uploads` directory does not exist. The `wwwroot` directory contains only `app.css`.
- **Database Model**:
  - `MainSchoolsManagementSystem/Data/LessonPlan.cs` contains the following properties:
    ```csharp
    public int Id { get; set; }
    public string TeacherId { get; set; } = string.Empty;
    public ApplicationUser? Teacher { get; set; }
    public DateTime UploadedAt { get; set; }
    public bool IsLate { get; set; }
    public string? JustificationText { get; set; }
    public bool HasJustificationAttachment { get; set; }
    public LessonPlanStatus Status { get; set; } = LessonPlanStatus.Pending;
    public string? Feedback { get; set; }
    ```
    No fields for storing file path, file name, or file extension are present.
- **Headmaster Page UI**:
  - `MainSchoolsManagementSystem/Components/Pages/Headmaster/LessonPlans.razor` displays the list of lesson plans and a review modal, but contains no links or buttons to download or view the lesson plan or justification attachment files.

---

## 2. Logic Chain
1. **File Upload Mechanics (Scratch Implementation)**:
   - Because there are no existing file-handling helper classes, components, or directories in the project (Observation 1 & 2), the entire file upload flow (using Blazor's `<InputFile>` component) and saving logic must be implemented from scratch.
2. **Directory Choice (Security)**:
   - Storing uploaded files in `wwwroot/uploads` (Observation 2) means they are publicly accessible via `app.UseStaticFiles()` without authentication. Since lesson plans and justifications contain sensitive school/personal data, we must store files in a secure directory outside `wwwroot` (e.g., `uploads/` in the project root) and serve them via an authorized endpoint.
3. **Handling File Extensions**:
   - Since the database does not store the file extension (Observation 3), we have two choices:
     - **Design A (No DB changes)**: When serving a file, scan the directory for `lessonplan_{id}.*` using `Directory.EnumerateFiles`. This is $O(N)$ and degrades in performance as the number of files grows. It also forces users to download files named `lessonplan_{id}.{ext}` rather than their original name.
     - **Design B (Schema Extension - Recommended)**: Add `LessonPlanFileName`, `LessonPlanExtension`, `JustificationFileName`, and `JustificationExtension` to the `LessonPlan` model. This allows $O(1)$ file verification on disk and enables serving the file with its original name.

---

## 3. Caveats
- We assume that the database schema can be modified and migrated using EF Core. If schema migrations are restricted, **Design A (Directory Scanning)** must be used.
- Access control logic in the serving endpoint assumes that the `Teacher` relationship is loaded on the `LessonPlan` record to verify the teacher's `SchoolId`.

---

## 4. Conclusion
We recommend:
1. **Secure Storage**: Save uploaded files to a `/uploads` folder in the project root (`ContentRootPath`).
2. **Model Extension**: Add `LessonPlanFileName`, `LessonPlanExtension`, `JustificationFileName`, and `JustificationExtension` to `LessonPlan.cs` and run a migration.
3. **Authorized Endpoint**: Create a secure Minimal API endpoint `/api/lessonplans/{id}/download` and `/api/lessonplans/{id}/justification/download` that validates the user's role and school ID before serving the file.
4. **UI Integration**: Add download buttons in `Headmaster/LessonPlans.razor` (both on the table and in the modal) and in `Teacher/LessonPlans.razor`.

---

## 5. Verification Method
- **Inspect Files**:
  - Open `MainSchoolsManagementSystem/Data/LessonPlan.cs` and verify the lack of file metadata fields.
  - Open `MainSchoolsManagementSystem/Components/Pages/Headmaster/LessonPlans.razor` and confirm that there are no file download/view links.
- **Review Design**:
  - Review the proposed secure endpoint code in `analysis.md` to verify it meets ASP.NET Core authorization and file serving standards.
