# Handoff Report: DB Schema Extension

## 1. Observation
* **Target File**: `MainSchoolsManagementSystem/Data/ApplicationUser.cs` (lines 6-13):
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
* **EF Core CLI Tool**: Executed `dotnet ef --version` in the terminal, which returned:
  ```
  Entity Framework Core .NET Command-line Tools
  8.0.28
  ```
* **Automatic Migrations at Startup**:
  * In `MainSchoolsManagementSystem/Program.cs` (line 65):
    ```csharp
    await MainSchoolsManagementSystem.Data.DbSeeder.SeedAsync(app.Services);
    ```
  * In `MainSchoolsManagementSystem/Data/DbSeeder.cs` (line 21):
    ```csharp
    await context.Database.MigrateAsync();
    ```
* **Tests**: Executed `dotnet test` which confirmed that the test project `MainSchoolsManagementSystem.Tests` is a placeholder console application and does not contain any tests:
  ```
  No test matches the given test-pattern.
  ```

## 2. Logic Chain
1. **Goal**: Add `ProfilePicturePath` to the database schema for users.
2. **Model Extension**: Adding `public string? ProfilePicturePath { get; set; }` to `ApplicationUser.cs` (observed in Section 1) will extend the ASP.NET Core Identity user model.
3. **Database Columns**: Because it is a nullable string (`string?`), EF Core will map it to a nullable `nvarchar(max)` column in the `AspNetUsers` table.
4. **Migration Generation**: The `dotnet ef` CLI tool is available (observed in Section 1) and can be used to generate a migration called `AddProfilePicturePath`.
5. **Database Update**: Since `DbSeeder.SeedAsync` calls `context.Database.MigrateAsync()` at startup (observed in Section 1), running the application will automatically apply the migration to the database.

## 3. Caveats
* There are no automated tests in the project. Verification must be done by running the build, executing the migration command, and inspecting the SQL database schema.
* The `ProfilePicturePath` is nullable, so the application UI must handle `null` values (defaulting to a placeholder profile image).

## 4. Conclusion
The DB Schema Extension is ready for implementation. The implementer should add the `ProfilePicturePath` property to `ApplicationUser.cs`, run the migration generation command, and start the application to apply the changes to the database.

## 5. Verification Method
1. Run `dotnet build` to verify the code compiles without errors.
2. Generate the migration using:
   ```bash
   dotnet ef migrations add AddProfilePicturePath --project MainSchoolsManagementSystem --startup-project MainSchoolsManagementSystem
   ```
3. Apply the migration using:
   ```bash
   dotnet ef database update --project MainSchoolsManagementSystem --startup-project MainSchoolsManagementSystem
   ```
   *(Or simply run the application using `dotnet run`)*
4. Run the following SQL query on the database to verify the schema change:
   ```sql
   SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE 
   FROM INFORMATION_SCHEMA.COLUMNS 
   WHERE TABLE_NAME = 'AspNetUsers' AND COLUMN_NAME = 'ProfilePicturePath';
   ```
   Verify that `ProfilePicturePath` exists, has type `nvarchar`, and `IS_NULLABLE` is `YES`.
