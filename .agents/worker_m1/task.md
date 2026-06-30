# Worker Task - Milestone 1: DB Schema Extension
Working Directory: c:\Users\sanlphyo\workspace\csharpworkspace\SchoolsManagementSystem\.agents\worker_m1\

## Objective
Implement the database schema extension for Milestone 1.

## Instructions
1. Edit `MainSchoolsManagementSystem/Data/ApplicationUser.cs` to add the following property:
   ```csharp
   public string? ProfilePicturePath { get; set; }
   ```
2. Edit `MainSchoolsManagementSystem/Data/ApplicationDbContext.cs` to remove or comment out the ignore statement for `PhoneNumber`:
   ```csharp
   // Remove/comment out:
   // builder.Entity<ApplicationUser>().Ignore(u => u.PhoneNumber);
   ```
   *Note: This is required because the profile page needs to allow editing and saving the phone number, but it is currently ignored in EF Core.*

3. Clean and build the project to ensure no compilation errors:
   ```powershell
   dotnet build MainSchoolsManagementSystem/MainSchoolsManagementSystem.csproj
   ```

4. Generate a new EF Core migration named `AddProfilePicturePathAndRestorePhoneNumber` using `dotnet ef`:
   ```powershell
   dotnet ef migrations add AddProfilePicturePathAndRestorePhoneNumber --project MainSchoolsManagementSystem --startup-project MainSchoolsManagementSystem
   ```

5. Apply the migration to the database:
   ```powershell
   dotnet ef database update --project MainSchoolsManagementSystem --startup-project MainSchoolsManagementSystem
   ```

6. Verify that the build still succeeds and the migration files are correctly generated in `MainSchoolsManagementSystem/Data/Migrations/`.

7. Document the commands run, the results, and the files modified in `c:\Users\sanlphyo\workspace\csharpworkspace\SchoolsManagementSystem\.agents\worker_m1\changes.md` and write a handoff to `c:\Users\sanlphyo\workspace\csharpworkspace\SchoolsManagementSystem\.agents\worker_m1\handoff.md`.

## MANDATORY INTEGRITY WARNING
DO NOT CHEAT. All implementations must be genuine. DO NOT hardcode test results, create dummy/facade implementations, or circumvent the intended task. A Forensic Auditor will independently verify your work. Integrity violations WILL be detected and your work WILL be rejected.
