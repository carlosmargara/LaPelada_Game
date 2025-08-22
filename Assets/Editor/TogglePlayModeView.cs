using UnityEditor;
using System;
using System.Reflection;

public static class TogglePlayModeView
{
    [MenuItem("Tools/Toggle Play Mode View %&f")] // Ctrl+Alt+F
    static void ToggleView()
    {
        // Obtiene el tipo interno
        var playModeViewType = typeof(Editor).Assembly.GetType("UnityEditor.PlayModeView");
        if (playModeViewType == null) return;

        // Campo interno que guarda el estado "maximizeOnPlay"
        var prop = playModeViewType.GetProperty("maximizeOnPlay", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
        if (prop == null) return;

        bool current = (bool)prop.GetValue(null, null);
        prop.SetValue(null, !current, null);

    }
}
