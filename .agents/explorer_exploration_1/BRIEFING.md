# BRIEFING — 2026-06-29T23:39:30Z

## Mission
Explore the SchoolsManagementSystem codebase to gather facts for implementing the Teacher Attendance feature.

## 🔒 My Identity
- Archetype: Explorer
- Roles: Read-only investigator
- Working directory: c:/Users/sanlphyo/workspace/csharpworkspace/SchoolsManagementSystem/.agents/explorer_exploration_1
- Original parent: e423b389-db16-45b9-94f3-0eca9a39eabf
- Milestone: Teacher Attendance Feature Investigation

## 🔒 Key Constraints
- Read-only investigation — do NOT implement
- Do not make any changes to any source code files

## Current Parent
- Conversation ID: e423b389-db16-45b9-94f3-0eca9a39eabf
- Updated: 2026-06-29T23:39:30Z

## Investigation State
- **Explored paths**:
  - `MainSchoolsManagementSystem/Data/ApplicationDbContext.cs`
  - `MainSchoolsManagementSystem/Data/Attendance.cs`
  - `MainSchoolsManagementSystem/Data/SystemSetting.cs`
  - `MainSchoolsManagementSystem/Components/Layout/TeacherLayout.razor`
  - `MainSchoolsManagementSystem/Components/Pages/Teacher/Attendance.razor`
  - `MainSchoolsManagementSystem/Components/Pages/Headmaster/Attendance.razor`
  - `MainSchoolsManagementSystem/Components/Pages/Headmaster/Settings.razor`
  - `MainSchoolsManagementSystem/Data/DbTestRunner.cs`
  - `design-system.md`
- **Key findings**:
  - `Attendance` contains `CheckedInAt` (UTC) and `Status` (`Present`, `Late`, `Absent`).
  - `SystemSetting` contains `DailyDeadline` (`TimeSpan`).
  - To compare check-in time against the deadline: `checkedInAtUtc.ToLocalTime().TimeOfDay > DailyDeadline`.
  - The route `/teacher/attendance` is already linked in `TeacherLayout.razor`.
  - `design-system.md` dictates using `IDbContextFactory` and scoped `IServiceProvider` inside Blazor components.
  - The only test file is `DbTestRunner.cs` (not run automatically).
- **Unexplored areas**: None.

## Key Decisions Made
- Confirmed that timezone handling should rely on `.ToLocalTime()` as done in the rest of the application.
- Confirmed that `IDbContextFactory<ApplicationDbContext>` must be used instead of injecting `ApplicationDbContext` directly, as per `design-system.md` Section 10.1.

## Artifact Index
- c:/Users/sanlphyo/workspace/csharpworkspace/SchoolsManagementSystem/.agents/explorer_exploration_1/analysis.md — Detailed findings on codebase, models, auth, timezone handling, navigation, UI design patterns, and tests.
- c:/Users/sanlphyo/workspace/csharpworkspace/SchoolsManagementSystem/.agents/explorer_exploration_1/handoff.md — Handoff report for the next agent.
