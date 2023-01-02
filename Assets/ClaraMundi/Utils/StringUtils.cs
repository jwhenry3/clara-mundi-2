using System;

namespace ClaraMundi
{
    public static class StringUtils
    {
        public static string UniqueId()
        {
            return Convert.ToBase64String(Guid.NewGuid().ToByteArray())
                .Replace("=", "")
                .Replace("+", "-")
                .Replace("/", "_");
        }
    }
}