using System;
using Object = UnityEngine.Object;

namespace Ib_Core
{
    public class Ib_Log
    {
        public static void Debug(string content)
        {
#if UNITY_EDITOR
            UnityEngine.Debug.Log(content);
#endif
        }

        public static void Info(string content)
        {
            UnityEngine.Debug.Log(content);
        }

        public static void Warning(string content)
        {
            UnityEngine.Debug.LogWarning(content);
        }

        public static void Error(string content)
        {
            UnityEngine.Debug.LogError(content);
        }
        public static void Error(Exception ex)
        {
            UnityEngine.Debug.LogError(ex);
        }
        public static void Error(Exception ex,Object context)
        {
            UnityEngine.Debug.LogError(ex,context);
        }
    }
}