# DB Schema Extension Analysis: Profile Picture Path

This report analyzes the codebase of the **HelloTwo Schools Management System** and recommends an implementation strategy for the Milestone 1 DB Schema Extension: adding a profile picture path to the user profile model.

---

## 1. Executive Summary

The objective of this task is to extend the database schema by adding a new nullable string property `ProfilePicturePath` to the `ApplicationUser` entity. This will allow storing the path of a user's uploaded profile picture.

Our findings and recommendations:
1. **Target Class**: Add `public string? ProfilePicturePath { get; set; }` to `ApplicationUser` in `MainSchoolsManagementSystem/Data/ApplicationUser.cs`.
2. **EF Core Tooling**: The project uses the global `dotnet ef` tool (version 8.0.28).
3. **Migration Strategy**: Create a new EF Core migration named `AddProfilePicturePath` using the `dotnet ef migrations add` command.
4. **Database Update**: The database is automatically migrated at application startup via `context.Database.MigrateAsync()` inside `DbSeeder.SeedAsync(app.Services)`. Thus, running the application is sufficient to apply the migration, although manual update via `dotnet ef database update` is also available.

---

## 2. Codebase Investigation

### 2.1 Target File Location
The `ApplicationUser` class is defined in:
`c:\Users\sanlphyo\workspace\csharpworkspace\SchoolsManagementSystem\MainSchoolsManagementSystem\Data\ApplicationUser.cs`

Currently, the class contains the following properties:
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

### 2.2 Property Specification
We will add:
```csharp
        public string? ProfilePicturePath { get; set; }
```
- **Type**: `string?` (nullable string)
- **Database Column**: By default, EF Core will map this to `nvarchar(max)` and make it nullable (`NULL` in SQL Server). This is appropriate for storing file paths of arbitrary length and allowing users to have no profile picture.

---

## 3. Entity Framework Core Migrations

### 3.1 Existing Migrations
Existing migrations are located in `MainSchoolsManagementSystem/Data/Migrations/`. The latest migration is `20260629182208_AddMaintenanceModeToSystemSetting.cs`. The model snapshot `ApplicationDbContextModelSnapshot.cs` is also stored here.

### 3.2 Migration Execution Environment
- The project target framework is `net8.0`.
- The startup project is `MainSchoolsManagementSystem`.
- The database connection string is defined in `MainSchoolsManagementSystem/appsettings.json` under `ConnectionStrings:DefaultConnection`.
- The `dotnet ef` CLI tool is installed and available (version `8.0.28`).

### 3.3 Automatic Migration at Startup
In `MainSchoolsManagementSystem/Program.cs`, the application seeds the database:
```csharp
// Seed database with roles, schools, and default users
await MainSchoolsManagementSystem.Data.DbSeeder.SeedAsync(app.Services);
```
Inside `DbSeeder.SeedAsync` (`MainSchoolsManagementSystem/Data/DbSeeder.cs`, line 21):
```csharp
            // Ensure database is migrated
            await context.Database.MigrateAsync();
```
This ensures that when the application starts up, any pending migrations are automatically applied to the database.

---

## 4. Proposed Code Changes

### 4.1 Modifying `ApplicationUser.cs`

We propose the following change to `MainSchoolsManagementSystem/Data/ApplicationUser.cs`:

```csharp
<<<<
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = "";
        public int? SchoolId { get; set; }
        public School? School { get; set; }
        public int? DepartmentId { get; set; }
        public Department? Department { get; set; }
    }
====
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = "";
        public int? SchoolId { get; set; }
        public School? School { get; set; }
        public int? DepartmentId { get; set; }
        public Department? Department { get; set; }
        public string? ProfilePicturePath { get; set; }
    }
>>>>
```

---

## 5. Migration and Seeding Commands

The implementer should execute the following commands in sequence:

### 5.1 Step 1: Build the Solution
Ensure the project compiles successfully before adding a migration:
```bash
dotnet build
```

### 5.2 Step 2: Generate the EF Core Migration
Run the following command from the root of the workspace or from the `MainSchoolsManagementSystem` directory to generate the migration:
```bash
dotnet ef migrations add AddProfilePicturePath --project MainSchoolsManagementSystem --startup-project MainSchoolsManagementSystem
```
*Note: If executing from the `MainSchoolsManagementSystem` directory, the `--project` and `--startup-project` flags can be omitted.*

This will generate two files in `MainSchoolsManagementSystem/Data/Migrations/`:
1. `<Timestamp>_AddProfilePicturePath.cs`
2. `<Timestamp>_AddProfilePicturePath.Designer.cs`
It will also update `ApplicationDbContextModelSnapshot.cs`.

The generated migration's `Up` method will contain:
```csharp
protected override void Up(MigrationBuilder migrationBuilder)
{
    migrationBuilder.AddColumn<string>(
        name: "ProfilePicturePath",
        table: "AspNetUsers",
        type: "nvarchar(max)",
        nullable: true);
}
```

The `Down` method will contain:
```csharp
protected override void Down(MigrationBuilder migrationBuilder)
{
    migrationBuilder.DropColumn(
        name: "ProfilePicturePath",
        table: "AspNetUsers");
}
```

### 5.3 Step 3: Apply the Migration
Since the application automatically calls `context.Database.MigrateAsync()` at startup, simply running the application will apply the migration:
```bash
dotnet run --project MainSchoolsManagementSystem
```
Alternatively, to apply the migration manually to the database without running the app:
```bash
dotnet ef database update --project MainSchoolsManagementSystem --startup-project MainSchoolsManagementSystem
```

---

## 6. Verification and Testing

### 6.1 Database Schema Verification
After applying the migration, verify the column is added to the database:
1. Open the database using a SQL Server client (e.g., SSMS or Azure Data Studio) or inspect via a database query tool.
2. Run the following query to verify the column:
   ```sql
   SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE 
   FROM INFORMATION_SCHEMA.COLUMNS 
   WHERE TABLE_NAME = 'AspNetUsers' AND COLUMN_NAME = 'ProfilePicturePath';
   ```
3. Verify that the output shows `ProfilePicturePath` is `nvarchar` and `IS_NULLABLE` is `YES`.

### 6.2 Application Integration Verification
Verify that the property can be read and written by creating a temporary test or checking via the database seeder:
- When a user is retrieved or seeded, the `ProfilePicturePath` property will default to `null`.
- The property can be set to a string path (e.g. `"/uploads/profile-pictures/user123.jpg"`) and saved to the database.
