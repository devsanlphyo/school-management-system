# TEST_READY.md - Profile Page E2E Test Execution Ready Reference

## Test Runner Command
To execute the E2E tests, run the following command from the repository root:

```bash
dotnet test c:\Users\sanlphyo\workspace\csharpworkspace\SchoolsManagementSystem\MainSchoolsManagementSystem.Tests\MainSchoolsManagementSystem.Tests.csproj --logger "console;verbosity=normal"
```

*Note: Before running the tests, ensure that the Playwright browsers are installed (see instructions below).*

---

## Coverage Summary
| Tier | Description | Count | Target Features |
|------|-------------|-------|-----------------|
| **Tier 1** | Feature Coverage | 5 | Username display, Name edit, Phone edit, School display, Picture upload |
| **Tier 2** | Boundary & Corner Cases | 5 | Empty name, Invalid phone format, Non-image upload, >2MB upload, >100 char name |
| **Tier 3** | Cross-Feature Combinations | 5 | Pairwise edits (valid name/phone/upload, invalid name + valid upload, valid name + invalid upload, multiple invalid fields, upload & cancel/reload) |
| **Tier 4** | Real-World Application Scenarios | 1 | Full teacher login, view, edit name/phone/avatar, save, verify header/page avatar, reload & verify persistence |
| **Total** | | **16** | |

---

## Feature Checklist
- [x] **Test Project Creation**: xUnit test project created at `MainSchoolsManagementSystem.Tests` and added to solution.
- [x] **Project References & Packages**: Added reference to `MainSchoolsManagementSystem` and installed `Microsoft.Playwright`, `xunit`, `xunit.runner.visualstudio`, and `Microsoft.NET.Test.Sdk`.
- [x] **Web Application Fixture**: Dynamic port allocation, background process management, and active startup synchronization implemented in `WebAppFixture.cs`.
- [x] **Playwright E2E Tests**: 16 comprehensive test cases covering Tiers 1-4 implemented in `ProfilePageE2eTests.cs`.
- [x] **Playwright Browser Installation**: Executed Playwright browser installation script.

---

## Troubleshooting & Diagnostics
1. **Playwright Browser Installation**:
   If Playwright browsers are missing, run the following command:
   ```powershell
   powershell -ExecutionPolicy Bypass -File c:\Users\sanlphyo\workspace\csharpworkspace\SchoolsManagementSystem\MainSchoolsManagementSystem.Tests\bin\Debug\net8.0\playwright.ps1 install
   ```
2. **Ports & Firewall**:
   The tests dynamically locate an open port. Ensure your local firewall allows the test runner to bind to localhost loopback ports.
3. **Seeded Data**:
   The tests assume the database is seeded with `teacher@stjude.edu` / `Password123!`. This is automatically handled by the application's `DbSeeder` on startup.
