using System.Threading.Tasks;
using Microsoft.Playwright;
using Xunit;

namespace MainSchoolsManagementSystem.Tests
{
    public class SanityTests : E2ETestBase
    {
        [Fact]
        public async Task Teacher_Can_Access_Feed_Successfully()
        {
            // Create an authenticated context for a Teacher
            var teacherContext = await CreateAuthenticatedContextAsync("Teacher");
            var page = await teacherContext.NewPageAsync();

            // Navigate to the teacher feed
            await page.GotoAsync($"{BaseUrl}/teacher/feed");

            // Verify the heading "Feed" is visible on the page
            await page.WaitForSelectorAsync("h3.card-title");
            var headerText = await page.Locator("h3.card-title").TextContentAsync();
            Assert.Contains("Feed", headerText);

            // Clean up
            await page.CloseAsync();
            await teacherContext.CloseAsync();
        }
    }
}
