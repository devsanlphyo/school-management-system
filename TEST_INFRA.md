# TEST_INFRA.md - Profile Page E2E Test Suite Design

## Test Philosophy
We employ a comprehensive 4-Tier End-to-End (E2E) testing strategy to ensure the stability, correctness, and robustness of the Profile Page feature in the HelloTwo Schools Management System. E2E tests validate the system from the user's perspective, simulating real browser interactions against a running instance of the web application.

- **Tier 1: Feature Coverage**: Verifies that all primary functional requirements (happy path) are met.
- **Tier 2: Boundary & Corner Cases**: Tests input validation, error handling, and extreme values.
- **Tier 3: Cross-Feature Combinations**: Validates pairwise interactions and state consistency under complex operations.
- **Tier 4: Real-World Application Scenarios**: Simulates full user journeys to ensure the application works cohesively across page reloads and authentication boundaries.

---

## Feature Inventory
| Feature ID | Feature Name | Description | Tiers Covered | Test Cases |
|------------|--------------|-------------|---------------|------------|
| **F-01** | Username Display | Display the logged-in user's username in a read-only field. | Tier 1, Tier 4 | `Tier1_UsernameDisplay_ShouldBeDisabled`, `Tier4_RealWorld_TeacherProfileFullJourney` |
| **F-02** | Edit Profile Name | Allow users to edit and save their full name with length constraints. | Tier 1, Tier 2, Tier 3, Tier 4 | `Tier1_EditName_ShouldUpdateSuccessfully`, `Tier2_EmptyName_ShouldShowValidationError`, `Tier2_ExtremelyLongName_ShouldShowValidationError`, `Tier3_ValidNamePhoneAndValidUpload_ShouldSucceed`, `Tier3_InvalidNameAndValidUpload_ShouldFailAndNotSave`, `Tier4_RealWorld_TeacherProfileFullJourney` |
| **F-03** | Edit Phone Number | Allow users to edit and save their phone number with format validation. | Tier 1, Tier 2, Tier 3, Tier 4 | `Tier1_EditPhone_ShouldUpdateSuccessfully`, `Tier2_InvalidPhoneFormat_ShouldShowValidationError`, `Tier3_ValidNamePhoneAndValidUpload_ShouldSucceed`, `Tier3_MultipleInvalidFields_ShouldShowAllValidationErrors`, `Tier4_RealWorld_TeacherProfileFullJourney` |
| **F-04** | School/Dept Display | Display the user's school and department from database tenancy. | Tier 1, Tier 4 | `Tier1_SchoolAndDepartment_ShouldBeDisplayedCorrectly`, `Tier4_RealWorld_TeacherProfileFullJourney` |
| **F-05** | Profile Picture Upload | Support uploading profile picture (<= 2MB, image files only). | Tier 1, Tier 2, Tier 3, Tier 4 | `Tier1_ProfilePictureUpload_ShouldUploadSuccessfully`, `Tier2_UploadNonImageFile_ShouldShowValidationError`, `Tier2_UploadImageTooLarge_ShouldShowValidationError`, `Tier3_ValidNamePhoneAndValidUpload_ShouldSucceed`, `Tier3_ValidNameAndInvalidUpload_ShouldFailAndNotSave`, `Tier3_MultipleInvalidFields_ShouldShowAllValidationErrors`, `Tier4_RealWorld_TeacherProfileFullJourney` |

---

## Test Architecture

### Application Startup and Lifetime (`WebAppFixture.cs`)
The E2E tests execute against a live, locally running instance of the ASP.NET Core web application. The lifecycle of this application is managed via an xUnit Class Fixture:
1. **Dynamic Port Allocation**: To prevent port conflicts in concurrent/CI environments, the fixture binds a temporary socket to port `0` to retrieve a free local port.
2. **Process Lifecycle**: The fixture starts the compiled main application DLL (`MainSchoolsManagementSystem.dll`) in a background process using `Process.Start`:
   - Command: `dotnet MainSchoolsManagementSystem.dll --urls http://127.0.0.1:{port}`
   - Working Directory: `MainSchoolsManagementSystem/`
   - Environment: `ASPNETCORE_ENVIRONMENT=Development`
3. **Startup Synchronization**: The fixture polls the application's base URL using `HttpClient` with a timeout of 20 seconds. It only starts executing tests once the application responds.
4. **Teardown**: The fixture implements `IAsyncLifetime`. On disposal, it terminates the process and its entire process tree (`process.Kill(entireProcessTree: true)`) to prevent orphaned processes.

### Playwright Configuration
- **Browser**: Chromium (headless mode).
- **Security**: Launched with `--no-sandbox` and `--disable-setuid-sandbox` flags to ensure compatibility with containerized and restricted environments.
- **Isolation**: Each test runs in a fresh `IBrowserContext` and `IPage` to guarantee state isolation.

---

## Real-World Application Scenarios (Tier 4)
We define a comprehensive user journey to validate the end-to-end integration:
- **Scenario**: `Tier4_RealWorld_TeacherProfileFullJourney`
  1. **Login**: Authenticate as `teacher@stjude.edu` using password `Password123!`.
  2. **Navigation**: Go to the Profile Page (`/Account/Manage`).
  3. **Verification**: Confirm initial username (`teacher@stjude.edu`), name (`Alice Johnson`), and school (`St. Jude Academy`) are loaded.
  4. **Edit**: Modify name to `Alice Smith`, phone to `555-019-2834`, and upload a valid image.
  5. **Save**: Click Save and verify the success notification.
  6. **UI Reflection**: Verify the updated avatar image is rendered in both the main profile card and the global navigation header.
  7. **Persistence**: Reload the page and verify that the updated name, phone, and avatar persist.

---

## Coverage Thresholds
- **Tier 1 (Feature Coverage)**: 5 tests (100% coverage of core happy-path features)
- **Tier 2 (Boundary & Corner Cases)**: 5 tests (covers empty inputs, invalid formats, file size limits, file type limits, and length constraints)
- **Tier 3 (Cross-Feature Combinations)**: 5 tests (covers pairwise successes, failures, and cancellations)
- **Tier 4 (Real-World Journeys)**: 1 test (covering the full user flow)
- **Total Tests**: 16 tests
