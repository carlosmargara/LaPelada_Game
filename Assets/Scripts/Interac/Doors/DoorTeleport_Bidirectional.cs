using System.Collections;
using UnityEngine;

public class DoorTeleport_Bidirectional : Interactable
{
    [Header("Configuración de teletransporte")]
    [SerializeField] private Transform destinationPointA;
    [SerializeField] private Transform destinationPointB;
    [SerializeField] private Door descripcion; // ScriptableObject con info de la puerta

    public Door Descripcion => descripcion;

    [Header("Configuración de puerta")]
    [SerializeField] private typoDoor doorType;

    private bool isTeleporting = false;

    public override void Interact()
    {
        if (isTeleporting) return;

        if (!string.IsNullOrEmpty(descripcion.requiredKeyID))
        {
            // Requiere llave
            if (Inventory.Instance.HasItem(descripcion.requiredKeyID))
            {
                DialogueManager.Instance.ShowWorldMessage(descripcion.GetUnlockedText());
                AudioManager02.Instance.PlayOneShot("event:/Fxs/the key slides into the lock_Sound");
                StartCoroutine(TeleportAfterDialogue());
            }
            else
            {
                DialogueManager.Instance.ShowWorldMessage(descripcion.GetLockedText());
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
        isTeleporting = true;

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

        TeleportPlayer();

        yield return StartCoroutine(FadeManager.Instance.FadeOut());

        isTeleporting = false;
    }

    private IEnumerator TeleportAfterDialogue()
    {
        isTeleporting = true;

        while (DialogueManager.Instance.IsTalking)
            yield return null;

        AudioManager02.Instance.PlayOneShot("event:/Fxs/OpenDoor_withKey");

        yield return StartCoroutine(FadeManager.Instance.FadeIn());

        TeleportPlayer();

        yield return StartCoroutine(FadeManager.Instance.FadeOut());

        isTeleporting = false;
    }

    private void TeleportPlayer()
    {
        if (descripcion == null || string.IsNullOrEmpty(descripcion.ID))
        {
            Debug.LogWarning("[DoorTeleport] DoorID no asignado en " + gameObject.name);
            return;
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        // Recuperar estado actual
        bool isAtA = GameStateManager.Instance.GetDoorState(descripcion.ID, true);

        if (isAtA && destinationPointB != null)
        {
            player.transform.SetPositionAndRotation(destinationPointB.position, destinationPointB.rotation);
            GameStateManager.Instance.SaveDoorState(descripcion.ID, false);
        }
        else if (!isAtA && destinationPointA != null)
        {
            player.transform.SetPositionAndRotation(destinationPointA.position, destinationPointA.rotation);
            GameStateManager.Instance.SaveDoorState(descripcion.ID, true);
        }
    }
}


