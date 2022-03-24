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
            Directory.CreateDirectory(path);
            using (var fs = new FileStream(Path.Combine(baseDirectory, path), FileMode.Append, FileAccess.Write))
            {
                await content.CopyToAsync(fs);
            }
        }
    }
}
