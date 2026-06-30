# LessonPlans File Upload & Serving Analysis

This report details the findings and design recommendations for the file upload, storage, and serving mechanics for the **LessonPlans** feature in the `SchoolsManagementSystem` project.

---

## Executive Summary
1. **Existing Implementations**: There are **no** existing file upload helpers, file-system utilities, or file storage implementations in the codebase. Both the UI and backend file-handling mechanisms must be built from scratch.
2. **Uploads Directory**: The `wwwroot/uploads` directory does **not** exist. We recommend creating an `uploads` folder in the **project root** (`ContentRootPath`) rather than in `wwwroot` to secure uploaded files and prevent public, unauthorized access.
3. **File Extension Handling**: Although the `LessonPlan` database model does not store file extensions, we can either:
   - **Scan the directory** (`Directory.EnumerateFiles`) to find files matching `lessonplan_{id}.*` (no database changes).
   - **Extend the database model** to store the file extension and original filename (recommended for security, performance, and user experience).

---

## 1. Existing File Upload Implementations and Helpers
A thorough investigation of the `MainSchoolsManagementSystem` project was conducted to locate any existing file upload components or helpers:
- **Blazor UI Components**: Checked for `<InputFile>` (the standard Blazor file upload component). No occurrences exist in the active codebase (only mentioned in `.agents/explorer_1/analysis.md`).
- **Backend API / Controllers**: Checked for `IFormFile` or controller endpoints that handle file streams. None exist.
- **File System Utilities**: Checked for references to `System.IO` (like `FileStream`, `Directory.CreateDirectory`, `Path.Combine`, or `File.WriteAllBytes`). No file-system operations are present.
- **Static File Serving**: The only file-related code in the application is in `IdentityComponentsEndpointRouteBuilderExtensions.cs` (line 107), which generates a JSON file from an in-memory byte array for downloading personal data using:
  ```csharp
  TypedResults.File(fileBytes, "text/json", "PersonalData.json")
  ```
  This does not interact with the local file system.

**Conclusion**: No file upload infrastructure exists. It must be implemented from scratch.

---

## 2. Directory `wwwroot/uploads` Existence and Handling
The directory `wwwroot/uploads` does not exist. The `wwwroot` folder currently only contains a single file: `app.css`.

We evaluated two options for handling the upload directory:

### Option A: Public Directory (`wwwroot/uploads`)
- **Location**: `Path.Combine(builder.Environment.WebRootPath, "uploads")`
- **Creation**: Checked and created dynamically on file upload:
  ```csharp
  var uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "uploads");
  if (!Directory.Exists(uploadsFolder))
  {
      Directory.CreateDirectory(uploadsFolder);
  }
  ```
- **Access**: Files are served automatically by `app.UseStaticFiles()` and can be accessed publicly via a direct URL (e.g., `https://localhost:xxxx/uploads/lessonplan_1.pdf`).
- **Security Rating**: **Unsafe (High Risk)**. Anyone can download any lesson plan or justification attachment by guessing or brute-forcing the URL (e.g., `/uploads/lessonplan_1.pdf`, `/uploads/justification_1.pdf`).

### Option B: Secure Directory (Root `/uploads` outside `wwwroot`) — *Recommended*
- **Location**: `Path.Combine(builder.Environment.ContentRootPath, "uploads")`
- **Creation**: Created dynamically when a file is uploaded:
  ```csharp
  var uploadsFolder = Path.Combine(builder.Environment.ContentRootPath, "uploads");
  if (!Directory.Exists(uploadsFolder))
  {
      Directory.CreateDirectory(uploadsFolder);
  }
  ```
- **Access**: Stored outside the web root, meaning they are inaccessible via direct URL. They can only be served via a custom, authorized endpoint (e.g., a Minimal API endpoint in `Program.cs` or a controller) that checks if the logged-in user has permission (e.g., is the teacher who uploaded it, or an authorized administrator/headmaster of the same school).
- **Security Rating**: **Safe (Recommended)**.

---

## 3. Handling File Extensions and Serving Files
Since the `LessonPlan` database model does not store file extensions, we analyzed two design approaches to find and serve files:

### Design A: Directory Scanning (No DB Schema Changes)
In this approach, the file extension is resolved by scanning the physical storage directory.

#### 1. File Upload (Writing to disk)
Because we use the `LessonPlan.Id` to name the file, the record must first be saved to the database:
1. Save the `LessonPlan` entity to the database to generate the `Id`.
2. Extract the file extension from the uploaded file (e.g., `.pdf` or `.docx`).
3. To prevent stale files (e.g., if a teacher replaces a `.docx` file with a `.pdf` file), scan and delete any existing files matching `lessonplan_{id}.*` or `justification_{id}.*` in the uploads folder.
4. Save the file stream to `uploads/lessonplan_{id}{extension}`.

#### 2. Finding and Serving Files
1. Create a secure Minimal API endpoint in `Program.cs` (e.g., `/api/lessonplans/{id:int}/download`).
2. Authorize the request (check if the user is authenticated and belongs to the same school as the lesson plan's teacher).
3. Scan the directory to find the file:
   ```csharp
   var uploadsFolder = Path.Combine(builder.Environment.ContentRootPath, "uploads");
   var filePath = Directory.EnumerateFiles(uploadsFolder, $"lessonplan_{id}.*").FirstOrDefault();
   if (filePath == null) return Results.NotFound("File not found.");
   ```
4. Resolve the content type:
   ```csharp
   var provider = new FileExtensionContentTypeProvider();
   if (!provider.TryGetContentType(filePath, out var contentType))
   {
       contentType = "application/octet-stream";
   }
   ```
5. Serve the file using:
   ```csharp
   return Results.File(filePath, contentType, fileDownloadName: Path.GetFileName(filePath));
   ```

#### Pros & Cons
* **Pros**:
  - No database migration or schema changes required.
* **Cons**:
  - **Performance**: Directory scanning (`Directory.EnumerateFiles`) is an $O(N)$ operation. As the number of lesson plans grows, performance will degrade.
  - **User Experience**: The downloaded file will be named `lessonplan_12.pdf` instead of the original user-friendly filename (e.g., `Grade5_Math_Week3.pdf`).
  - **Fragility**: If multiple files match the pattern due to an error, `FirstOrDefault()` may pick the wrong file.

---

### Design B: Database Schema Extension — *Recommended*
This approach extends the database model to store the file extension and the original filename.

#### 1. Database Schema Changes
Add the following properties to the `LessonPlan` class:
```csharp
public string? LessonPlanFileName { get; set; }      // e.g., "Grade5_Math_Week3.pdf"
public string? LessonPlanExtension { get; set; }     // e.g., ".pdf"
public string? JustificationFileName { get; set; }   // e.g., "Medical_Certificate.jpg"
public string? JustificationExtension { get; set; }  // e.g., ".jpg"
```

#### 2. File Upload
1. Save the `LessonPlan` entity to the database to generate the `Id`.
2. Save the file stream to `uploads/lessonplan_{id}{extension}`.
3. Update the `LessonPlan` record with the extension and original filename, then call `await dbContext.SaveChangesAsync()`.

#### 3. Finding and Serving Files
1. Retrieve the `LessonPlan` record from the database.
2. Formulate the exact path:
   ```csharp
   var filePath = Path.Combine(uploadsFolder, $"lessonplan_{id}{plan.LessonPlanExtension}");
   if (!File.Exists(filePath)) return Results.NotFound("File not found.");
   ```
3. Resolve the content type using `FileExtensionContentTypeProvider`.
4. Serve the file using the original filename:
   ```csharp
   return Results.File(filePath, contentType, fileDownloadName: plan.LessonPlanFileName);
   ```

#### Pros & Cons
* **Pros**:
  - **High Performance**: $O(1)$ file check; no directory scanning.
  - **Excellent User Experience**: The downloaded file retains its original name.
  - **Robustness**: No risk of serving the wrong file due to stale files or directory scanning anomalies.
* **Cons**:
  - Requires a database migration.

---

## Recommended Implementation Plan
We strongly recommend **Option B (Secure Storage outside `wwwroot`)** coupled with **Design B (Database Schema Extension)**. 

### Secure Endpoint Design (Draft)
```csharp
// Program.cs
app.MapGet("/api/lessonplans/{id:int}/download", async (
    int id, 
    ClaimsPrincipal user, 
    IDbContextFactory<ApplicationDbContext> dbFactory, 
    IWebHostEnvironment env) => 
{
    using var db = await dbFactory.CreateDbContextAsync();
    var plan = await db.LessonPlans.Include(lp => lp.Teacher).FirstOrDefaultAsync(lp => lp.Id == id);
    if (plan == null) return Results.NotFound("Lesson plan not found.");

    // Authorization: Ensure user is authenticated and has access to this school
    var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    if (string.IsNullOrEmpty(userId)) return Results.Unauthorized();

    var currentUser = await db.Users.FindAsync(userId);
    if (currentUser == null) return Results.Forbid();

    // Check if user is the teacher who uploaded it OR is an Admin/Headmaster/Officer in the same school
    bool isAuthorized = user.IsInRole("Admin") || 
                       (currentUser.SchoolId == plan.Teacher.SchoolId && 
                        (user.IsInRole("Headmaster") || user.IsInRole("Officer") || plan.TeacherId == userId));

    if (!isAuthorized) return Results.Forbid();

    var uploadsFolder = Path.Combine(env.ContentRootPath, "uploads");
    var filePath = Path.Combine(uploadsFolder, $"lessonplan_{id}{plan.LessonPlanExtension}");

    if (!File.Exists(filePath)) return Results.NotFound("Physical file not found.");

    var provider = new FileExtensionContentTypeProvider();
    if (!provider.TryGetContentType(filePath, out var contentType))
    {
        contentType = "application/octet-stream";
    }

    return Results.File(filePath, contentType, fileDownloadName: plan.LessonPlanFileName);
}).RequireAuthorization();
```
