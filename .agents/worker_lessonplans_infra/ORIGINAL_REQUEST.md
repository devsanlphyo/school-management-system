## 2026-06-30T06:12:46Z
You are a Worker subagent. Your task is to set up the E2E test infrastructure for the LessonPlans feature in the `MainSchoolsManagementSystem.Tests` project.
Please do the following:
1. Update `MainSchoolsManagementSystem.Tests.csproj` to use xUnit and Playwright. Add the following package references:
   - `Microsoft.NET.Test.Sdk`
   - `xunit`
   - `xunit.runner.visualstudio`
   - `Microsoft.Playwright`
   - `Microsoft.AspNetCore.Mvc.Testing`
2. Create `E2ETestBase.cs` in the `MainSchoolsManagementSystem.Tests` project. It should:
   - Start the ASP.NET Core web host on a real local TCP port (e.g., using a custom WebApplicationFactory that starts Kestrel, or by starting the process in a background task). Note that Blazor Server requires a real TCP port for Playwright to connect to.
   - Initialize Playwright (IPlaywright, IBrowser, IBrowserContext, IPage).
   - Implement database seeding/resetting to ensure a clean state before each test.
   - Implement a mechanism to generate Playwright storage states for Teacher, Headmaster, and Admin users to bypass the login screen in subsequent tests.
3. Write a simple sanity test (e.g. `SanityTests.cs` with a test that logs in as a teacher and verifies the page displays).
4. Copy c:\Users\sanlphyo\workspace\csharpworkspace\SchoolsManagementSystem\.agents\e2e_testing_1\TEST_INFRA.md to c:\Users\sanlphyo\workspace\csharpworkspace\SchoolsManagementSystem\TEST_INFRA.md.
5. Run the build and the sanity test to verify everything works.
6. Provide a detailed handoff report in your working directory c:\Users\sanlphyo\workspace\csharpworkspace\SchoolsManagementSystem\.agents\worker_lessonplans_infra including build/test output.

MANDATORY INTEGRITY WARNING:
DO NOT CHEAT. All implementations must be genuine. DO NOT hardcode test results, create dummy/facade implementations, or circumvent the intended task. A Forensic Auditor will independently verify your work. Integrity violations WILL be detected and your work WILL be rejected.

Your working directory is c:\Users\sanlphyo\workspace\csharpworkspace\SchoolsManagementSystem\.agents\worker_lessonplans_infra.
