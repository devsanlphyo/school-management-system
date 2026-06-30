# Analysis: DB Schema Extension (Milestone 2)

This report details the analysis and design for extending the database schema of the **HelloTwo Schools Management System** to support the **Profile Page Feature**.

---

## 1. Codebase Analysis

### 1.1 Model Investigation
- **Path**: `MainSchoolsManagementSystem/Data/ApplicationUser.cs`
- **Current Structure**:
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
- **Proposed Addition**:
  We need to add the `ProfilePicturePath` property to this class. It should be a nullable string (`string?`) because:
  1. Not all users will have a profile picture initially.
  2. The column in the database must allow `NULL` values to prevent database constraints from failing on existing user records.

### 1.2 DB Context Investigation
- **Path**: `MainSchoolsManagementSystem/Data/ApplicationDbContext.cs`
- **Configuration**:
  The `ApplicationDbContext` class inherits from `IdentityDbContext<ApplicationUser>`.
  In `OnModelCreating`, several default `IdentityUser` columns are explicitly ignored to keep the schema clean:
  ```csharp
  builder.Entity<ApplicationUser>().Ignore(u => u.PhoneNumber);
  builder.Entity<ApplicationUser>().Ignore(u => u.PhoneNumberConfirmed);
  builder.Entity<ApplicationUser>().Ignore(u => u.TwoFactorEnabled);
  builder.Entity<ApplicationUser>().Ignore(u => u.LockoutEnd);
  builder.Entity<ApplicationUser>().Ignore(u => u.LockoutEnabled);
  builder.Entity<ApplicationUser>().Ignore(u => u.AccessFailedCount);
  builder.Entity<ApplicationUser>().Ignore(u => u.EmailConfirmed);
  ```

---

## 2. Crucial Finding: Phone Number Persistency

In `PROJECT.md` under the **UI** section, the requirements state:
> - **UI**: Revamped `Index.razor` under Account management, displaying:
>   - Username / Email (read-only)
>   - Full Name (editable)
>   - **Phone Number (editable)**
>   - School & Department (read-only, shown if assigned)

However, as observed in `ApplicationDbContext.cs` and the migration file `20260628113447_RemoveUnusedIdentityColumns.cs`, the `PhoneNumber` and `PhoneNumberConfirmed` columns were explicitly dropped from the database and are ignored in the EF Core configuration.

### Impact
If the implementer attempts to bind `ApplicationUser.PhoneNumber` to the UI form and save the user, it will either throw an EF Core exception (if attempting to access a property that is ignored) or fail to persist the value in the database.

### Recommendation
To satisfy the requirement of an editable Phone Number, the implementer must:
1. Remove the ignore statement in `ApplicationDbContext.cs`:
   ```csharp
   // Remove or comment out:
   // builder.Entity<ApplicationUser>().Ignore(u => u.PhoneNumber);
   ```
2. Generate a migration that recreates the `PhoneNumber` column in the `AspNetUsers` table. The column should be nullable (`nvarchar(max)`).

---

## 3. Formulated Code Changes

### 3.1 `ApplicationUser.cs`
Add the `ProfilePicturePath` property:
```csharp
namespace MainSchoolsManagementSystem.Data
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = "";
        public int? SchoolId { get; set; }
        public School? School { get; set; }
        public int? DepartmentId { get; set; }
        public Department? Department { get; set; }
        
        // Extended property for the profile picture
        public string? ProfilePicturePath { get; set; }
    }
}
```

### 3.2 `ApplicationDbContext.cs` (Optional but Recommended for Phone Number)
If phone number persistency is required:
```csharp
// Remove this line from OnModelCreating:
builder.Entity<ApplicationUser>().Ignore(u => u.PhoneNumber);
```

---

## 4. Migration Strategy & Commands

### 4.1 Tools Check
The project file `MainSchoolsManagementSystem/MainSchoolsManagementSystem.csproj` includes:
```xml
<PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="8.0.28" />
```
This indicates that both `dotnet ef` (command-line tool) and Package Manager Console (PMC) commands are supported.

### 4.2 Step-by-Step Commands

1. **Clean and Build the Project**:
   Ensure the project compiles successfully before adding a migration:
   ```powershell
   dotnet build MainSchoolsManagementSystem/MainSchoolsManagementSystem.csproj
   ```

2. **Add the Migration**:
   Run the following command from the repository root:
   - **EF Core CLI**:
     ```powershell
     dotnet ef migrations add AddProfilePicturePathToApplicationUser --project MainSchoolsManagementSystem --startup-project MainSchoolsManagementSystem
     ```
   - **Package Manager Console (PMC)**:
     ```powershell
     Add-Migration AddProfilePicturePathToApplicationUser -Project MainSchoolsManagementSystem -StartUpProject MainSchoolsManagementSystem
     ```

3. **Verify the Generated Migration**:
   The command will generate two files in `MainSchoolsManagementSystem/Data/Migrations/`:
   - `xxxx_AddProfilePicturePathToApplicationUser.cs`
   - `xxxx_AddProfilePicturePathToApplicationUser.Designer.cs`
   
   The `Up` method in the migration file should contain:
   ```csharp
   protected override void Up(MigrationBuilder migrationBuilder)
   {
       migrationBuilder.AddColumn<string>(
           name: "ProfilePicturePath",
           table: "AspNetUsers",
           type: "nvarchar(max)",
           nullable: true);
           
       // If PhoneNumber was un-ignored, it will also include:
       // migrationBuilder.AddColumn<string>(
       //     name: "PhoneNumber",
       //     table: "AspNetUsers",
       //     type: "nvarchar(max)",
       //     nullable: true);
   }
   ```

4. **Apply the Migration**:
   Run the following command to update the database schema:
   - **EF Core CLI**:
     ```powershell
     dotnet ef database update --project MainSchoolsManagementSystem --startup-project MainSchoolsManagementSystem
     ```
   - **PMC**:
     ```powershell
     Update-Database -Project MainSchoolsManagementSystem -StartUpProject MainSchoolsManagementSystem
     ```

---

## 5. Verification Method

To verify that the database schema was successfully updated:
1. Compile the project: `dotnet build`.
2. Check the database schema using a SQL client or by inspecting the `ApplicationDbContextModelSnapshot.cs` file.
   - Verify that `b.Property<string>("ProfilePicturePath")` is present under the `ApplicationUser` entity in the snapshot.
3. If possible, run `DbTestRunner.cs` to ensure no database relationship or startup errors occur.
