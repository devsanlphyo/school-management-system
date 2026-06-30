# Project: Profile Page Feature

## Architecture
- **User Schema**: `ApplicationUser` extended with `ProfilePicturePath` (string, nullable).
- **UI**: Revamped `Index.razor` under Account management, displaying:
  - Username / Email (read-only)
  - Full Name (editable)
  - Phone Number (editable)
  - School & Department (read-only, shown if assigned)
- **File Upload**: Image upload control supporting `.png`, `.jpg`, `.jpeg` up to 2MB, saving to `wwwroot/uploads/profile-pictures/` with unique filenames.
- **Styling**: Styled avatar display with fallback default avatar, complying with `design-system.md` using CSS custom properties.
- **Testing**: Automated integration/unit tests in `ProfileTests.cs` and E2E verification.

## Milestones
| # | Name | Scope | Dependencies | Status |
|---|------|-------|-------------|--------|
| 1 | E2E Testing Track | Define and implement the E2E test suite (Tiers 1-4) and publish `TEST_READY.md` | None | PLANNED |
| 2 | DB Schema Extension | Add `ProfilePicturePath` to `ApplicationUser`, generate and apply EF migration | None | PLANNED |
| 3 | Profile UI & Editing | Revamp `Index.razor` with form, input validation, and saving to DB | M2 | PLANNED |
| 4 | Image Upload & Avatar | Implement upload control, secure local saving, and avatar display | M3 | PLANNED |
| 5 | E2E Verification & Hardening | Pass 100% of the E2E test suite and perform adversarial hardening (Tier 5) | M1, M4 | PLANNED |

## Interface Contracts
### ApplicationUser Schema
- `ProfilePicturePath`: `string?`

### File Upload
- **Target Path**: `wwwroot/uploads/profile-pictures/`
- **Allowed Formats**: `.png`, `.jpg`, `.jpeg`
- **Max Size**: 2MB
- **Filename**: Unique identifier (e.g., GUID or timestamp-based)

## Code Layout
- **Model**: `MainSchoolsManagementSystem/Data/ApplicationUser.cs`
- **DB Context**: `MainSchoolsManagementSystem/Data/ApplicationDbContext.cs`
- **UI Page**: `MainSchoolsManagementSystem/Components/Account/Pages/Manage/Index.razor`
- **Design System**: `design-system.md`
- **Upload Directory**: `MainSchoolsManagementSystem/wwwroot/uploads/profile-pictures/`
- **Tests**: `MainSchoolsManagementSystem/Data/ProfileTests.cs` (or integration test files)
