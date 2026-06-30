# Scope: LessonPlans E2E Test Suite

## Architecture
- **Target Project**: `MainSchoolsManagementSystem.Tests` (re-purposed as xUnit + Playwright E2E test project).
- **Web Host**: In-process or background ASP.NET Core web host running on a local port (e.g., `http://localhost:5000` or a dynamic port).
- **Database**: SQL Server (or local DB) configured via `appsettings.json` or environment variables. A clean database state must be ensured by seeding default data and cleaning up after tests.
- **Authentication**: Playwright `StorageState` files (`teacher_state.json`, `headmaster_state.json`, `admin_state.json`) to bypass login screens.

## Milestones
| # | Name | Scope | Dependencies | Status |
|---|------|-------|-------------|--------|
| 1 | Test Infra Setup | Configure `MainSchoolsManagementSystem.Tests` with xUnit, Playwright, and a test server host. Create `E2ETestBase` and write a basic test to verify. Copy `TEST_INFRA.md` to project root. | None | PLANNED |
| 2 | Tier 1 Tests | Implement all Tier 1 Feature Coverage tests (1.1 - 1.5). | M1 | PLANNED |
| 3 | Tier 2 Tests | Implement all Tier 2 Boundary & Corner case tests (2.1 - 2.5). | M2 | PLANNED |
| 4 | Tiers 3 & 4 Tests | Implement all Tier 3 Cross-Feature (3.1 - 3.4) and Tier 4 Real-World Scenario (4.1 - 4.5) tests. | M3 | PLANNED |
| 5 | Verification & Publish | Run the complete suite, fix any issues, and publish `TEST_READY.md` at project root. | M4 | PLANNED |

## Interface Contracts
- **E2ETestBase**: A base class providing:
  - `Task InitializeAsync()`: Starts the web host, initializes Playwright, and seeds/configures the DB.
  - `Task DisposeAsync()`: Stops the web host and cleans up Playwright.
  - Helper methods for logging in, uploading files, and resetting settings.
- **Paths**:
  - Web App: `MainSchoolsManagementSystem/`
  - Test Project: `MainSchoolsManagementSystem.Tests/`
  - Storage States: `MainSchoolsManagementSystem.Tests/bin/Debug/net8.0/` or similar output folder.
