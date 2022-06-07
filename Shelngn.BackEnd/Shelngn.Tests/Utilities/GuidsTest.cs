using System;
using Xunit;

namespace Shelngn.Tests.Utilities
{
    public class GuidsTest
    {
        [Fact]
        public void ToUrlSafeBase64_Always_ReplacesPlusWithMinus()
        {
            byte[] guidBytes = Convert.FromHexString("FB000000000000000000000000000000");
            Guid guid = new Guid(guidBytes);

            string result = Guids.ToUrlSafeBase64(guid);

            Assert.NotEqual("+wAAAAAAAAAAAAAAAAAAAA==", result);
            Assert.Equal("-wAAAAAAAAAAAAAAAAAAAA", result);
        }

        [Fact]
        public void ToUrlSafeBase64_Always_ReplacesSlashWithUnderscore()
        {
            byte[] guidBytes = Convert.FromHexString("FC000000000000000000000000000000");
            Guid guid = new Guid(guidBytes);

            string result = Guids.ToUrlSafeBase64(guid);

            Assert.NotEqual("/AAAAAAAAAAAAAAAAAAAAA==", result);
            Assert.Equal("_AAAAAAAAAAAAAAAAAAAAA", result);
        }

        [Fact]
        public void FromUrlSafeBase64_Always_ReplacesMinusWithPlus()
        {
            string base64 = "-wAAAAAAAAAAAAAAAAAAAA";

            Guid guid = Guids.FromUrlSafeBase64(base64);

            Assert.Equal("FB000000000000000000000000000000", Convert.ToHexString(guid.ToByteArray()));
        }

        [Fact]
        public void FromUrlSafeBase64_Always_ReplacesUnderscoreWithSlash()
        {
            string base64 = "_AAAAAAAAAAAAAAAAAAAAA";

            Guid guid = Guids.FromUrlSafeBase64(base64);

            Assert.Equal("FC000000000000000000000000000000", Convert.ToHexString(guid.ToByteArray()));
        }
    }
}
