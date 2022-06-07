using Shelngn.Business.FileUpload;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Shelngn.Tests.Services.FileUpload
{
    public class FileUploadUrlSigningTest
    {
        [Fact]
        public void CreateSignature_Always_GeneratesValidSignature()
        {
            string filePath = "/foo.bar";
            string contentType = "image/png";
            var signer = new FileUploadUrlSigning("private key");

            string signature = signer.CreateSignature(filePath, contentType);
            bool isValid = signer.ValidateSignature(signature, filePath, contentType);

            Assert.True(isValid);
        }

        [Fact]
        public void ValidateSignature_IfFilePathIsInvalid_ReturnsFalse()
        {
            string filePath = "/foo.bar";
            string contentType = "image/png";
            var signer = new FileUploadUrlSigning("private key");

            string signature = signer.CreateSignature(filePath, contentType);
            bool isValid = signer.ValidateSignature(signature, "/aaaaaa.aaa", contentType);

            Assert.False(isValid);
        }

        [Fact]
        public void ValidateSignature_IfContentTypeIsInvalid_ReturnsFalse()
        {
            string filePath = "/foo.bar";
            string contentType = "image/png";
            var signer = new FileUploadUrlSigning("private key");

            string signature = signer.CreateSignature(filePath, contentType);
            bool isValid = signer.ValidateSignature(signature, filePath, "text/html");

            Assert.False(isValid);
        }

        [Fact]
        public void ValidateSignature_IfPrivateKeyIsDifferent_ReturnsFalse()
        {
            string filePath = "/foo.bar";
            string contentType = "image/png";
            var signer1 = new FileUploadUrlSigning("private key");
            var signer2 = new FileUploadUrlSigning("another key");

            string signature = signer1.CreateSignature(filePath, contentType);
            bool isValid = signer2.ValidateSignature(signature, filePath, contentType);

            Assert.False(isValid);
        }
    }
}
