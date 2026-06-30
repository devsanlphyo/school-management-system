# Project: LessonPlans Feature Implementation

## Architecture
- **Framework**: Blazor (C# ASP.NET Core)
- **Database**: Entity Framework Core with existing models `LessonPlan` and `SystemSetting`.
- **Pages**:
  - `Components/Pages/Teacher/LessonPlans.razor`: Teacher dashboard and form.
  - `Components/Pages/Headmaster/LessonPlans.razor`: Headmaster review list and modal.
- **File Storage**:
  - Lesson plans: `wwwroot/uploads/lessonplan_{id}.{ext}`
  - Justifications: `wwwroot/uploads/justification_{id}.{ext}`

## Milestones
| # | Name | Scope | Dependencies | Status |
|---|------|-------|-------------|--------|
| 1 | M1: Exploration | Investigate codebase structure, DB context, authentication, existing styles, file upload patterns | None | PLANNED |
| 2 | M2: Teacher Portal | Implement Teacher/LessonPlans.razor page, submission form, deadline check, file upload | M1 | PLANNED |
| 3 | M3: Headmaster Portal | Update Headmaster/LessonPlans.razor with download/view links for lesson plans and justifications | M1 | PLANNED |
| 4 | M4: Verification | Build, test, run challengers, run forensic auditor | M2, M3 | PLANNED |

## Interface Contracts
- **Uploaded Files**:
  - Path: `wwwroot/uploads/`
  - Naming: `lessonplan_{id}.{ext}`, `justification_{id}.{ext}`
- **Models & DbContext**:
  - `LessonPlan` fields: `Id`, `TeacherId`, `Teacher`, `UploadedAt`, `IsLate`, `JustificationText`, `HasJustificationAttachment`, `Status` (Pending/Reviewed), `Feedback`.
  - `SystemSetting` fields: `Id`, `DailyDeadline` (TimeSpan), `MaintenanceMode` (bool).
  - DbContext: `ApplicationDbContext`.
