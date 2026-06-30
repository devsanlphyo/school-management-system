# Handoff Report: DB Schema Extension (Milestone 2)

## 1. Observation

- **ApplicationUser Path**: `MainSchoolsManagementSystem/Data/ApplicationUser.cs`
  - Current class declaration:
    ```csharp
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = "";
        public int? SchoolId { get; set; }
        public School? School { get; set; }
        public int? DepartmentId { get; set; }
        public Department? Department { get; set; }
    }
    ```
- **ApplicationDbContext Path**: `MainSchoolsManagementSystem/Data/ApplicationDbContext.cs`
  - Ignores default `IdentityUser` columns (lines 23-30):
    ```csharp
    builder.Entity<ApplicationUser>().Ignore(u => u.PhoneNumber);
    builder.Entity<ApplicationUser>().Ignore(u => u.PhoneNumberConfirmed);
    // ...
    ```
- **Migration History**: `MainSchoolsManagementSystem/Data/Migrations/`
  - `20260628113447_RemoveUnusedIdentityColumns.cs` dropped `PhoneNumber` from `AspNetUsers` (lines 27-28):
    ```csharp
    migrationBuilder.DropColumn(
        name: "PhoneNumber",
        table: "AspNetUsers");
    ```
- **Project File**: `MainSchoolsManagementSystem/MainSchoolsManagementSystem.csproj`
  - Uses `Microsoft.EntityFrameworkCore.SqlServer` and `Microsoft.EntityFrameworkCore.Tools` (version 8.0.28).
- **Test Runner**: `MainSchoolsManagementSystem/Data/DbTestRunner.cs`
  - Validates database relationships programmatically.

---

## 2. Logic Chain

1. **Adding `ProfilePicturePath`**:
   - To support the profile picture feature, we must extend `ApplicationUser` by adding `public string? ProfilePicturePath { get; set; }` in `ApplicationUser.cs` (Ref: Observation 1).
   - Because it is nullable (`string?`), EF Core will configure the database column as `NULL` allowed. This is necessary to avoid violating constraints on existing rows in `AspNetUsers`.
2. **Restoring `PhoneNumber` (Crucial Finding)**:
   - The UI requirements in `PROJECT.md` specify an editable **Phone Number**.
   - However, `PhoneNumber` is currently ignored in `ApplicationDbContext.cs` and has been dropped from the database (Ref: Observation 2, Observation 3).
   - Therefore, the implementer must remove the ignore statement for `PhoneNumber` in `ApplicationDbContext.cs` and include it in the migration to restore the column.
3. **Migration Management**:
   - Since `Microsoft.EntityFrameworkCore.Tools` is referenced in the project (Ref: Observation 4), the standard `dotnet ef migrations add` and `dotnet ef database update` commands are the correct and supported way to manage the database schema extension.

---

## 3. Caveats

- **Existing Data**: The `ProfilePicturePath` and restored `PhoneNumber` columns will be added as nullable columns, so existing user records will have `NULL` values. The UI and application code must handle `null` gracefully.
- **Server vs Local DB**: The migration commands assume the developer has a local SQL Server instance configured and running, matching the connection string `DefaultConnection` in `appsettings.json`.

---

## 4. Conclusion

To complete the DB Schema Extension milestone, the following actions must be taken:
1. Edit `MainSchoolsManagementSystem/Data/ApplicationUser.cs` to add `public string? ProfilePicturePath { get; set; }`.
2. Edit `MainSchoolsManagementSystem/Data/ApplicationDbContext.cs` to remove `builder.Entity<ApplicationUser>().Ignore(u => u.PhoneNumber);` (if phone number persistency is required).
3. Run the following commands to create and apply the migration:
   ```powershell
   dotnet ef migrations add AddProfilePicturePathToApplicationUser --project MainSchoolsManagementSystem
   dotnet ef database update --project MainSchoolsManagementSystem
   ```

---

## 5. Verification Method

To verify the database schema extension:
1. Run a build to ensure the code compiles without errors:
   ```powershell
   dotnet build
   ```
2. Verify that the new migration files are generated in `MainSchoolsManagementSystem/Data/Migrations/`.
3. Open `MainSchoolsManagementSystem/Data/Migrations/ApplicationDbContextModelSnapshot.cs` and verify that the `ProfilePicturePath` (and optionally `PhoneNumber`) properties are present under the `ApplicationUser` entity.
4. Run the application and check the database schema directly (e.g. via SQL Server Object Explorer) to confirm the columns `ProfilePicturePath` and `PhoneNumber` exist in the `AspNetUsers` table.
