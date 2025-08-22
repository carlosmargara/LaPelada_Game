using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorChangeScene_keyLess : Interactable
{
    [Header("Configuración de escena")]
    [SerializeField] private string sceneToLoad = "PatioInterno_TheOffice";
    [SerializeField] private string spawnPointName = "EntradaDesdePlaza";

    public override void Interact()
    {
        StartCoroutine(ChangeSceneWithFade());
    }

    private IEnumerator ChangeSceneWithFade()
    {
        // Espera a que termine el fade
        yield return StartCoroutine(FadeManager.Instance.FadeIn());

        // Ahora sí, carga la escena
        SceneLoader.LoadScene(sceneToLoad, spawnPointName);
    }
}
