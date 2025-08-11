using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorChangeScene : Interactable
{
    [Header("Configuración de escena")]
    [SerializeField] private string sceneToLoad = "PatioInterno_TheOffice";
    [SerializeField] private string spawnPointName = "EntradaDesdePlaza";

    public override void Interact()
    {
        SceneLoader.LoadScene(sceneToLoad, spawnPointName);
    }
}
