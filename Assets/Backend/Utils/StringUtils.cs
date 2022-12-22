using System;

public static class StringUtils
{
    public static string UniqueId()
    {
        Guid guid = Guid.NewGuid();
        string value = Convert.ToBase64String(guid.ToByteArray())
            .Replace('+', '-') // escape (for filepath)
            .Replace('/', '_'); // escape (for filepath)
        
        return value.Substring(0, value.Length - 2); // remove trailing ==
    }
}