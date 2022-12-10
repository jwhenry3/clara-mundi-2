using System.Diagnostics;

namespace ClaraMundi
{
    using UnityEngine;

    public class LoggingUtils
    {

        public static void Error(string message)
        {
            Debug.LogError(message);
        }
        public static void Info(string message)
        {
            Debug.Log(message);
        }

        public static void Warn(string message)
        {
            Debug.LogWarning(message);
        }
    }
}