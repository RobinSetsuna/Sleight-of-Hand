using UnityEngine;

public struct LogUtility
{
    public static string MakeLogString(string type, object content)
    {
        return "[" + System.DateTime.Now.ToShortTimeString() + "][" + type + "] " + content.ToString();
    }

    public static void PrintLog(string type, object content)
    {
#if UNITY_EDITOR
        Debug.Log(MakeLogString(type, content));
#endif
    }
}
