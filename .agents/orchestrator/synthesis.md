# Synthesis: LessonPlans Feature Exploration

Based on the investigation by Explorer 1 (`9e38fdcc-4842-4d80-aae2-4ef5c7f0c980`), here is the synthesized analysis of the codebase:

## 1. File Uploads
- **Finding**: There are no existing file upload implementations or helpers. We must implement file upload from scratch using Blazor's `<InputFile>` component.
- **Decision**: Save uploaded files to the folder `uploads/` in the project root (`ContentRootPath`) rather than `wwwroot/uploads/` to prevent public unauthorized access.
- **Naming**:
  - Lesson Plan: `lessonplan_{id}.{ext}`
  - Justification: `justification_{id}.{ext}`

## 2. Daily Deadline & Lateness
- **Finding**: `SystemSettings` table has a single global settings record. The deadline is retrieved via `context.SystemSettings.Select(s => s.DailyDeadline).FirstOrDefaultAsync()`.
- **Decision**: When a teacher submits a lesson plan, query the `DailyDeadline`. Compare the current local time of the submission against this deadline. If late, set `IsLate = true` and require `JustificationText`.

## 3. Serving Files with Extensions
- **Finding**: The database does not store file extensions.
- **Decision**: When displaying or downloading files, scan the uploads directory using `Directory.EnumerateFiles` for files matching `lessonplan_{id}.*` and `justification_{id}.*` to determine their extensions dynamically. Serve the files via a secure, authorized controller/endpoint (e.g. `/api/lesson-plans/{id}/download`).

## 4. DbContext Concurrency
- **Finding**: The design system (`design-system.md` section 10.1) mandates using `IDbContextFactory<ApplicationDbContext>` in Blazor components to avoid concurrency issues.
- **Decision**: We will inject `IDbContextFactory<ApplicationDbContext>` in the new Teacher LessonPlans page. We should also refactor/update the Headmaster LessonPlans page to use the factory pattern if possible, or at least ensure thread safety.

## 5. UI & Styling
- **Finding**: The Teacher portal uses the same layout structure and classes as the Admin and Headmaster portals.
- **Decision**: Use the HelloTwo premium design system classes (`.glass-panel`, `.premium-table`, `.btn-premium`, `.form-control-custom`) for the new Teacher page.
