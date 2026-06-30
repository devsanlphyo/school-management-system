# Schools Management System — Setup Guide

This guide walks you through setting up and running the **Schools Management System** from scratch on a new machine.

---

## Prerequisites

Before you begin, make sure you have the following installed:

| Requirement | Version | Download |
|---|---|---|
| **.NET SDK** | 8.0 (LTS) | [https://dotnet.microsoft.com/download/dotnet/8.0](https://dotnet.microsoft.com/download/dotnet/8.0) |
| **SQL Server** | 2019+ or LocalDB | [https://www.microsoft.com/en-us/sql-server/sql-server-downloads](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) |
| **Git** | Latest | [https://git-scm.com/downloads](https://git-scm.com/downloads) |

> **Tip:** If you installed Visual Studio with the "ASP.NET and web development" workload, you likely already have SQL Server LocalDB and the .NET 8 SDK.

Verify your .NET SDK installation:
```powershell
dotnet --version
# Expected output: 8.0.x
```

---

## 1. Clone the Repository

```powershell
git clone <repository-url>
cd SchoolsManagementSystem
```

---

## 2. Restore Local Tools

The project includes a local tool manifest (`.config/dotnet-tools.json`) that registers the **Entity Framework Core CLI** tool. Restore it with:

```powershell
dotnet tool restore
```

This installs `dotnet-ef` (v8.0.28) locally so you can run migration commands without a global install.

---

## 3. Restore NuGet Packages

```powershell
dotnet restore
```

This restores all NuGet dependencies for both the main project and the test project.

---

## 4. Database Setup

### Database Provider Toggle

The application supports both **SQLite** (default, portable, zero-install) and **SQL Server**. You can configure this in [`MainSchoolsManagementSystem/appsettings.json`](MainSchoolsManagementSystem/appsettings.json) using the `"DatabaseProvider"` setting:

```json
{
  "DatabaseProvider": "Sqlite",
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=SchoolsManagementSystem;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True",
    "SqliteConnection": "Data Source=SchoolsManagement.db"
  }
}
```

- **`Sqlite` (Recommended for portability)**:
  - No database server installation required.
  - The database is stored in a local file named `SchoolsManagement.db` in the project directory.
  - When the app starts, it automatically creates the database structure using `EnsureCreatedAsync()` and seeds it.
- **`SqlServer`**:
  - Requires a local SQL Server instance (LocalDB, SQL Express, or Developer edition) running on `localhost`.
  - Uses Windows Authentication by default. Update `DefaultConnection` if your instance name or credentials differ.
  - When the app starts, it automatically applies migrations using `MigrateAsync()` and seeds the database.

---

### Seeding & Initial Data

Regardless of the provider chosen, when the application starts, the `DbSeeder` automatically:
1. **Creates the schema** if it doesn't exist.
2. **Seeds roles** (`Admin`, `Director`, `Headmaster`, `Officer`, `Teacher`, `Assistant`).
3. **Seeds schools** ("St. Jude Academy" and "Oakridge High").
4. **Seeds system settings** (daily deadline at 8:30 AM).
5. **Seeds users** (with password `Password123!`).
6. **Seeds mock data** (attendance, leave requests, lesson plans).

Simply run the application and everything is set up for you.

### Manual SQL Server Migration

If using SQL Server and you want to apply migrations manually before running the app:

```powershell
dotnet ef database update --project MainSchoolsManagementSystem
```

### Manual SQL Server Schema Script

A pre-generated SQL Server schema script is included at [`database_structure.sql`](database_structure.sql). You can execute it directly in SQL Server Management Studio (SSMS) or via `sqlcmd`:

```powershell
sqlcmd -S localhost -d master -Q "CREATE DATABASE SchoolsManagementSystem"
sqlcmd -S localhost -d SchoolsManagementSystem -i database_structure.sql
```


> **Note:** Options B and C only create the schema. The seed data (roles, users, schools, etc.) will still be populated automatically when you first run the application.

---

## 5. Run the Application

```powershell
cd MainSchoolsManagementSystem
dotnet run
```

The app will start and display a URL (typically `https://localhost:5001` or `http://localhost:5000`). Open it in your browser.

> On first run, the database will be created, migrated, and seeded automatically. This may take a few extra seconds.

---

## 6. Default Accounts

All seeded users share the same password: **`Password123!`**

| Email | Role | School |
|---|---|---|
| `admin@system.com` | Admin | — (Global) |
| `director@system.com` | Director | — (Global) |
| `headmaster@stjude.edu` | Headmaster | St. Jude Academy |
| `officer@stjude.edu` | Officer | St. Jude Academy |
| `teacher@stjude.edu` | Teacher | St. Jude Academy |
| `science.teacher@stjude.edu` | Teacher | St. Jude Academy |
| `math.teacher@stjude.edu` | Teacher | St. Jude Academy |
| `teacher@oakridge.edu` | Teacher | Oakridge High |
| `history.teacher@oakridge.edu` | Teacher | Oakridge High |

---

## 7. Project Structure

```
SchoolsManagementSystem/
├── .config/
│   └── dotnet-tools.json          # Local tool manifest (dotnet-ef)
├── MainSchoolsManagementSystem/   # Main Blazor Server web application
│   ├── Components/                # Razor components and pages
│   ├── Data/                      # EF Core DbContext, models, migrations, seeders
│   │   ├── ApplicationDbContext.cs
│   │   ├── DbSeeder.cs            # Automatic database seeding on startup
│   │   ├── DbInitializer.cs       # Alternative seeder
│   │   ├── Migrations/            # EF Core migration files
│   │   └── *.cs                   # Entity models (School, Attendance, etc.)
│   ├── Program.cs                 # Application entry point and configuration
│   ├── appsettings.json           # Connection string and app configuration
│   ├── uploads/                   # Runtime file uploads (lesson plans, feed media)
│   └── wwwroot/                   # Static files (CSS, JS, profile pictures)
├── MainSchoolsManagementSystem.Tests/  # xUnit + Playwright E2E test project
├── database_structure.sql         # Pre-generated full database schema script
├── SchoolsManagementSystem.sln    # Visual Studio solution file
└── SETUP_GUIDE.md                 # This file
```

---

## 8. Running Tests

### Build the Test Project

```powershell
dotnet build MainSchoolsManagementSystem.Tests
```

### Install Playwright Browsers (First Time Only)

The E2E tests use Playwright. Before running them for the first time, install the required browsers:

```powershell
pwsh MainSchoolsManagementSystem.Tests/bin/Debug/net8.0/playwright.ps1 install
```

> If `pwsh` is not available, use `powershell -ExecutionPolicy Bypass -File` instead of `pwsh`.

### Run the Tests

```powershell
dotnet test MainSchoolsManagementSystem.Tests --logger "console;verbosity=normal"
```

The tests use a **separate test database** (`SchoolsManagementSystem_Test`) configured in [`TestDbHelper.cs`](MainSchoolsManagementSystem.Tests/TestDbHelper.cs). This database is automatically created and destroyed by the test runner, so your main development database is not affected.

---

## 9. Common EF Core Commands

These commands use the locally installed `dotnet-ef` tool (restored via `dotnet tool restore`):

| Task | Command |
|---|---|
| Apply all migrations | `dotnet ef database update --project MainSchoolsManagementSystem` |
| Add a new migration | `dotnet ef migrations add <MigrationName> --project MainSchoolsManagementSystem` |
| Generate SQL script | `dotnet ef migrations script --project MainSchoolsManagementSystem --output database_structure.sql` |
| Remove last migration | `dotnet ef migrations remove --project MainSchoolsManagementSystem` |
| Drop the database | `dotnet ef database drop --project MainSchoolsManagementSystem` |

---

## 10. File Uploads

The application stores uploaded files locally:

| Upload Type | Storage Path |
|---|---|
| Lesson plans | `MainSchoolsManagementSystem/uploads/` |
| Feed media | `MainSchoolsManagementSystem/uploads/feed/` |
| Profile pictures | `MainSchoolsManagementSystem/wwwroot/uploads/profile-pictures/` |

These directories are created automatically at runtime if they don't exist. They are **not** checked into source control — each developer starts with empty upload directories.

---

## Troubleshooting

### "Connection string 'DefaultConnection' not found"
Make sure `appsettings.json` exists in the `MainSchoolsManagementSystem/` directory and contains the `ConnectionStrings` section.

### "Cannot connect to SQL Server"
- Verify SQL Server is running: `Get-Service *SQL*` in PowerShell.
- Check that the `Server=` value in your connection string matches your SQL Server instance name.
- Common values: `localhost`, `.\SQLEXPRESS`, `(localdb)\MSSQLLocalDB`.

### "Login failed for user"
If using `Trusted_Connection=True`, your Windows account must have access to SQL Server. Alternatively, switch to SQL Server Authentication:
```json
"DefaultConnection": "Server=localhost;Database=SchoolsManagementSystem;User Id=sa;Password=YourPassword;TrustServerCertificate=True"
```

### Database Already Exists with Old Schema
Drop and recreate:
```powershell
dotnet ef database drop --project MainSchoolsManagementSystem --force
dotnet run --project MainSchoolsManagementSystem
```
The app will recreate and re-seed the database on next startup.

### Playwright Browsers Not Found
Run the browser installation script as described in [Step 8](#8-running-tests).
