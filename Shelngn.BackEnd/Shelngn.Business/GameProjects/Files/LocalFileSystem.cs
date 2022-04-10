﻿using Shelngn.Services.GameProjects;
using Shelngn.Services.GameProjects.Files;

namespace Shelngn.Business.GameProjects.Files
{
    public class LocalFileSystem : IFileSystem
    {
        public Task CreateDirectoryOrDoNothingIfExistsAsync(string uri)
        {
            Directory.CreateDirectory(uri);
            return Task.CompletedTask;
        }

        public async Task CreateOrOverwriteFileAsync(string uri, byte[] fileContent, CancellationToken ct = default)
        {
            await File.WriteAllBytesAsync(uri, fileContent, ct);
        }

        public Task<ProjectDirectory?> ListDirectoryFilesAsync(string uri, CancellationToken ct = default)
        {
            if (!Directory.Exists(uri))
            {
                return Task.FromResult<ProjectDirectory?>(null);
            }
            var root = ListDirectoryFiles(uri);
            return Task.FromResult<ProjectDirectory?>(root);
        }

        public ProjectDirectory ListDirectoryFiles(string path)
        {
            string[] fileNames = Directory.GetFiles(path);
            ProjectFile[] files = fileNames.Select(f =>
            {
                return new ProjectFile(f);
            }).ToArray();
            string[] directoriesNames = Directory.GetDirectories(path);
            ProjectDirectory[] directories = directoriesNames.Select(d => ListDirectoryFiles(d)).ToArray();
            return new ProjectDirectory(
                fullPath: path,
                directories: directories,
                files: files
            );
        }

        public Task DeleteFileIfExistsAsync(string uri)
        {
            if (File.Exists(uri))
            {
                File.Delete(uri);
            }
            return Task.CompletedTask;
        }
        public Task DeleteDirectoryIfExistsAsync(string uri)
        {
            if (Directory.Exists(uri))
            {
                Directory.Delete(uri, true);
            }
            return Task.CompletedTask;
        }

        public Task MoveFileAsync(string sourceUri, string destinationUri)
        {
            File.Move(sourceUri, destinationUri, overwrite: true);
            return Task.CompletedTask;
        }

        public Task MoveDirectoryAsync(string sourcePath, string destinationPath)
        {
            Directory.Move(sourcePath, destinationPath);
            return Task.CompletedTask;
        }
    }
}
