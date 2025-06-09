using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneLoader
{
    public static string nextScene;

    public static void LoadScene(string targetScene)
    {
        nextScene = targetScene;
        SceneManager.LoadScene("ScreenLoading");
    }
}
