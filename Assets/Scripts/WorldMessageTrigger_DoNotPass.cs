using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMessageTrigger_DoNotPass : MonoBehaviour
{
    private DiffetentTypes_footSteps_with_FmodEvent footSteps_Player;

    [SerializeField] private LocalizedString message; // 🔥 Ahora es un ScriptableObject con key
    [SerializeField] private float cooldownTime = 3f;

    private bool canShowMessage = true;

    void Start()
    {
        footSteps_Player = FindObjectOfType<DiffetentTypes_footSteps_with_FmodEvent>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!canShowMessage) return;

        if (other.CompareTag("Player"))
        {
            // 🔥 Ahora obtiene el texto localizado desde la key del SO
            DialogueManager.Instance.ShowWorldMessage(message.GetValue());

            StartCoroutine(CooldownCoroutine());
            footSteps_Player.StopAllFootsteps();
        }
    }

    private IEnumerator CooldownCoroutine()
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
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(box.center, box.size);
    }
}
