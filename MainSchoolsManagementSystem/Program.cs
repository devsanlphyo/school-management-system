using MainSchoolsManagementSystem.Components;
using MainSchoolsManagementSystem.Components.Account;
using MainSchoolsManagementSystem.Data;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
// Services are resolved via global usings in GlobalUsings.cs


var builder = WebApplication.CreateBuilder(args);

// Configure Kestrel to allow large file uploads (200MB max)
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 210 * 1024 * 1024; // 210MB
});

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();

var databaseProvider = builder.Configuration.GetValue<string>("DatabaseProvider") ?? "Sqlite";

if (databaseProvider.Equals("SqlServer", StringComparison.OrdinalIgnoreCase))
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
        options.UseSqlServer(connectionString));
}
else
{
    var sqliteConnectionString = builder.Configuration.GetConnectionString("SqliteConnection") ?? "Data Source=SchoolsManagement.db";
    builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
        options.UseSqlite(sqliteConnectionString));
}
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<ISchoolService, SchoolService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ISystemSettingService, SystemSettingService>();
builder.Services.AddScoped<ILessonPlanService, LessonPlanService>();
builder.Services.AddScoped<IAttendanceService, AttendanceService>();
builder.Services.AddScoped<ILeaveRequestService, LeaveRequestService>();
builder.Services.AddScoped<IFeedService, FeedService>();
builder.Services.AddScoped<ITrustedDeviceService, TrustedDeviceService>();
builder.Services.AddScoped<INotificationService, NotificationService>();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseMiddleware<MainSchoolsManagementSystem.Features.Admin.Middlewares.ErrorLoggingMiddleware>();

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

app.MapGet("/api/lesson-plans/{id:int}/download", async (
    int id,
    System.Security.Claims.ClaimsPrincipal user,
    IDbContextFactory<ApplicationDbContext> dbContextFactory,
    IWebHostEnvironment env) =>
{
    var currentUserId = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
    if (string.IsNullOrEmpty(currentUserId))
    {
        return Results.Unauthorized();
    }

    using var db = await dbContextFactory.CreateDbContextAsync();
    var lessonPlan = await db.LessonPlans
        .Include(lp => lp.Teacher)
        .FirstOrDefaultAsync(lp => lp.Id == id);

    if (lessonPlan == null)
    {
        return Results.NotFound();
    }

    bool isAuthorized = false;
    if (user.IsInRole("Teacher") && lessonPlan.TeacherId == currentUserId)
    {
        isAuthorized = true;
    }
    if (user.IsInRole("Admin") || user.IsInRole("Headmaster") || user.IsInRole("Officer"))
    {
        var currentUser = await db.Users.FindAsync(currentUserId);
        if (currentUser != null && currentUser.SchoolId != null && currentUser.SchoolId == lessonPlan.Teacher?.SchoolId)
        {
            isAuthorized = true;
        }
    }

    if (!isAuthorized)
    {
        return Results.Forbid();
    }

    var uploadsPath = Path.Combine(env.ContentRootPath, "uploads");
    if (!Directory.Exists(uploadsPath))
    {
        return Results.NotFound();
    }

    var matchingFiles = Directory.EnumerateFiles(uploadsPath, $"lessonplan_{id}.*").ToList();
    if (!matchingFiles.Any())
    {
        return Results.NotFound();
    }

    var filePath = matchingFiles.First();
    var provider = new Microsoft.AspNetCore.StaticFiles.FileExtensionContentTypeProvider();
    if (!provider.TryGetContentType(filePath, out var contentType))
    {
        contentType = "application/octet-stream";
    }

    return Results.File(filePath, contentType, fileDownloadName: Path.GetFileName(filePath));
})
.RequireAuthorization();

app.MapGet("/api/lesson-plans/{id:int}/justification/download", async (
    int id,
    System.Security.Claims.ClaimsPrincipal user,
    IDbContextFactory<ApplicationDbContext> dbContextFactory,
    IWebHostEnvironment env) =>
{
    var currentUserId = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
    if (string.IsNullOrEmpty(currentUserId))
    {
        return Results.Unauthorized();
    }

    using var db = await dbContextFactory.CreateDbContextAsync();
    var lessonPlan = await db.LessonPlans
        .Include(lp => lp.Teacher)
        .FirstOrDefaultAsync(lp => lp.Id == id);

    if (lessonPlan == null)
    {
        return Results.NotFound();
    }

    bool isAuthorized = false;
    if (user.IsInRole("Teacher") && lessonPlan.TeacherId == currentUserId)
    {
        isAuthorized = true;
    }
    if (user.IsInRole("Admin") || user.IsInRole("Headmaster") || user.IsInRole("Officer"))
    {
        var currentUser = await db.Users.FindAsync(currentUserId);
        if (currentUser != null && currentUser.SchoolId != null && currentUser.SchoolId == lessonPlan.Teacher?.SchoolId)
        {
            isAuthorized = true;
        }
    }

    if (!isAuthorized)
    {
        return Results.Forbid();
    }

    var uploadsPath = Path.Combine(env.ContentRootPath, "uploads");
    if (!Directory.Exists(uploadsPath))
    {
        return Results.NotFound();
    }

    var matchingFiles = Directory.EnumerateFiles(uploadsPath, $"justification_{id}.*").ToList();
    if (!matchingFiles.Any())
    {
        return Results.NotFound();
    }

    var filePath = matchingFiles.First();
    var provider = new Microsoft.AspNetCore.StaticFiles.FileExtensionContentTypeProvider();
    if (!provider.TryGetContentType(filePath, out var contentType))
    {
        contentType = "application/octet-stream";
    }

    return Results.File(filePath, contentType, fileDownloadName: Path.GetFileName(filePath));
})
.RequireAuthorization();

app.MapPost("/api/feed/upload", async (
    Microsoft.AspNetCore.Http.HttpRequest request,
    System.Security.Claims.ClaimsPrincipal user,
    IDbContextFactory<ApplicationDbContext> dbContextFactory,
    IWebHostEnvironment env) =>
{
    var currentUserId = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
    if (string.IsNullOrEmpty(currentUserId)) return (IResult)Results.Unauthorized();

    if (!request.HasFormContentType) return Results.BadRequest("Unsupported media type");

    var form = await request.ReadFormAsync();
    var files = form.Files;

    if (files.Count > 20) return Results.BadRequest("Maximum 20 files allowed");

    var uploadsPath = Path.Combine(env.ContentRootPath, "uploads", "feed");
    if (!Directory.Exists(uploadsPath)) Directory.CreateDirectory(uploadsPath);

    var uploadedFiles = new List<object>();

    foreach (var file in files)
    {
        if (file.Length > 200 * 1024 * 1024) return Results.BadRequest($"File {file.FileName} exceeds 200MB limit");

        var storedFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
        var filePath = Path.Combine(uploadsPath, storedFileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        uploadedFiles.Add(new
        {
            fileName = file.FileName,
            storedFileName = storedFileName,
            contentType = file.ContentType,
            fileSize = file.Length
        });
    }

    return Results.Ok(uploadedFiles);
})
.RequireAuthorization();

app.MapGet("/api/feed/media/{storedFileName}/view", (
    string storedFileName,
    System.Security.Claims.ClaimsPrincipal user,
    IWebHostEnvironment env) =>
{
    var currentUserId = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
    if (string.IsNullOrEmpty(currentUserId)) return (IResult)Results.Unauthorized();

    var uploadsPath = Path.Combine(env.ContentRootPath, "uploads", "feed");
    var filePath = Path.Combine(uploadsPath, storedFileName);

    if (!System.IO.File.Exists(filePath)) return Results.NotFound();

    var provider = new Microsoft.AspNetCore.StaticFiles.FileExtensionContentTypeProvider();
    if (!provider.TryGetContentType(filePath, out var contentType))
    {
        contentType = "application/octet-stream";
    }

    return Results.File(filePath, contentType); // Served inline for rendering inside img/video tags
})
.RequireAuthorization();

app.MapGet("/api/feed/media/{storedFileName}/download", (
    string storedFileName,
    System.Security.Claims.ClaimsPrincipal user,
    IWebHostEnvironment env) =>
{
    var currentUserId = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
    if (string.IsNullOrEmpty(currentUserId)) return (IResult)Results.Unauthorized();

    var uploadsPath = Path.Combine(env.ContentRootPath, "uploads", "feed");
    var filePath = Path.Combine(uploadsPath, storedFileName);

    if (!System.IO.File.Exists(filePath)) return Results.NotFound();

    var provider = new Microsoft.AspNetCore.StaticFiles.FileExtensionContentTypeProvider();
    if (!provider.TryGetContentType(filePath, out var contentType))
    {
        contentType = "application/octet-stream";
    }

    return Results.File(filePath, contentType, fileDownloadName: storedFileName); // Triggers download prompt
})
.RequireAuthorization();

// Seed database with roles, schools, and default users
await MainSchoolsManagementSystem.Data.DbSeeder.SeedAsync(app.Services);

var uploadsPath = Path.Combine(builder.Environment.ContentRootPath, "uploads");
if (!Directory.Exists(uploadsPath))
{
    Directory.CreateDirectory(uploadsPath);
}
var feedUploadsPath = Path.Combine(uploadsPath, "feed");
if (!Directory.Exists(feedUploadsPath))
{
    Directory.CreateDirectory(feedUploadsPath);
}

app.Run();

public partial class Program { }
