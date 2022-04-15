using Microsoft.Extensions.Logging;
using Shelngn.Services.GameProjects.Build;
using System.Diagnostics;

namespace Shelngn.Business.GameProjects.Build
{
    public class GameProjectBuilder : IGameProjectBuilder
    {
        private readonly string projectsDirectory;
        private readonly ILogger<GameProjectBuilder> logger;

        public GameProjectBuilder(string projectsDirectory, ILogger<GameProjectBuilder> logger)
        {
            this.projectsDirectory = projectsDirectory;
            this.logger = logger;
        }

        public async Task<BuildResult> BuildProjectAsync(string gameProjectId)
        {
            var webpackProcess = new ProcessStartInfo
            {
                FileName = "cmd",
                WorkingDirectory = projectsDirectory,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                //CreateNoWindow = true,
                UseShellExecute = false,
                Arguments = $"/c npx webpack build --config ./{gameProjectId}/webpack.config.js --entry ./{gameProjectId}/main.js"
            };

            Process webpack = Process.Start(webpackProcess)
                ?? throw new Exception("No webpack process has been started.");

            webpack.WaitForExit();

            string output = await webpack.StandardOutput.ReadToEndAsync();
            string error = await webpack.StandardError.ReadToEndAsync();

            logger.LogInformation("Built project: {output}", output);

            return new BuildResult
            {
                Error = error,
                IsSuccess = string.IsNullOrEmpty(error)
            };
        }
    }
}
