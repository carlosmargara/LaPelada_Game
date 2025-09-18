using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum typoDoor
{
    metal,
    wood
}

public class DoorChangeScene_keyLess : Interactable
{
    [Header("Configuración de escena")]
    [SerializeField] private string sceneToLoad = "PatioInterno_TheOffice";
    [SerializeField] private string spawnPointName = "EntradaDesdePlaza";

    [Header("Configuración de puerta")]
    [SerializeField] private typoDoor doorType; //siempre que hago un enum despues tengo que crear la variable para poder usarlo
                                                //y si la variable es "public o SerializeField" la voy a ver en el inspector


    public override void Interact()
    {
        StartCoroutine(ChangeSceneWithFade());
    }

    private IEnumerator ChangeSceneWithFade()
    {
        // 🔊 Sonido según tipo de puerta
        switch (doorType)
        {
            case typoDoor.metal:
                Debug.Log("Sonido de puerta metálica");
                AudioManager02.Instance.PlayOneShot("event:/Fxs/OpenDoor_withKey");
                break;

            case typoDoor.wood:
                Debug.Log("Sonido de puerta de madera");
                // AudioManager.Instance.Play("WoodDoor");
                break;
        }

        yield return StartCoroutine(FadeManager.Instance.FadeIn());// Espera a que termine el fade
        SceneLoader.LoadScene(sceneToLoad, spawnPointName);// Ahora sí, carga la escena
    }
}
