using System.Collections;
using UnityEngine;

public class DoorTeleport_keyLess : Interactable
{
    [Header("Configuración de teletransporte")]
    [SerializeField] private Transform destinationPoint; // Punto de llegada
    [SerializeField] private Door descripcion; // ScriptableObject con info de la puerta

    public Door Descripcion => descripcion;

    [Header("Configuración de puerta")]
    [SerializeField] private typoDoor doorType;

    public override void Interact()
    {
        if (!string.IsNullOrEmpty(descripcion.requiredKeyID))
        {
            // Requiere llave
            if (Inventory.Instance.HasItem(descripcion.requiredKeyID))
            {
                DialogueManager.Instance.ShowWorldMessage(descripcion.unlockedText);
                AudioManager02.Instance.PlayOneShot("event:/Fxs/the key slides into the lock_Sound");
                StartCoroutine(TeleportAfterDialogue());
            }
            else
            {
                DialogueManager.Instance.ShowWorldMessage(descripcion.lockedText);
                AudioManager02.Instance.PlayOneShot("event:/Fxs/Closed_Door");
            }
        }
        else
        {
            // No requiere llave → directo
            StartCoroutine(TeleportWithFade());
        }
    }

    private IEnumerator TeleportWithFade()
    {
        // Sonido según tipo de puerta
        switch (doorType)
        {
            case typoDoor.metal:
                AudioManager02.Instance.PlayOneShot("event:/Fxs/OpenDoor_withKey");
                break;
            case typoDoor.wood:
                AudioManager02.Instance.PlayOneShot("event:/Fxs/OpenWoodDoor");
                break;
        }

        yield return StartCoroutine(FadeManager.Instance.FadeIn());

        // Mover player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null && destinationPoint != null)
        {
            player.transform.position = destinationPoint.position;
            player.transform.rotation = destinationPoint.rotation;
        }

        yield return StartCoroutine(FadeManager.Instance.FadeOut());
    }

    private IEnumerator TeleportAfterDialogue()
    {
        // Esperar a que cierre el cartel
        while (DialogueManager.Instance.IsTalking)
            yield return null;

        // Sonido según tipo de puerta
        switch (doorType)
        {
            case typoDoor.metal:
                AudioManager02.Instance.PlayOneShot("event:/Fxs/OpenDoor_withKey");
                break;
            case typoDoor.wood:
                AudioManager02.Instance.PlayOneShot("event:/Fxs/OpenWoodDoor");
                break;
        }

        yield return StartCoroutine(FadeManager.Instance.FadeIn());

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null && destinationPoint != null)
        {
            player.transform.position = destinationPoint.position;
            player.transform.rotation = destinationPoint.rotation;
        }

        yield return StartCoroutine(FadeManager.Instance.FadeOut());
    }
}

