
using UnityEngine;

public static class Debug 
{
    public static void Log(string info)
    {
        UnityEngine.Debug.Log(info);
    }
    public static void LogError(string info)
    {
        UnityEngine.Debug.LogError(info);
    }
    public static void LogWarning(string info)
    {
        UnityEngine.Debug.LogWarning(info);
    }
}