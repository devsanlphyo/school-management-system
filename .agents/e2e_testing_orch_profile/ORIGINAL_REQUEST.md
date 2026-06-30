# Original User Request

## Initial Request — 2026-06-30T06:11:27+06:30

You are the E2E Testing Orchestrator for the Profile Page feature in the HelloTwo Schools Management System.
Your working directory is: c:\Users\sanlphyo\workspace\csharpworkspace\SchoolsManagementSystem\.agents\e2e_testing_orch_profile\

Your mission:
Design a comprehensive, requirement-driven, opaque-box E2E test suite for the Profile Page feature, implement the test cases/runner, and publish `TEST_READY.md` at the project root.

Instructions:
1. Read the requirements in c:\Users\sanlphyo\workspace\csharpworkspace\SchoolsManagementSystem\ORIGINAL_REQUEST.md and the project plan in c:\Users\sanlphyo\workspace\csharpworkspace\SchoolsManagementSystem\PROJECT.md.
2. Design test cases using the 4-tier approach specified in the Project Pattern:
   - Tier 1: Feature Coverage (>=5 tests per feature: username display, editing name/phone, school/dept display, profile picture upload)
   - Tier 2: Boundary & Corner Cases (>=5 tests per feature: empty name, invalid phone formats, non-image file upload, >2MB file upload)
   - Tier 3: Cross-Feature Combinations (pairwise interactions, e.g., editing while uploading)
   - Tier 4: Real-World Application Scenarios (full user journey: login, view, edit name/phone, upload picture, verify avatar display on page and header, reload and verify persistence)
3. Since there is no dedicated test project in the solution, you can create a test project (e.g. xUnit) or write a custom test runner (like extending DbTestRunner or adding a console app/test harness) to run these tests programmatically.
4. Document your test design in `TEST_INFRA.md` at the project root.
5. Implement the tests and the test runner.
6. Once the tests are implemented and can be executed, publish `TEST_READY.md` at the project root with the runner command and coverage summary.
7. Maintain your own plan, progress, and context in your working directory.
8. When finished, write a handoff report and send a message to your parent (conv ID: 4e6b7263-9bf9-43cd-b66a-64eeed07c5ac) with your results. Do NOT write code directly yourself; spawn workers or explorers if needed.
