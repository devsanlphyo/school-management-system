# Original User Request

## Initial Request — 2026-06-30T06:10:42+06:30

Create a general Profile Page integrated into the HelloTwo Schools Management System (C# / Blazor Web App) that allows logged-in users (students, teachers, admins, etc.) to view and update their profile details.

Working directory: `c:\Users\sanlphyo\workspace\csharpworkspace\SchoolsManagementSystem`
Integrity mode: development

## Requirements

### R1. Extend ApplicationUser Schema
- Add a `ProfilePicturePath` string property to `ApplicationUser` (in [ApplicationUser.cs](file:///c:/Users/sanlphyo/workspace/csharpworkspace/SchoolsManagementSystem/MainSchoolsManagementSystem/Data/ApplicationUser.cs)).
- Add a new Entity Framework Core migration and update the database to include this field.

### R2. Revamp Profile Page UI & Editing
- Update the existing profile management page [Index.razor](file:///c:/Users/sanlphyo/workspace/csharpworkspace/SchoolsManagementSystem/MainSchoolsManagementSystem/Components/Account/Pages/Manage/Index.razor).
- Display the following user details:
  - **Username / Email**: Read-only.
  - **Full Name**: Editable.
  - **Phone Number**: Editable.
  - **School & Department**: Read-only (display only if assigned).
- Validate input fields (e.g., Full Name should not be empty, Phone Number should match a valid format).
- Save changes successfully to the database when the form is submitted.

### R3. Profile Picture Upload & Display
- Provide an image upload control on the profile page.
- Limit uploads to image files (e.g., `.jpg`, `.jpeg`, `.png`) with a maximum size of 2MB.
- Save uploaded images securely to the local file system under `wwwroot/uploads/profile-pictures/` using unique filenames to prevent collisions.
- Save the path in the user's `ProfilePicturePath` column.
- Display the profile picture as a styled avatar on the page. Show a default avatar if no profile picture has been uploaded.

### R4. Design System Compliance
- Align all UI elements, layout, typography, and colors with the HelloTwo design system specified in [design-system.md](file:///c:/Users/sanlphyo/workspace/csharpworkspace/SchoolsManagementSystem/design-system.md).
- Use CSS custom properties (e.g., `--primary`, `--bg-surface`, `--text-primary`) for styling.
- Ensure the layout is clean, responsive, and fits naturally with the rest of the application.

## Acceptance Criteria

### Database & Schema
- [ ] `ApplicationUser.cs` has `public string? ProfilePicturePath { get; set; }`.
- [ ] A new EF migration is created and successfully applied to the database.

### Profile Form & Functionality
- [ ] Navigating to `/Account/Manage` displays the logged-in user's details.
- [ ] Full Name and Phone Number are editable, and changes are successfully saved to the database.
- [ ] School and Department names are shown as read-only (if the user has a School or Department assigned).
- [ ] Validation errors are displayed if Full Name is empty or if the phone number format is invalid.

### Profile Picture Upload
- [ ] Users can upload a `.png`, `.jpg`, or `.jpeg` file.
- [ ] Uploaded files are saved to `wwwroot/uploads/profile-pictures/` with unique filenames.
- [ ] The file path is correctly saved in the user's `ProfilePicturePath` property.
- [ ] The profile picture is displayed as a styled avatar on `/Account/Manage` and in the navigation/header if applicable.
- [ ] A fallback default avatar is shown if `ProfilePicturePath` is null or empty.

### Styling & Design
- [ ] The Profile page styling matches the colors, typography, borders, and shadows defined in `design-system.md`.
- [ ] The page layout is clean, centered or aligned properly, and responsive.

### Verification
- [ ] A C# verification/test class exists that programmatically tests the profile update and validation logic, and all tests pass.

## Verification Plan

### Automated Tests
- Create or extend a test suite (e.g., a new integration test file `ProfileTests.cs`) that verifies:
  1. A user can be retrieved from the database and has the `FullName` and `ProfilePicturePath` fields.
  2. Updating `FullName`, `PhoneNumber`, and `ProfilePicturePath` via the `UserManager` or a profile service saves the updated values to the database.
  3. Input validation is enforced (e.g. invalid phone number formats or empty names are rejected).
- Run the test suite programmatically to verify correctness.

### Manual Verification
1. Run the Blazor application (`dotnet run` in `MainSchoolsManagementSystem`).
2. Log in as any seeded user (e.g., `teacher@stjude.edu` or admin).
3. Navigate to `/Account/Manage` (Profile page).
4. Verify that:
   - The current Full Name, Phone Number, School, and Department are loaded correctly.
   - Editing the Full Name and Phone Number and clicking "Save" updates the details, showing a success message.
   - Reloading the page retains the updated details.
   - Uploading a new profile image saves the file to `wwwroot/uploads/profile-pictures/`, updates the user record, and immediately displays the new avatar.
   - Selecting a non-image file or an oversized file shows a validation error.
