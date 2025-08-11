using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class LoadingScene : MonoBehaviour
{
    public TextMeshProUGUI loadingText;

    void Start()
    {
        StartCoroutine(LoadSceneAsync());
        //StartCoroutine(BlinkText());
    }

    IEnumerator LoadSceneAsync()
    {
        // Empezamos la carga de la escena que guardó SceneLoader
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(SceneLoader.nextScene); //aca se carga la escena a la que voy
        asyncLoad.allowSceneActivation = false;

        // Espera mínima (tu efecto)
        yield return new WaitForSeconds(3f);

        // Esperamos a que esté listo
        while (asyncLoad.progress < 0.9f)
            yield return null;

        // Breve efecto final
        yield return new WaitForSeconds(0.5f);

        // Activamos la escena destino
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

