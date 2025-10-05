using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThoughtsPlayer_SaveRoom : MonoBehaviour
{
    [SerializeField] private PlayerThoughts saveRoom;
    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player")) return;

        StartCoroutine(Luanch_ThoughtsSaveRoom());
        triggered = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            AudioManager02.Instance.FadeOutAndStopMusic(10f);
    }

    private IEnumerator Luanch_ThoughtsSaveRoom()
    {
        yield return new WaitForSeconds(5f);
        DialogueManager.Instance.ShowThoughts(saveRoom);

        Invoke(nameof(EndThoughtPlayer), 5f);
    }

    private void OnDrawGizmos()
    {
        BoxCollider box = GetComponent<BoxCollider>();
        if (box == null) return;

        Gizmos.color = Color.green;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(box.center, box.size);
    }

    private void EndThoughtPlayer()
    {
        Debug.Log("Termino el pensamineto del jugador ");
        AudioManager02.Instance.PlayMusic("event:/Music/SaveRoom");
    }
}
