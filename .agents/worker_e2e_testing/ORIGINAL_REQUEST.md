## 2026-06-30T06:12:27Z

You are tasked with implementing the E2E test suite and test runner for the Profile Page feature in the HelloTwo Schools Management System.

Your working directory is: c:\Users\sanlphyo\workspace\csharpworkspace\SchoolsManagementSystem\.agents\worker_e2e_testing\

Please follow these steps:

1. **Create the Test Project**:
   - Create a new xUnit test project named `MainSchoolsManagementSystem.Tests` at `c:\Users\sanlphyo\workspace\csharpworkspace\SchoolsManagementSystem\MainSchoolsManagementSystem.Tests\`.
   - Add it to the solution: `dotnet sln c:\Users\sanlphyo\workspace\csharpworkspace\SchoolsManagementSystem\SchoolsManagementSystem.sln add c:\Users\sanlphyo\workspace\csharpworkspace\SchoolsManagementSystem\MainSchoolsManagementSystem.Tests\MainSchoolsManagementSystem.Tests.csproj`.
   - Add a reference to the main project: `dotnet add c:\Users\sanlphyo\workspace\csharpworkspace\SchoolsManagementSystem\MainSchoolsManagementSystem.Tests\MainSchoolsManagementSystem.Tests.csproj reference c:\Users\sanlphyo\workspace\csharpworkspace\SchoolsManagementSystem\MainSchoolsManagementSystem\MainSchoolsManagementSystem.csproj`.
   - Install NuGet packages:
     - `Microsoft.Playwright`
     - `Microsoft.NET.Test.Sdk`
     - `xunit`
     - `xunit.runner.visualstudio`

2. **Implement the Web Application Fixture**:
   - Implement a class fixture (e.g., `WebAppFixture.cs`) that starts the main web application in a background process using `Process.Start`.
   - It should find a free local port dynamically, start the app with `--urls http://127.0.0.1:{port}`, wait for the app to become responsive (by polling or reading stdout), and expose the base URL.
   - It must implement `IDisposable` (or `IAsyncLifetime`) to properly kill the process and any child processes when the tests finish.

3. **Implement the E2E Test Cases**:
   - Write the E2E tests in a class (e.g., `ProfilePageE2eTests.cs`) using `Microsoft.Playwright`.
   - Implement the test cases according to the 4-tier approach:
     - **Tier 1: Feature Coverage** (>=5 tests per feature: username display, editing name/phone, school/dept display, profile picture upload)
     - **Tier 2: Boundary & Corner Cases** (>=5 tests per feature: empty name, invalid phone formats, non-image file upload, >2MB file upload)
     - **Tier 3: Cross-Feature Combinations** (pairwise interactions, e.g., editing name/phone while uploading a profile picture, invalid edit with valid upload, etc.)
     - **Tier 4: Real-World Application Scenarios** (full user journey: login as `teacher@stjude.edu`/`Password123!`, view profile, edit name/phone, upload a valid picture, verify avatar display on page and header, reload and verify persistence)
   - Note: Since the profile page features are not yet fully implemented in the main project, the tests are expected to fail. This is correct. The goal is to have the tests implemented, compilable, and executable.

4. **Install Playwright Browsers**:
   - After building the test project, run the Playwright install command to ensure the browser binaries are installed on the system:
     `dotnet build c:\Users\sanlphyo\workspace\csharpworkspace\SchoolsManagementSystem\MainSchoolsManagementSystem.Tests\MainSchoolsManagementSystem.Tests.csproj`
     Then run:
     `powershell -ExecutionPolicy Bypass -File c:\Users\sanlphyo\workspace\csharpworkspace\SchoolsManagementSystem\MainSchoolsManagementSystem.Tests\bin\Debug\net8.0\playwright.ps1 install` (or the appropriate command for the environment).

5. **Document the Test Design in `TEST_INFRA.md`**:
   - Create `TEST_INFRA.md` at the project root (`c:\Users\sanlphyo\workspace\csharpworkspace\SchoolsManagementSystem\TEST_INFRA.md`).
   - Follow the template in the system instructions:
     - Test Philosophy
     - Feature Inventory (mapping features to requirements and Tiers 1-3)
     - Test Architecture (how the runner starts the app, how Playwright is configured)
     - Real-World Application Scenarios (Tier 4)
     - Coverage Thresholds

6. **Create `TEST_READY.md`**:
   - Create `TEST_READY.md` at the project root (`c:\Users\sanlphyo\workspace\csharpworkspace\SchoolsManagementSystem\TEST_READY.md`).
   - Follow the template in the system instructions:
     - Test Runner Command (e.g. `dotnet test ...`)
     - Coverage Summary (count of tests per tier)
     - Feature Checklist

7. **Verify & Report**:
   - Run the tests using the command line (`dotnet test ...`) and capture the output.
   - Verify that the test runner executes properly, even if the tests fail as expected.
   - Write a detailed handoff report in your working directory and send a message back to me with the results and the paths to the created files.

MANDATORY INTEGRITY WARNING:
DO NOT CHEAT. All implementations must be genuine. DO NOT hardcode test results, create dummy/facade implementations, or circumvent the intended task. A Forensic Auditor will independently verify your work. Integrity violations WILL be detected and your work WILL be rejected.
