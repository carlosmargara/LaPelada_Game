using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorChangeScene_withKey : Interactable
{
    [Header("Configuración de escena")]
    [SerializeField] private string sceneToLoad = "PatioInterno_TheOffice";
    [SerializeField] private string spawnPointName = "EntradaDesdePlaza";
    [SerializeField] private Door descripcion; // ScriptableObject con info de la puerta

    public Door Descripcion => descripcion;

    public override void Interact()
    {
        if (!string.IsNullOrEmpty(descripcion.requiredKeyID))
        {
            // Verificar si el jugador tiene la llave en el inventario
            if (Inventory.Instance.HasItem(descripcion.requiredKeyID))
            {

                DialogueManager.Instance.ShowWorldMessage(descripcion.unlockedText); // Mostrar mensaje "Usaste la llave..."
                AudioManager02.Instance.PlayOneShot("event:/Fxs/the key slides into the lock_Sound"); //lanza sonido de desbloquiaste la puerta 
                StartCoroutine(ChangeSceneAfterDialogue()); // Después del cartel, ir a la otra escena
            }
            else
            {
                DialogueManager.Instance.ShowWorldMessage(descripcion.lockedText);// Mostrar mensaje "Está cerrada..."
                AudioManager02.Instance.PlayOneShot("event:/Fxs/Closed_Door");// lanza sonido de puerta cerrada
            }
        }
        else
        {
            // No requiere llave → directamente ir a la escena
            StartCoroutine(ChangeSceneWithFade());
        }
    }

    private IEnumerator ChangeSceneWithFade()
    {
        yield return StartCoroutine(FadeManager.Instance.FadeIn());
        SceneLoader.LoadScene(sceneToLoad, spawnPointName);
    }

    private IEnumerator ChangeSceneAfterDialogue()
    {
        // Esperar a que el jugador cierre el cartel
        while (DialogueManager.Instance.IsTalking)
            yield return null;

        AudioManager02.Instance.PlayOneShot("event:/Fxs/OpenDoor_withKey");

        yield return StartCoroutine(FadeManager.Instance.FadeIn());
        SceneLoader.LoadScene(sceneToLoad, spawnPointName);
    }
}
