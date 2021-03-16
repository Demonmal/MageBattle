using UnityEngine;

namespace MageBattle.Utility
{
    public class DebugUtility : MonoBehaviour
    {
        public static void Log(Color color, object message)
        {
#if UNITY_EDITOR
            Debug.Log(StringBuilderUtility.GetColoredString(color, message));
#endif
        }

        public static void Log(object message)
        {
#if UNITY_EDITOR
            Debug.Log(message);
#endif
        }

        public static void LogBold(object message)
        {
#if UNITY_EDITOR
            Debug.Log(StringBuilderUtility.GetBoldString(message));
#endif
        }

        public static void LogError(object message)
        {
#if UNITY_EDITOR
            Debug.LogError(message);
#endif
        }
    }
}