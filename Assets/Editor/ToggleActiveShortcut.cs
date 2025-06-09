using UnityEngine;
using UnityEditor;

public class ToggleActiveShortcut
{
    [MenuItem("Tools/Toggle Active %t")] // Ctrl + T (Windows) / Cmd + T (Mac)
    private static void ToggleActive()
    {
        foreach (GameObject obj in Selection.gameObjects)
        {
            obj.SetActive(!obj.activeSelf);
        }
    }
}

