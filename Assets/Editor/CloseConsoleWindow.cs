// Assets/Editor/CloseConsoleWindow.cs
using UnityEditor;
using UnityEngine;
using System;
using System.Reflection;

public static class CloseConsoleWindow
{
    [MenuItem("Window/Custom/Close Console %#x")] // Ctrl + Shift + X
    public static void CloseConsole()
    {
        var consoleType = Type.GetType("UnityEditor.ConsoleWindow, UnityEditor.dll");

        if (consoleType == null)
        {
            Debug.LogWarning("No se pudo encontrar el tipo ConsoleWindow.");
            return;
        }

        var windows = Resources.FindObjectsOfTypeAll(consoleType);

        foreach (var window in windows)
        {
            if (window is EditorWindow editorWindow)
            {
                editorWindow.Close();
                return;
            }
        }

        Debug.Log("No hay una ventana de consola abierta.");
    }

    [MenuItem("Window/Custom/Clear Console %#z")] // Ctrl + Shift + Z
    public static void ClearConsole()
    {
        var logEntries = Type.GetType("UnityEditor.LogEntries, UnityEditor.dll");
        var clearMethod = logEntries?.GetMethod("Clear", BindingFlags.Static | BindingFlags.Public);

        if (clearMethod != null)
        {
            clearMethod.Invoke(null, null);
        }
        else
        {
            Debug.LogWarning("No se pudo acceder al método Clear para limpiar la consola.");
        }
    }
}



