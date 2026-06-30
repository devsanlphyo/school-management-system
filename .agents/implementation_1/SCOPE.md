# Scope: LessonPlans Feature Implementation

## Architecture
- **Data Layer**: `LessonPlan` entity with relations to `ApplicationUser` (Teacher). `SystemSetting` contains the `DailyDeadline` time.
- **Storage Layer**: Physical files stored in `uploads/` in the project root.
  - Lesson Plan file: `lessonplan_{id}.{ext}`
  - Justification file: `justification_{id}.{ext}`
- **API Layer**: Minimal API endpoints in `Program.cs` for secure download with authorization checks:
  - `/api/lesson-plans/{id}/download`
  - `/api/lesson-plans/{id}/justification/download`
- **UI Layer**:
  - `Teacher/LessonPlans.razor`: Teacher uploads plans, provides justifications, views feedback, and downloads their own files.
  - `Headmaster/LessonPlans.razor`: Headmaster views submissions, downloads files, and reviews plans.
  - Both use `IDbContextFactory<ApplicationDbContext>` for thread-safe DB access and HelloTwo premium design system classes.

## Milestones
| # | Name | Scope | Dependencies | Status |
|---|------|-------|-------------|--------|
| 1 | File Storage & Download Endpoints | Set up `uploads/` directory, implement secure endpoints `/api/lesson-plans/{id}/download` and `/api/lesson-plans/{id}/justification/download` with proper auth and dynamic extension matching, and add tests. | None | PLANNED |
| 2 | Teacher Portal Component | Implement `Teacher/LessonPlans.razor` using `IDbContextFactory`, Blazor `<InputFile>`, deadline compliance checks against `SystemSettings.DailyDeadline`, justification requirements, and file listing. | M1 | PLANNED |
| 3 | Headmaster Portal Component | Refactor `Headmaster/LessonPlans.razor` to use `IDbContextFactory`, add download links, and integrate with the new download endpoints. | M1, M2 | PLANNED |

## Interface Contracts
### File Download API
- `GET /api/lesson-plans/{id}/download`
  - Auth: Authenticated, role is Teacher (must own the plan) or Admin/Headmaster/Officer (must share SchoolId with the teacher).
  - Returns: File stream with correct MIME type based on the dynamic extension.
  - Errors: 401 Unauthorized, 403 Forbidden, 404 Not Found.
- `GET /api/lesson-plans/{id}/justification/download`
  - Auth: Same as above.
  - Returns: Justification file stream.
  - Errors: 401, 403, 404.

### Code Layout
- `uploads/` - Project root directory for uploaded files (not in wwwroot, excluded from git or kept secure).
- `MainSchoolsManagementSystem/Program.cs` - Minimal API endpoint definitions.
- `MainSchoolsManagementSystem/Components/Pages/Teacher/LessonPlans.razor` - Teacher portal page.
- `MainSchoolsManagementSystem/Components/Pages/Headmaster/LessonPlans.razor` - Headmaster portal page.
- `MainSchoolsManagementSystem.Tests/` - Unit and integration tests.
