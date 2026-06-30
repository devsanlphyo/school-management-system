# Scope: Teacher Attendance Implementation

## Architecture
- Presentation Layer: Blazor Server Razor Page `/teacher/attendance` located at `MainSchoolsManagementSystem/Components/Pages/Teacher/Attendance.razor`.
- Data/Business Layer: DbContext or Service layer handling attendance logging, calculating lateness against `SystemSetting.DailyDeadline`, and mapping to the currently logged-in user.

## Milestones
| # | Name | Scope | Dependencies | Status |
|---|------|-------|-------------|--------|
| 1 | Milestone 1 | Implement `/teacher/attendance` Razor page | None | PLANNED |
| 2 | Milestone 2 | Implement check-in business logic (saving records, calculating lateness, using current user) | Milestone 1 | PLANNED |
| 3 | Milestone 3 | E2E Test Pass (Phase 1) - Poll for TEST_READY.md, run E2E tests, fix code until they pass | Milestone 2 | PLANNED |
| 4 | Milestone 4 | Adversarial Coverage Hardening (Phase 2) - Challenger loop | Milestone 3 | PLANNED |

## Interface Contracts
### `/teacher/attendance` Razor page
- Route: `/teacher/attendance`
- View: Shows check-in button, current status, and history of attendance.
- Behavior: Button triggers check-in, writes to database, checks against daily deadline.
