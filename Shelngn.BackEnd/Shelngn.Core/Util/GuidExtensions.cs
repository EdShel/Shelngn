namespace Shelngn
{
    public static class GuidExtensions
    {
        public static string ToUrlSafeBase64(this Guid guid)
        {
            return Convert.ToBase64String(guid.ToByteArray())
                .Replace('+', '-')
                .Replace('/', '_')
                .Substring(0, 22); // the last 2 of 24 chars are == which ain't url-safe
        }

        public static Guid FromUrlSafeBase64(string urlSafeBase64)
        {
            return new Guid(Convert.FromBase64String(urlSafeBase64
                .Replace('-', '+')
                .Replace('_', '_') + "=="
            ));
        }
    }
}
