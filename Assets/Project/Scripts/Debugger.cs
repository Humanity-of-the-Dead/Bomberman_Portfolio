using System.Diagnostics;

/// <summary>
/// 条件付きでログを出力するデバッグ用ラッパークラス
/// </summary>
public static class Debugger{
    [Conditional("DEBUG")]
    public static void Log(object message){
        UnityEngine.Debug.Log(message);
    }    
    
    [Conditional("DEBUG")]
    public static void Log(object message,UnityEngine.Object context){
        UnityEngine.Debug.Log(message,context);
    }

    [Conditional("DEBUG")]
    public static void LogWarning(object message){
        UnityEngine.Debug.LogWarning(message);
    }
    
    [Conditional("DEBUG")]
    public static void LogWarning(object message,UnityEngine.Object context){
        UnityEngine.Debug.LogWarning(message,context);
    }

    [Conditional("DEBUG")]
    public static void LogError(object message){
        UnityEngine.Debug.LogError(message);
    }
    
    [Conditional("DEBUG")]
    public static void LogError(object message,UnityEngine.Object context){
        UnityEngine.Debug.LogError(message,context);
    }
}
