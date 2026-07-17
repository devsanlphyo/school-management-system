using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using MainSchoolsManagementSystem.Data;
using MainSchoolsManagementSystem.Features.Admin.Models;

namespace MainSchoolsManagementSystem.Features.Admin.Middlewares
{
    public class ErrorLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                // We resolve DbContext from request services to avoid scope issues in Singleton middleware
                using (var scope = context.RequestServices.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    
                    var errorLog = new SystemErrorLog
                    {
                        ExceptionMessage = ex.Message,
                        StackTrace = ex.StackTrace ?? "",
                        RequestPath = context.Request.Path,
                        CreatedAt = DateTime.UtcNow,
                        IsResolved = false
                    };

                    dbContext.SystemErrorLogs.Add(errorLog);
                    await dbContext.SaveChangesAsync();
                }

                // Re-throw the exception so the default error page handles it
                throw;
            }
        }
    }
}
