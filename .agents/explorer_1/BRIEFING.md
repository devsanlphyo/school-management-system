# BRIEFING — 2026-06-29T23:41:00Z

## Mission
Investigate the SchoolsManagementSystem codebase to help design the implementation of the LessonPlans feature.

## 🔒 My Identity
- Archetype: teamwork_preview_explorer
- Roles: explorer
- Working directory: c:\Users\sanlphyo\workspace\csharpworkspace\SchoolsManagementSystem\.agents\explorer_1
- Original parent: 9e85a267-351c-44ad-96a9-92382280c5c1
- Milestone: LessonPlans feature design investigation

## 🔒 Key Constraints
- Read-only investigation — do NOT implement
- Code-only network mode (no external web access)

## Current Parent
- Conversation ID: 9e85a267-351c-44ad-96a9-92382280c5c1
- Updated: not yet

## Investigation State
- **Explored paths**:
  - `MainSchoolsManagementSystem/Data/SystemSetting.cs`
  - `MainSchoolsManagementSystem/Data/LessonPlan.cs`
  - `design-system.md`
  - `MainSchoolsManagementSystem/Components/Pages/Admin/Settings.razor`
  - `MainSchoolsManagementSystem/Components/Layout/TeacherLayout.razor`
  - `MainSchoolsManagementSystem/Components/Pages/Teacher/LessonPlans.razor`
  - `MainSchoolsManagementSystem/Data/ApplicationDbContext.cs`
  - `MainSchoolsManagementSystem/Components/Pages/Headmaster/LessonPlans.razor`
- **Key findings**:
  - No file upload helpers exist in the codebase.
  - `SystemSettings` table always contains a single row, and `DailyDeadline` is queried via `FirstOrDefaultAsync()`.
  - To support various file formats without changing the database schema, files should be stored using unique prefix names (e.g., `lessonplan_{id}.*`) and resolved dynamically using prefix scanning, then served via a secure authorized endpoint.
  - Blazor components should use `IDbContextFactory<ApplicationDbContext>` rather than injecting the DbContext directly to avoid concurrency issues.
  - The Teacher portal layout (`TeacherLayout.razor`) shares the same layout CSS classes as the Admin/Headmaster layouts; we should use the premium UI classes like `.glass-panel`, `.premium-table`, and `.btn-premium`.
  - The `wwwroot/uploads` directory does not exist and must be created dynamically if used.
- **Unexplored areas**: None.

## Key Decisions Made
- Completed full analysis of the 6 design questions.
- Recommended storing uploaded files outside of `wwwroot` for security reasons.
- Recommended adhering to the `IDbContextFactory` pattern.

## Artifact Index
- c:\Users\sanlphyo\workspace\csharpworkspace\SchoolsManagementSystem\.agents\explorer_1\analysis.md — Main findings and analysis report
- c:\Users\sanlphyo\workspace\csharpworkspace\SchoolsManagementSystem\.agents\explorer_1\handoff.md — Handoff report following protocol
