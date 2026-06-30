# Project: LessonPlans Feature

## Architecture
- **UI Layer**:
  - Teacher: `/teacher/lessonplans` (`MainSchoolsManagementSystem/Components/Pages/Teacher/LessonPlans.razor`).
  - Headmaster: `/headmaster/lesson-plans` (`MainSchoolsManagementSystem/Components/Pages/Headmaster/LessonPlans.razor`).
- **Layout**:
  - Teacher: `MainSchoolsManagementSystem.Components.Layout.TeacherLayout`
  - Headmaster: `HeadmasterLayout`
- **Data Access**: `ApplicationDbContext` to query and save `LessonPlan` and `SystemSetting` records.
- **File Storage**:
  - Secure folder: `uploads/` in the project root.
  - Naming: `lessonplan_{id}.{ext}` and `justification_{id}.{ext}`.
- **Authentication**: Retrieve the currently logged-in user's ID and role using `AuthenticationStateProvider` and `UserManager`.

## Milestones
| # | Name | Scope | Dependencies | Status |
|---|------|-------|-------------|--------|
| 1 | Exploration & Analysis | Explore DB context, models, authentication, layout, existing uploads, and design system. | None | DONE |
| 2 | E2E Test Suite Design | Create comprehensive E2E tests covering Tiers 1-4 for the LessonPlans feature. | M1 | IN_PROGRESS |
| 3 | Implementation | Implement the Teacher LessonPlans page (dashboard, submission form, file upload, deadline check) and update the Headmaster LessonPlans page. | M1, M2 | IN_PROGRESS |
| 4 | E2E & Adversarial Verification | Run E2E tests, perform adversarial coverage hardening, and run the Forensic Audit. | M2, M3 | PLANNED |

## Interface Contracts
### Teacher Lesson Plans UI
- Retrieve `SystemSetting` for `DailyDeadline` (fallback: `08:30:00`).
- Retrieve `LessonPlan` records for the logged-in teacher.
- Save new `LessonPlan` record with `TeacherId`, `UploadedAt = DateTime.UtcNow`, `IsLate` (calculated), `JustificationText`, `HasJustificationAttachment`, `Status = LessonPlanStatus.Pending`.
- Save files to `uploads/` with naming conventions.

### Headmaster Lesson Plans UI
- Display list of lesson plans for the headmaster's school.
- Display download/view links for lesson plan files and justification files.
- Submit feedback and update status to `Reviewed`.

## Code Layout
- `MainSchoolsManagementSystem/Components/Pages/Teacher/LessonPlans.razor` - Teacher Lesson Plans Dashboard and Submission UI.
- `MainSchoolsManagementSystem/Components/Pages/Headmaster/LessonPlans.razor` - Headmaster Lesson Plans Review UI.
- `MainSchoolsManagementSystem/Data/LessonPlan.cs` - LessonPlan Entity model.
- `MainSchoolsManagementSystem/Data/SystemSetting.cs` - SystemSetting Entity model.
