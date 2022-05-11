using Microsoft.Extensions.Logging;
using Shelngn.Services.GameProjects.Build;
using System.Diagnostics;
using System.Text.RegularExpressions;

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
                WorkingDirectory = Path.Combine(projectsDirectory, gameProjectId),
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                Arguments = $"/c npx webpack build --config ./webpack.config.js --env workspace={gameProjectId} --no-color"
            };

            Process webpack = Process.Start(webpackProcess)
                ?? throw new Exception("No webpack process has been started.");

            webpack.WaitForExit();

            string buildOutput = await webpack.StandardOutput.ReadToEndAsync();
            string webpackError = await webpack.StandardError.ReadToEndAsync();
            string? buiildError = ParseError(buildOutput);
            string error = buiildError ?? webpackError;

            logger.LogInformation("Built project: {output}", buildOutput);

            return new BuildResult
            {
                Error = error,
                IsSuccess = string.IsNullOrEmpty(error)
            };
        }

        private string? ParseError(string webpackOutput)
        {
            Regex errorRegex = new Regex(@"ERROR [^\r\n]+\r?\n[^\r\n]+");
            Match errorMatch = errorRegex.Match(webpackOutput);
            if (!errorMatch.Success)
            {
                return null;
            }
            return errorMatch.Value;
        }
    }
}
