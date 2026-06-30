# Milestone 1 Analysis: DB Schema Extension

This document provides a comprehensive analysis of the database schema extension required for adding a profile picture feature to the `ApplicationUser` in the Schools Management System.

---

## 1. Objective & Scope

The objective is to extend the existing ASP.NET Core Identity user model (`ApplicationUser`) to support a profile picture path. This requires:
1. Locating the `ApplicationUser` model class and identifying the exact place to add a new `ProfilePicturePath` property.
2. Analyzing the project's Entity Framework Core migration strategy.
3. Formulating the exact code changes and migration commands needed for the implementer to execute.

---

## 2. Codebase Findings

### 2.1 Model Class Location
- **File Path**: `MainSchoolsManagementSystem/Data/ApplicationUser.cs`
- **Current Definition**:
  ```csharp
  using Microsoft.AspNetCore.Identity;

  namespace MainSchoolsManagementSystem.Data
  {
      // Add profile data for application users by adding properties to the ApplicationUser class
      public class ApplicationUser : IdentityUser
      {
          public string FullName { get; set; } = "";
          public int? SchoolId { get; set; }
          public School? School { get; set; }
          public int? DepartmentId { get; set; }
          public Department? Department { get; set; }
      }
  }
  ```

### 2.2 Entity Framework Core Migrations Setup
- **Migration Files Location**: `MainSchoolsManagementSystem/Data/Migrations/`
- **Current Migration Snapshots**:
  - The latest migration is `20260629182208_AddMaintenanceModeToSystemSetting.cs`.
  - The model snapshot is `ApplicationDbContextModelSnapshot.cs`.
- **EF Core Dependencies**:
  - The project `MainSchoolsManagementSystem.csproj` includes the `Microsoft.EntityFrameworkCore.Tools` (version 8.0.28) package, which enables standard EF Core CLI tools (`dotnet ef`).
- **Database Context**:
  - The context `ApplicationDbContext` (in `MainSchoolsManagementSystem/Data/ApplicationDbContext.cs`) inherits from `IdentityDbContext<ApplicationUser>`, meaning any properties added to `ApplicationUser` will automatically be mapped by EF Core to the `AspNetUsers` table unless explicitly ignored.
- **Startup Execution**:
  - In `MainSchoolsManagementSystem/Program.cs`, the application calls `DbSeeder.SeedAsync(app.Services)` on startup.
  - In `MainSchoolsManagementSystem/Data/DbSeeder.cs`, the seeding logic begins with:
    ```csharp
    using var scope = serviceProvider.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    ...
    // Ensure database is migrated
    await context.Database.MigrateAsync();
    ```
  - **Crucial Finding**: Because `MigrateAsync()` is invoked automatically at application startup, the developer does not need to manually run database update commands in production or development environments. Any pending migrations generated via the CLI will be automatically applied when the application starts.

---

## 3. Proposed Code Changes

To implement the database schema extension, the following changes are required:

### 3.1 `ApplicationUser.cs` Modification
Add `public string? ProfilePicturePath { get; set; }` to `ApplicationUser.cs`. Since it is an optional path, it should be nullable (`string?`).

**Proposed Code Diff:**
```diff
diff --git a/MainSchoolsManagementSystem/Data/ApplicationUser.cs b/MainSchoolsManagementSystem/Data/ApplicationUser.cs
--- a/MainSchoolsManagementSystem/Data/ApplicationUser.cs
+++ b/MainSchoolsManagementSystem/Data/ApplicationUser.cs
@@ -10,5 +10,6 @@
         public School? School { get; set; }
         public int? DepartmentId { get; set; }
         public Department? Department { get; set; }
+        public string? ProfilePicturePath { get; set; }
     }
 }
```

---

## 4. Entity Framework Core Migration Commands

The implementer should execute the following commands in the terminal from the solution root directory:

### 4.1 Step 1: Add the EF Core Migration
Generate the migration that adds the `ProfilePicturePath` column to the `AspNetUsers` table.
```powershell
dotnet ef migrations add AddProfilePicturePathToUser --project MainSchoolsManagementSystem --startup-project MainSchoolsManagementSystem
```
*Note: If executing from inside the `MainSchoolsManagementSystem` directory, the `--project` and `--startup-project` flags can be omitted.*

### 4.2 Step 2: Apply the Migration
The migration will be applied automatically upon starting the application due to `context.Database.MigrateAsync()` in `DbSeeder.cs`. However, to apply it manually via CLI without starting the web server, run:
```powershell
dotnet ef database update --project MainSchoolsManagementSystem --startup-project MainSchoolsManagementSystem
```

---

## 5. Verification Strategy

To verify that the database schema has been successfully extended:

1. **Verify Migration Generation**:
   - Check that a new migration file named `*_AddProfilePicturePathToUser.cs` has been created in `MainSchoolsManagementSystem/Data/Migrations/`.
   - The `Up` method in this file should contain:
     ```csharp
     migrationBuilder.AddColumn<string>(
         name: "ProfilePicturePath",
         table: "AspNetUsers",
         type: "nvarchar(max)",
         nullable: true);
     ```
2. **Verify Database Update**:
   - Run the application: `dotnet run --project MainSchoolsManagementSystem`.
   - Inspect the local SQL Server database `SchoolsManagementSystem` using a SQL client or query tool.
   - Run the following query to verify the column exists:
     ```sql
     SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE 
     FROM INFORMATION_SCHEMA.COLUMNS 
     WHERE TABLE_NAME = 'AspNetUsers' AND COLUMN_NAME = 'ProfilePicturePath';
     ```
   - Verify that the query returns one row indicating `ProfilePicturePath` is `nvarchar(max)` and `YES` (nullable).
3. **Verify Build & Tests**:
   - Ensure the solution builds successfully: `dotnet build`.
