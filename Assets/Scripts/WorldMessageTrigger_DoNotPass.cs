using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMessageTrigger_DoNotPass : MonoBehaviour
{
    [TextArea]
    [SerializeField] private string message;
    [SerializeField] private float cooldownTime = 3f;

    private bool canShowMessage = true;

    private void OnTriggerEnter(Collider other)
    {
        if (!canShowMessage) return;

        if (other.CompareTag("Player"))
        {
            DialogueManager.Instance.ShowWorldMessage(message);
            StartCoroutine(CooldownCoroutine());
        }
    }

    private System.Collections.IEnumerator CooldownCoroutine()
    {
        canShowMessage = false;
        yield return new WaitForSeconds(cooldownTime);
        canShowMessage = true;
    }

    private void OnDrawGizmos()
    {
        BoxCollider box = GetComponent<BoxCollider>();
        if (box == null) return;

        Gizmos.color = Color.red;

        // Aplicar la matriz del transform actual
        Gizmos.matrix = transform.localToWorldMatrix;

        // Dibujar el cubo en el espacio local del objeto
        Gizmos.DrawWireCube(box.center, box.size);
    }
}
