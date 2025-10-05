using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Npc_TriggerBox : MonoBehaviour
{
    [SerializeField] private NPC_Intetaction npcInteraction; // ScriptableObject con los diálogos
    [SerializeField] private Image _npc;
    [SerializeField] private float cooldownTime = 10f; // Tiempo de espera en segundos antes de que vuelva a hablar

    private bool hasTriggered = false;

    private void Start()
    {
        if (_npc != null)
            _npc.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered) return;
        if (!other.CompareTag("Player")) return;

        if (npcInteraction != null)
        {
            _npc.enabled = true;
            DialogueManager.Instance.StartDialogue(npcInteraction);
            AudioManager02.Instance.StopAllFootstepsGlobal();
            hasTriggered = true;

            // Esperar a que termine el diálogo y luego iniciar cooldown
            StartCoroutine(WaitForDialogueEnd());
        }
    }

    private IEnumerator WaitForDialogueEnd()
    {
        // Espera hasta que DialogueManager deje de hablar
        while (DialogueManager.Instance.IsTalking)
        {
            yield return null;
        }

        _npc.enabled = false; // Oculta la imagen

        // Ahora esperamos el cooldown antes de poder volver a hablar
        yield return new WaitForSeconds(cooldownTime);

        hasTriggered = false; // Se puede volver a disparar
    }

    private void OnDrawGizmos()
    {
        BoxCollider box = GetComponent<BoxCollider>();
        if (box == null) return;

        Gizmos.color = Color.blue;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(box.center, box.size);
    }
}


