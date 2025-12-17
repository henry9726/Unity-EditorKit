using UnityEngine;

namespace Henry.EditorKit
{
    static class LogPrinter
    {
        public static void Print(string message)
        {
            Debug.Log($"[EditorKit] {message}");
        }

        public static void PrintWarning(string message)
        {
            Debug.LogWarning($"[EditorKit] {message}");
        }

        public static void PrintError(string message)
        {
            Debug.LogError($"[EditorKit] {message}");
        }
    }
}