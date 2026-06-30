using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading.Tasks;
using Xunit;

namespace MainSchoolsManagementSystem.Tests
{
    public class WebAppFixture : IAsyncLifetime
    {
        private Process? _process;
        public string BaseUrl { get; private set; } = string.Empty;

        public async Task InitializeAsync()
        {
            int port = GetFreePort();
            BaseUrl = $"http://127.0.0.1:{port}";

            string solutionRoot = FindSolutionRoot();
            string dllPath = Path.Combine(solutionRoot, "MainSchoolsManagementSystem", "bin", "Debug", "net8.0", "MainSchoolsManagementSystem.dll");

            if (!File.Exists(dllPath))
            {
                // Fallback to project run if DLL is not yet built, but we expect it to be built.
                // However, running the DLL directly is preferred for clean process tracking.
                throw new FileNotFoundException($"Main application DLL not found at: {dllPath}. Ensure the project is built before running tests.", dllPath);
            }

            var startInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"\"{dllPath}\" --urls {BaseUrl}",
                WorkingDirectory = Path.Combine(solutionRoot, "MainSchoolsManagementSystem"),
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            startInfo.EnvironmentVariables["ASPNETCORE_ENVIRONMENT"] = "Development";

            _process = new Process { StartInfo = startInfo };

            // Set up logging for diagnostics
            _process.OutputDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    Console.WriteLine($"[App StdOut] {e.Data}");
                }
            };
            _process.ErrorDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    Console.Error.WriteLine($"[App StdErr] {e.Data}");
                }
            };

            _process.Start();
            _process.BeginOutputReadLine();
            _process.BeginErrorReadLine();

            // Wait for the server to be responsive
            using var client = new HttpClient();
            var stopwatch = Stopwatch.StartNew();
            bool isResponsive = false;

            while (stopwatch.Elapsed < TimeSpan.FromSeconds(20))
            {
                if (_process.HasExited)
                {
                    throw new InvalidOperationException($"Web application exited prematurely. Exit code: {_process.ExitCode}");
                }

                try
                {
                    var response = await client.GetAsync(BaseUrl);
                    // Any status code indicates the server is listening and responding to HTTP
                    if (response.StatusCode != HttpStatusCode.ServiceUnavailable)
                    {
                        isResponsive = true;
                        break;
                    }
                }
                catch (HttpRequestException)
                {
                    // Expected when port is not yet listening
                }
                catch (SocketException)
                {
                    // Expected when port is not yet listening
                }

                await Task.Delay(200);
            }

            if (!isResponsive)
            {
                throw new InvalidOperationException($"Web application failed to become responsive at {BaseUrl} within the timeout period.");
            }
        }

        public Task DisposeAsync()
        {
            if (_process != null && !_process.HasExited)
            {
                try
                {
                    // Kill the process and all its children
                    _process.Kill(entireProcessTree: true);
                }
                catch
                {
                    try
                    {
                        _process.Kill();
                    }
                    catch
                    {
                        // Ignore
                    }
                }
                _process.Dispose();
            }
            return Task.CompletedTask;
        }

        private static int GetFreePort()
        {
            using var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(new IPEndPoint(IPAddress.Loopback, 0));
            return ((IPEndPoint)socket.LocalEndPoint!).Port;
        }

        private static string FindSolutionRoot()
        {
            var currentDir = new DirectoryInfo(AppContext.BaseDirectory);
            while (currentDir != null && !File.Exists(Path.Combine(currentDir.FullName, "SchoolsManagementSystem.sln")))
            {
                currentDir = currentDir.Parent;
            }
            if (currentDir == null)
            {
                throw new InvalidOperationException("Could not find solution root directory containing SchoolsManagementSystem.sln");
            }
            return currentDir.FullName;
        }
    }
}
