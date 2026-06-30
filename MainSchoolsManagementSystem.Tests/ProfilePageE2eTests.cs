using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Xunit;

namespace MainSchoolsManagementSystem.Tests
{
    public class ProfilePageE2eTests : IClassFixture<WebAppFixture>
    {
        private readonly WebAppFixture _fixture;

        public ProfilePageE2eTests(WebAppFixture fixture)
        {
            _fixture = fixture;
        }

        private async Task RunTestWithPageAsync(Func<IPage, Task> testAction)
        {
            using var playwright = await Playwright.CreateAsync();
            await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = true,
                Args = new[] { "--no-sandbox", "--disable-setuid-sandbox" }
            });
            var context = await browser.NewContextAsync();
            var page = await context.NewPageAsync();

            try
            {
                await testAction(page);
            }
            finally
            {
                await context.CloseAsync();
                await browser.CloseAsync();
            }
        }

        private async Task LoginAsTeacherAsync(IPage page)
        {
            await page.GotoAsync($"{_fixture.BaseUrl}/Account/Login");
            
            // Fill credentials using placeholders from Login.razor
            await page.FillAsync("input[placeholder='name@example.com']", "teacher@stjude.edu");
            await page.FillAsync("input[placeholder='••••••••']", "Password123!");
            
            // Click Sign In
            await page.ClickAsync("button[type='submit']");
            
            // Wait for navigation away from Login page
            await page.WaitForURLAsync(url => !url.Contains("/Account/Login"), new PageWaitForURLOptions { Timeout = 10000 });
        }

        private async Task NavigateToProfileAsync(IPage page)
        {
            await page.GotoAsync($"{_fixture.BaseUrl}/Account/Manage");
            await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        #region Helper: Create Temporary Test Files
        private string CreateTempFile(string fileName, long sizeInBytes, string content = "test content")
        {
            var tempPath = Path.Combine(Path.GetTempPath(), fileName);
            using (var fs = new FileStream(tempPath, FileMode.Create, FileAccess.Write))
            {
                if (sizeInBytes > 0)
                {
                    fs.SetLength(sizeInBytes);
                    if (!string.IsNullOrEmpty(content))
                    {
                        using var writer = new StreamWriter(fs);
                        writer.Write(content);
                    }
                }
            }
            return tempPath;
        }
        #endregion

        #region Tier 1: Feature Coverage (>= 5 tests)

        [Fact]
        public async Task Tier1_UsernameDisplay_ShouldBeDisabled()
        {
            await RunTestWithPageAsync(async page =>
            {
                await LoginAsTeacherAsync(page);
                await NavigateToProfileAsync(page);

                // Username should be displayed and disabled
                var usernameInput = page.Locator("input[value='teacher@stjude.edu']");
                await Assertions.Expect(usernameInput).ToBeVisibleAsync();
                await Assertions.Expect(usernameInput).ToBeDisabledAsync();
            });
        }

        [Fact]
        public async Task Tier1_EditName_ShouldUpdateSuccessfully()
        {
            await RunTestWithPageAsync(async page =>
            {
                await LoginAsTeacherAsync(page);
                await NavigateToProfileAsync(page);

                // Edit Name
                var nameInput = page.Locator("input[name='Input.FullName']");
                await nameInput.FillAsync("Alice Updated");

                await page.ClickAsync("button[type='submit']");

                // Verify success status
                var statusMessage = page.Locator(".alert-success, [class*='success']");
                await Assertions.Expect(statusMessage).ToContainTextAsync("Your profile has been updated");
            });
        }

        [Fact]
        public async Task Tier1_EditPhone_ShouldUpdateSuccessfully()
        {
            await RunTestWithPageAsync(async page =>
            {
                await LoginAsTeacherAsync(page);
                await NavigateToProfileAsync(page);

                // Edit Phone
                var phoneInput = page.Locator("input[placeholder='Please enter your phone number.']");
                await phoneInput.FillAsync("9876543210");

                await page.ClickAsync("button[type='submit']");

                // Verify success status
                var statusMessage = page.Locator(".alert-success, [class*='success']");
                await Assertions.Expect(statusMessage).ToContainTextAsync("Your profile has been updated");
            });
        }

        [Fact]
        public async Task Tier1_SchoolAndDepartment_ShouldBeDisplayedCorrectly()
        {
            await RunTestWithPageAsync(async page =>
            {
                await LoginAsTeacherAsync(page);
                await NavigateToProfileAsync(page);

                // Verify School and Department displays are visible and correct
                var schoolText = page.Locator("#school-display, .school-info, :text('St. Jude Academy')");
                await Assertions.Expect(schoolText).ToBeVisibleAsync();
            });
        }

        [Fact]
        public async Task Tier1_ProfilePictureUpload_ShouldUploadSuccessfully()
        {
            await RunTestWithPageAsync(async page =>
            {
                await LoginAsTeacherAsync(page);
                await NavigateToProfileAsync(page);

                var tempJpg = CreateTempFile("test_avatar.jpg", 1024, "fake-jpeg-content");
                try
                {
                    // Upload profile picture
                    var fileInput = page.Locator("input[type='file']");
                    await fileInput.SetInputFilesAsync(tempJpg);

                    await page.ClickAsync("button[type='submit']");

                    // Verify success
                    var statusMessage = page.Locator(".alert-success, [class*='success']");
                    await Assertions.Expect(statusMessage).ToContainTextAsync("Your profile has been updated");
                }
                finally
                {
                    if (File.Exists(tempJpg)) File.Delete(tempJpg);
                }
            });
        }

        #endregion

        #region Tier 2: Boundary & Corner Cases (>= 5 tests)

        [Fact]
        public async Task Tier2_EmptyName_ShouldShowValidationError()
        {
            await RunTestWithPageAsync(async page =>
            {
                await LoginAsTeacherAsync(page);
                await NavigateToProfileAsync(page);

                var nameInput = page.Locator("input[name='Input.FullName']");
                await nameInput.FillAsync(""); // Empty Name

                await page.ClickAsync("button[type='submit']");

                // Verify validation error
                var validationMsg = page.Locator(".text-danger, [class*='danger'], [data-valmsg-for='Input.FullName']");
                await Assertions.Expect(validationMsg).ToContainTextAsync("required");
            });
        }

        [Fact]
        public async Task Tier2_InvalidPhoneFormat_ShouldShowValidationError()
        {
            await RunTestWithPageAsync(async page =>
            {
                await LoginAsTeacherAsync(page);
                await NavigateToProfileAsync(page);

                var phoneInput = page.Locator("input[placeholder='Please enter your phone number.']");
                await phoneInput.FillAsync("invalid-phone-123");

                await page.ClickAsync("button[type='submit']");

                // Verify validation error
                var validationMsg = page.Locator(".text-danger, [class*='danger'], [data-valmsg-for='Input.PhoneNumber']");
                await Assertions.Expect(validationMsg).ToContainTextAsync("phone");
            });
        }

        [Fact]
        public async Task Tier2_UploadNonImageFile_ShouldShowValidationError()
        {
            await RunTestWithPageAsync(async page =>
            {
                await LoginAsTeacherAsync(page);
                await NavigateToProfileAsync(page);

                var tempTxt = CreateTempFile("test_document.txt", 512, "This is a text file, not an image.");
                try
                {
                    var fileInput = page.Locator("input[type='file']");
                    await fileInput.SetInputFilesAsync(tempTxt);

                    await page.ClickAsync("button[type='submit']");

                    // Verify validation error for file type
                    var validationMsg = page.Locator(".text-danger, [class*='danger'], [data-valmsg-for='Input.ProfilePicture']");
                    await Assertions.Expect(validationMsg).ToContainTextAsync("image");
                }
                finally
                {
                    if (File.Exists(tempTxt)) File.Delete(tempTxt);
                }
            });
        }

        [Fact]
        public async Task Tier2_UploadImageTooLarge_ShouldShowValidationError()
        {
            await RunTestWithPageAsync(async page =>
            {
                await LoginAsTeacherAsync(page);
                await NavigateToProfileAsync(page);

                // 2.5 MB file
                var tempLargeJpg = CreateTempFile("large_avatar.jpg", (long)(2.5 * 1024 * 1024));
                try
                {
                    var fileInput = page.Locator("input[type='file']");
                    await fileInput.SetInputFilesAsync(tempLargeJpg);

                    await page.ClickAsync("button[type='submit']");

                    // Verify validation error for file size
                    var validationMsg = page.Locator(".text-danger, [class*='danger'], [data-valmsg-for='Input.ProfilePicture']");
                    await Assertions.Expect(validationMsg).ToContainTextAsync("2 MB");
                }
                finally
                {
                    if (File.Exists(tempLargeJpg)) File.Delete(tempLargeJpg);
                }
            });
        }

        [Fact]
        public async Task Tier2_ExtremelyLongName_ShouldShowValidationError()
        {
            await RunTestWithPageAsync(async page =>
            {
                await LoginAsTeacherAsync(page);
                await NavigateToProfileAsync(page);

                var nameInput = page.Locator("input[name='Input.FullName']");
                await nameInput.FillAsync(new string('A', 101)); // Length > 100

                await page.ClickAsync("button[type='submit']");

                // Verify validation error
                var validationMsg = page.Locator(".text-danger, [class*='danger'], [data-valmsg-for='Input.FullName']");
                await Assertions.Expect(validationMsg).ToContainTextAsync("length");
            });
        }

        #endregion

        #region Tier 3: Cross-Feature Combinations (>= 5 tests)

        [Fact]
        public async Task Tier3_ValidNamePhoneAndValidUpload_ShouldSucceed()
        {
            await RunTestWithPageAsync(async page =>
            {
                await LoginAsTeacherAsync(page);
                await NavigateToProfileAsync(page);

                var nameInput = page.Locator("input[name='Input.FullName']");
                await nameInput.FillAsync("Bob Johnson");

                var phoneInput = page.Locator("input[placeholder='Please enter your phone number.']");
                await phoneInput.FillAsync("5551234567");

                var tempJpg = CreateTempFile("valid_combo.jpg", 2048);
                try
                {
                    var fileInput = page.Locator("input[type='file']");
                    await fileInput.SetInputFilesAsync(tempJpg);

                    await page.ClickAsync("button[type='submit']");

                    var statusMessage = page.Locator(".alert-success, [class*='success']");
                    await Assertions.Expect(statusMessage).ToContainTextAsync("Your profile has been updated");
                }
                finally
                {
                    if (File.Exists(tempJpg)) File.Delete(tempJpg);
                }
            });
        }

        [Fact]
        public async Task Tier3_InvalidNameAndValidUpload_ShouldFailAndNotSave()
        {
            await RunTestWithPageAsync(async page =>
            {
                await LoginAsTeacherAsync(page);
                await NavigateToProfileAsync(page);

                var nameInput = page.Locator("input[name='Input.FullName']");
                await nameInput.FillAsync(""); // Invalid

                var tempJpg = CreateTempFile("valid_picture.jpg", 2048);
                try
                {
                    var fileInput = page.Locator("input[type='file']");
                    await fileInput.SetInputFilesAsync(tempJpg);

                    await page.ClickAsync("button[type='submit']");

                    // Should show validation error and NOT show success
                    var validationMsg = page.Locator(".text-danger, [class*='danger'], [data-valmsg-for='Input.FullName']");
                    await Assertions.Expect(validationMsg).ToBeVisibleAsync();

                    var statusMessage = page.Locator(".alert-success, [class*='success']");
                    await Assertions.Expect(statusMessage).Not.ToBeVisibleAsync();
                }
                finally
                {
                    if (File.Exists(tempJpg)) File.Delete(tempJpg);
                }
            });
        }

        [Fact]
        public async Task Tier3_ValidNameAndInvalidUpload_ShouldFailAndNotSave()
        {
            await RunTestWithPageAsync(async page =>
            {
                await LoginAsTeacherAsync(page);
                await NavigateToProfileAsync(page);

                var nameInput = page.Locator("input[name='Input.FullName']");
                await nameInput.FillAsync("Valid Name");

                var tempTxt = CreateTempFile("invalid_picture.txt", 1024, "not an image");
                try
                {
                    var fileInput = page.Locator("input[type='file']");
                    await fileInput.SetInputFilesAsync(tempTxt);

                    await page.ClickAsync("button[type='submit']");

                    // Should show validation error for file and NOT show success
                    var validationMsg = page.Locator(".text-danger, [class*='danger'], [data-valmsg-for='Input.ProfilePicture']");
                    await Assertions.Expect(validationMsg).ToBeVisibleAsync();

                    var statusMessage = page.Locator(".alert-success, [class*='success']");
                    await Assertions.Expect(statusMessage).Not.ToBeVisibleAsync();
                }
                finally
                {
                    if (File.Exists(tempTxt)) File.Delete(tempTxt);
                }
            });
        }

        [Fact]
        public async Task Tier3_MultipleInvalidFields_ShouldShowAllValidationErrors()
        {
            await RunTestWithPageAsync(async page =>
            {
                await LoginAsTeacherAsync(page);
                await NavigateToProfileAsync(page);

                var nameInput = page.Locator("input[name='Input.FullName']");
                await nameInput.FillAsync(""); // Invalid

                var phoneInput = page.Locator("input[placeholder='Please enter your phone number.']");
                await phoneInput.FillAsync("invalid-phone"); // Invalid

                var tempTxt = CreateTempFile("invalid_pic.txt", 512, "text file");
                try
                {
                    var fileInput = page.Locator("input[type='file']");
                    await fileInput.SetInputFilesAsync(tempTxt);

                    await page.ClickAsync("button[type='submit']");

                    // Check all three validation messages
                    var nameVal = page.Locator("[data-valmsg-for='Input.FullName']");
                    var phoneVal = page.Locator("[data-valmsg-for='Input.PhoneNumber']");
                    var fileVal = page.Locator("[data-valmsg-for='Input.ProfilePicture']");

                    await Assertions.Expect(nameVal).ToBeVisibleAsync();
                    await Assertions.Expect(phoneVal).ToBeVisibleAsync();
                    await Assertions.Expect(fileVal).ToBeVisibleAsync();
                }
                finally
                {
                    if (File.Exists(tempTxt)) File.Delete(tempTxt);
                }
            });
        }

        [Fact]
        public async Task Tier3_UploadAndCancel_ShouldNotPersistChanges()
        {
            await RunTestWithPageAsync(async page =>
            {
                await LoginAsTeacherAsync(page);
                await NavigateToProfileAsync(page);

                var nameInput = page.Locator("input[name='Input.FullName']");
                await nameInput.FillAsync("Temporary Name Change");

                // Instead of saving, we navigate away or reload
                await page.ReloadAsync();

                // Name should revert to original (which is seeded as 'Alice Johnson')
                var nameVal = await nameInput.InputValueAsync();
                Assert.NotEqual("Temporary Name Change", nameVal);
            });
        }

        #endregion

        #region Tier 4: Real-World Application Scenarios (1 full user journey)

        [Fact]
        public async Task Tier4_RealWorld_TeacherProfileFullJourney()
        {
            await RunTestWithPageAsync(async page =>
            {
                // 1. Login as teacher
                await LoginAsTeacherAsync(page);

                // 2. Navigate to Profile page
                await NavigateToProfileAsync(page);

                // 3. Verify initial profile details
                var usernameInput = page.Locator("input[value='teacher@stjude.edu']");
                await Assertions.Expect(usernameInput).ToBeDisabledAsync();

                var nameInput = page.Locator("input[name='Input.FullName']");
                var phoneInput = page.Locator("input[placeholder='Please enter your phone number.']");
                var schoolText = page.Locator("#school-display, .school-info, :text('St. Jude Academy')");

                await Assertions.Expect(schoolText).ToBeVisibleAsync();

                // 4. Perform edits (name, phone, upload profile picture)
                await nameInput.FillAsync("Alice Smith");
                await phoneInput.FillAsync("555-019-2834");

                var tempJpg = CreateTempFile("alice_avatar.jpg", 10 * 1024, "fake-avatar-data");
                try
                {
                    var fileInput = page.Locator("input[type='file']");
                    await fileInput.SetInputFilesAsync(tempJpg);

                    // 5. Submit changes
                    await page.ClickAsync("button[type='submit']");

                    // 6. Verify success message and updated UI
                    var statusMessage = page.Locator(".alert-success, [class*='success']");
                    await Assertions.Expect(statusMessage).ToContainTextAsync("Your profile has been updated");

                    // Verify avatar displays on page and header
                    var pageAvatar = page.Locator("img.profile-avatar, img#profile-avatar");
                    var headerAvatar = page.Locator("img.header-avatar, img#header-avatar");
                    await Assertions.Expect(pageAvatar).ToBeVisibleAsync();
                    await Assertions.Expect(headerAvatar).ToBeVisibleAsync();

                    // 7. Reload page and verify persistence
                    await page.ReloadAsync();
                    await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

                    await Assertions.Expect(nameInput).ToHaveValueAsync("Alice Smith");
                    await Assertions.Expect(phoneInput).ToHaveValueAsync("555-019-2834");
                    await Assertions.Expect(pageAvatar).ToBeVisibleAsync();
                }
                finally
                {
                    if (File.Exists(tempJpg)) File.Delete(tempJpg);
                }
            });
        }

        #endregion
    }
}
