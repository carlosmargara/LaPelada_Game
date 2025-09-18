using UnityEngine;
using Cinemachine;

public class CamTriggerZone : MonoBehaviour
{
    [Header("Cámaras")]
    public CinemachineVirtualCamera camFija;

    [Header("Objetos")]
    public GameObject laPelada; // Referencia a La Pelada para activar/desactivar
    public GameObject crossHair;

    [Header("Configuración")]
    [Range(0f, 1f)]
    public float chanceToTrigger = 0.5f; // 50% de probabilidad

    [Header("Control del Player")]
    public PlayerController playerController; // referencia a tu PlayerController refactorizado

    private bool triggered = false; // Para saber si esta vez se activó la cámara

    void Start()
    {
        laPelada.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Asignar referencia si no está puesta en el inspector
            if (playerController == null)
                playerController = other.GetComponent<PlayerController>();

            if (Random.value <= chanceToTrigger)
            {
                triggered = true;

                // Cambiar a modo tanque
                if (playerController != null)
                    playerController.currentMode = ControlMode.Tank;

                camFija.Priority = 20;
                laPelada?.SetActive(true);
                crossHair.SetActive(false);

                AudioManager02.Instance.TryFindEmittersIfNull();
                if (AudioManager02.Instance.SheIsLookingAtYou_Emitter != null)
                    AudioManager02.Instance.SheIsLookingAtYou_Emitter.Play();
            }
            else
            {
                triggered = false;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && triggered)
        {
            // Volver a primera persona
            if (playerController != null)
                playerController.currentMode = ControlMode.FirstPerson;

            camFija.Priority = 5;
            laPelada?.SetActive(false);
            crossHair.SetActive(true);
            triggered = false;
        }
    }

    private void OnDrawGizmos()
    {
        BoxCollider box = GetComponent<BoxCollider>();
        if (box == null) return;

        Gizmos.color = Color.green;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(box.center, box.size);
    }
}


