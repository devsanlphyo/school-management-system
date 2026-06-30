# Scope: Teacher Attendance E2E Testing

## Architecture
- Opaque-box testing of Teacher Attendance feature.
- Test runner: custom console app, updated DbTestRunner, or an xUnit integration project.
- Targets:
  - `/teacher/attendance` (Teacher Dashboard & Check-In)
  - `/headmaster/attendance` (Headmaster View & Override)

## Milestones
| # | Name | Scope | Dependencies | Status |
|---|------|-------|-------------|--------|
| 1 | Test Design | Design 60+ test cases across 4 tiers; write `TEST_INFRA.md`. | None | PLANNED |
| 2 | Test Implementation | Implement the test runner and test cases in the codebase. | M1 | PLANNED |
| 3 | Verification & Ready | Verify test execution, publish `TEST_READY.md`, and report. | M2 | PLANNED |

## Interface Contracts
- Tests must use the same database/seeding context and simulate teacher and headmaster actions.
- Features to test:
  1. Teacher Check-In (button click, status update)
  2. Lateness Evaluation (compared to `SystemSetting.DailyDeadline`)
  3. Teacher Attendance Dashboard (current status, history grid)
  4. Headmaster Attendance View (summary stats, teacher rows)
  5. Headmaster Attendance Override (changing status via buttons)
