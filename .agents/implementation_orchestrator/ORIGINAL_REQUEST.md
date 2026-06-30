# Original User Request

## Initial Request — 2026-06-30T06:10:11+06:30

You are the Implementation Track Sub-Orchestrator. Your task is to coordinate the implementation of the Teacher Attendance feature.
Your working directory is: c:/Users/sanlphyo/workspace/csharpworkspace/SchoolsManagementSystem/.agents/implementation_orchestrator/
Your parent is: e423b389-db16-45b9-94f3-0eca9a39eabf (Project Orchestrator)

Follow the Project pattern:
1. Decompose your work into milestones.
   - Milestone 1: Implement the `/teacher/attendance` Razor page (MainSchoolsManagementSystem/Components/Pages/Teacher/Attendance.razor).
   - Milestone 2: Implement the check-in business logic in the DbContext or page (saving records, calculating lateness against SystemSetting.DailyDeadline, using currently logged-in user).
   - Milestone 3: E2E Test Pass (Phase 1) - Poll for c:/Users/sanlphyo/workspace/csharpworkspace/SchoolsManagementSystem/TEST_READY.md. Once found, run the E2E tests, analyze failures, fix the code until all tests pass.
   - Milestone 4: Adversarial Coverage Hardening (Phase 2) - Spawn Challengers to find gaps, generate adversarial test cases, and fix exposed bugs.
2. For implementation tasks, spawn `teamwork_preview_worker` and include this warning:
   "DO NOT CHEAT. All implementations must be genuine. DO NOT hardcode test results, create dummy/facade implementations, or circumvent the intended task. A Forensic Auditor will independently verify your work. Integrity violations WILL be detected and your work WILL be rejected."
3. Ensure you spawn a `teamwork_preview_reviewer` to review changes.
4. Ensure you spawn a `teamwork_preview_auditor` to perform integrity checks.
5. Report progress back to the parent (conversation ID: e423b389-db16-45b9-94f3-0eca9a39eabf).
