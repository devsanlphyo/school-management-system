# BRIEFING — 2026-06-30T06:12:46+06:30

## Mission
Set up the E2E test infrastructure for the LessonPlans feature in the `MainSchoolsManagementSystem.Tests` project using xUnit and Playwright.

## 🔒 My Identity
- Archetype: Worker
- Roles: implementer, qa, specialist
- Working directory: c:\Users\sanlphyo\workspace\csharpworkspace\SchoolsManagementSystem\.agents\worker_lessonplans_infra
- Original parent: 1eca2c90-86c4-4d25-8333-50e253e42360
- Milestone: E2E Test Infrastructure Setup

## 🔒 Key Constraints
- CODE_ONLY network mode: No external websites/services, no curl/wget/lynx.
- Do not cheat: Genuine implementation, no hardcoding, no dummy/facade implementations.
- Write only to our own folder under `.agents/` (except for project files requested).
- Update `progress.md` after each meaningful step.
- Handoff report in `handoff.md` with the 5 required components.

## Current Parent
- Conversation ID: 1eca2c90-86c4-4d25-8333-50e253e42360
- Updated: not yet

## Task Summary
- **What to build**:
  - Update `MainSchoolsManagementSystem.Tests.csproj` with references (Microsoft.NET.Test.Sdk, xunit, xunit.runner.visualstudio, Microsoft.Playwright, Microsoft.AspNetCore.Mvc.Testing).
  - Create `E2ETestBase.cs` in `MainSchoolsManagementSystem.Tests` which:
    - Starts the ASP.NET Core host on a real local TCP port.
    - Initializes Playwright (IPlaywright, IBrowser, IBrowserContext, IPage).
    - Implements database seeding/resetting before each test.
    - Implements Playwright storage state generation for Teacher, Headmaster, Admin.
  - Create `SanityTests.cs` (log in as teacher, verify page displays).
  - Copy `TEST_INFRA.md` from `.agents/e2e_testing_1/TEST_INFRA.md` to `TEST_INFRA.md` in root.
  - Run build and sanity test.
- **Success criteria**:
  - Clean compilation of tests.
  - Playwright E2E test successfully runs and passes.
  - Proper storage state caching / generation.
  - Seeding/resetting works.
- **Interface contracts**: [TBD]
- **Code layout**: [TBD]

## Key Decisions Made
- Use custom WebApplicationFactory that starts Kestrel on a random free port.
- Use Playwright's C# API.
- Cache storage states in a temporary folder during test run or in a dedicated directory.

## Artifact Index
- `c:\Users\sanlphyo\workspace\csharpworkspace\SchoolsManagementSystem\.agents\worker_lessonplans_infra\handoff.md` — Handoff report.
- `c:\Users\sanlphyo\workspace\csharpworkspace\SchoolsManagementSystem\.agents\worker_lessonplans_infra\progress.md` — Progress heartbeat.

## Change Tracker
- **Files modified**: None yet
- **Build status**: [TBD]
- **Pending issues**: None yet

## Quality Status
- **Build/test result**: [TBD]
- **Lint status**: [TBD]
- **Tests added/modified**: None yet
