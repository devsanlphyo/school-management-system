## 2026-06-30T06:11:45+06:30
You are the Worker. Your task is to implement Milestone 1 and Milestone 2 for the Teacher Attendance feature.
1. Implement the `/teacher/attendance` Razor page (`MainSchoolsManagementSystem/Components/Pages/Teacher/Attendance.razor`).
2. Implement the check-in business logic:
   - Save attendance records to the database.
   - Calculate lateness against `SystemSetting.DailyDeadline` (fallback to 8:30 AM if not present).
   - Use the currently logged-in teacher's ID.
   - Store `Date` as the local date (midnight) and `CheckedInAt` in UTC. Convert to local time for display and comparison.
   - Follow the design system styling (glass-panel, premium-table, btn-premium, badges).
Refer to the Explorer synthesis at `c:/Users/sanlphyo/workspace/csharpworkspace/SchoolsManagementSystem/.agents/implementation_orchestrator/synthesis.md` and the detailed analysis at `c:/Users/sanlphyo/workspace/csharpworkspace/SchoolsManagementSystem/.agents/explorer_m1_3/analysis.md` for the recommended code.

Verify your work by:
1. Building the project.
2. Running any existing unit tests.
3. Documenting the build and test results in your handoff report at `c:/Users/sanlphyo/workspace/csharpworkspace/SchoolsManagementSystem/.agents/worker_m1_m2/handoff.md`.

MANDATORY INTEGRITY WARNING:
DO NOT CHEAT. All implementations must be genuine. DO NOT hardcode test results, create dummy/facade implementations, or circumvent the intended task. A Forensic Auditor will independently verify your work. Integrity violations WILL be detected and your work WILL be rejected.
