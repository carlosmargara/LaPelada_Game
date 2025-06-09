using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class LoadingScene : MonoBehaviour
{
    public string sceneToLoad;
    public TextMeshProUGUI loadingText; // Texto pixelado que parpadea

    void Start()
    {
        sceneToLoad = SceneLoader.nextScene;
        StartCoroutine(LoadSceneAsync());
        StartCoroutine(BlinkText());
    }

    IEnumerator LoadSceneAsync()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneToLoad);
        asyncLoad.allowSceneActivation = false;

        // Simular un mínimo de tiempo de espera si querés un efecto más largo
        yield return new WaitForSeconds(3f);

        while (asyncLoad.progress < 0.9f)
        {
            yield return null;
        }

        // Mostrar un efecto final antes de activar la escena (opcional)
        yield return new WaitForSeconds(0.5f);
        asyncLoad.allowSceneActivation = true;
    }

    IEnumerator BlinkText()
    {
        while (true)
        {
            loadingText.enabled = !loadingText.enabled;
            yield return new WaitForSeconds(0.5f);
        }
    }
}

