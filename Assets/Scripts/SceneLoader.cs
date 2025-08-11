using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneLoader
{
    public static string nextScene;
    public static string nextSpawnPoint;

    /// <summary>
    /// Llama a la pantalla de carga y guarda escena + spawn.
    /// </summary>
    public static void LoadScene(string targetScene, string spawnPointName = null)
    {
        nextScene = targetScene;
        nextSpawnPoint = spawnPointName;
        SceneManager.LoadScene("ScreenLoading");
    }

    /// <summary>
    /// Llamar desde ScreenLoading para continuar a la siguiente escena.
    /// </summary>
    public static void ContinueToNextScene()
    {
        SceneManager.LoadScene(nextScene);
    }
}
