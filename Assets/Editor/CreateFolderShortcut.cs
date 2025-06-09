using UnityEditor;
using UnityEngine;
using System.IO;

public class CreateFolderShortcut
{
    [MenuItem("Assets/Crear Nueva Carpeta %&n")] // Ctrl+Alt+N
    private static void CreateNewFolder()
    {
        string basePath = GetSelectedPathOrFallback();
        string newFolderName = "Nueva Carpeta";
        string fullPath = Path.Combine(basePath, newFolderName);

        // Asegura que sea único (Nueva Carpeta, Nueva Carpeta 1, etc.)
        fullPath = AssetDatabase.GenerateUniqueAssetPath(fullPath);

        // Crea la carpeta físicamente
        Directory.CreateDirectory(fullPath);
        AssetDatabase.Refresh();

        // Selecciona y permite renombrar
        EditorApplication.delayCall += () =>
        {
            Object folder = AssetDatabase.LoadAssetAtPath<Object>(fullPath);
            if (folder != null)
            {
                Selection.activeObject = folder;
                ProjectWindowUtil.ShowCreatedAsset(folder);
            }
        };
    }

    private static string GetSelectedPathOrFallback()
    {
        string path = "Assets";
        foreach (Object obj in Selection.GetFiltered(typeof(Object), SelectionMode.Assets))
        {
            path = AssetDatabase.GetAssetPath(obj);
            if (!string.IsNullOrEmpty(path) && File.Exists(path))
                path = Path.GetDirectoryName(path);
            break;
        }
        return path;
    }
}
