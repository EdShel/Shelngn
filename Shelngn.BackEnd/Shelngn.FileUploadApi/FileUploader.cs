namespace Shelngn.FileUploadApi
{
    public class FileUploader
    {
        private readonly string baseDirectory;

        public FileUploader(string baseDirectory)
        {
            this.baseDirectory = baseDirectory;
        }

        public async Task CreateOrAppendAsync(string path, Stream content)
        {
            var filePath = Path.Combine(baseDirectory, path);
            var directoryPath = Path.GetDirectoryName(filePath)!;
            Directory.CreateDirectory(directoryPath);
            using (var fs = new FileStream(filePath, FileMode.Append, FileAccess.Write))
            {
                await content.CopyToAsync(fs);
            }
        }
    }
}
