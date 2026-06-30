## 2026-06-30T06:11:28Z

You are the E2E Test Developer. Your task is to design and implement a comprehensive E2E test suite for the Teacher Attendance feature.

Your tasks:
1. Create a C# Console Application test project named `MainSchoolsManagementSystem.Tests` in the directory `c:/Users/sanlphyo/workspace/csharpworkspace/SchoolsManagementSystem/MainSchoolsManagementSystem.Tests`. Using a Console Application avoids external NuGet dependency issues (like Playwright or bUnit not being in the local cache under CODE_ONLY network mode). You can reference the main project `MainSchoolsManagementSystem` and write the tests there.
2. Add the test project to the solution `SchoolsManagementSystem.sln` and add a project reference to `MainSchoolsManagementSystem`.
3. Design and implement at least 60 test cases covering the 5 core features of the Teacher Attendance feature across 4 tiers:
   - Feature 1: Teacher Check-In (ability to check in)
   - Feature 2: Lateness Evaluation (calculating Present vs Late against SystemSetting.DailyDeadline or default 08:30:00)
   - Feature 3: Teacher Attendance Dashboard (displaying status, check-in button, and history table for the current month)
   - Feature 4: Headmaster Attendance View (viewing daily attendance list and stats for their school)
   - Feature 5: Headmaster Attendance Override (overriding teacher attendance status)
   Tiers:
   - Tier 1: Feature Coverage (>=25 tests, >=5 per feature)
   - Tier 2: Boundary & Corner Cases (>=25 tests, >=5 per feature)
   - Tier 3: Cross-Feature Combinations (>=5 tests)
   - Tier 4: Real-World Application Scenarios (>=5 tests)
4. Write `c:/Users/sanlphyo/workspace/csharpworkspace/SchoolsManagementSystem/TEST_INFRA.md` using the template in the system prompt.
5. In your test runner, configure it to use a separate test database (e.g. `Server=localhost;Database=SchoolsManagementSystem_Test;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True`) so that running the tests does not affect the main development database. Ensure the test runner can automatically delete and recreate the database for clean runs.
6. For the UI-level testing: since we are in a headless/network-restricted environment and cannot use Playwright, you can test the Blazor components by instantiating them, injecting the required services (e.g. using reflection to set the `[Inject]` properties), calling their lifecycle and event handler methods (like `OnInitializedAsync`, `LoadAttendanceData`, `SetAttendance`, or the teacher's check-in method), and verifying that the database and the component's state are updated correctly.
7. Run the test suite and verify that it compiles and executes. Note that the Teacher Attendance dashboard page is currently a placeholder, so tests targeting its specific UI elements or methods may fail or can be stubbed/mocked. The tests are considered "ready for implementation verification" even if they fail due to the missing implementation.
8. Once the test suite is complete and ready, write a handoff.md in your working directory and notify the orchestrator.

Your working directory is: c:/Users/sanlphyo/workspace/csharpworkspace/SchoolsManagementSystem/.agents/e2e_testing_worker/
Please create this directory if it does not exist, and write your progress.md and handoff.md there.
