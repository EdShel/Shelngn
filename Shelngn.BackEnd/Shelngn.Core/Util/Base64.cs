namespace Shelngn
{
    public static class Base64
    {
        public static string ToUrlSafe(byte[] data)
        {
            return Convert.ToBase64String(data).Replace('+', '-').Replace('/', '_');
        }
        public static byte[] FromUrlSafe(string urlSafeBase64)
        {
            return Convert.FromBase64String(urlSafeBase64.Replace('-', '+').Replace('_', '/'));
        }
    }
}
