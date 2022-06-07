using Shelngn.Services.FileUpload;
using System.Security.Cryptography;
using System.Text;

namespace Shelngn.Business.FileUpload
{
    public class FileUploadUrlSigning : IFileUploadUrlSigning
    {
        private readonly string privateKey;

        public FileUploadUrlSigning(string privateKey)
        {
            this.privateKey = privateKey;
        }

        public string CreateSignature(string filePath, string contentType)
        {
            string hashedString = filePath + contentType + this.privateKey;
            byte[] hashedData = Encoding.UTF8.GetBytes(hashedString);

            byte[] hash = SHA256.HashData(hashedData);
            return Base64.ToUrlSafe(hash);
        }

        public bool ValidateSignature(string signature, string filePath, string contentType)
        {
            return CreateSignature(filePath, contentType) == signature;
        }
    }
}
