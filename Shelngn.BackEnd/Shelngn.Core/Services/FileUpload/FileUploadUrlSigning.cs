namespace Shelngn.Services.FileUpload
{
    public interface IFileUploadUrlSigning
    {
        string CreateSignature(string filePath, string contentType);
        bool ValidateSignature(string signature, string filePath, string contentType);
    }
}
