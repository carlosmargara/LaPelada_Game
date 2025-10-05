using UnityEngine;
using Cinemachine;
using Unity.VisualScripting;

public class CamTriggerGarage : MonoBehaviour
{
    [Header("Cámaras")]
    public CinemachineVirtualCamera camGarage;

    [Header("Objetos")]
    public GameObject crossHair;

    [Header("Configuración")]
    [Range(0f, 1f)]
    public float chanceToTrigger = 0.5f; // 50% de probabilidad por defecto

    [Header("Control del Player")]
    public PlayerController playerController; // referencia a tu PlayerController refactorizado

    private bool triggered = false;
    //private bool hasTriggeredOnce = false;

    [Header("Pensamientos Player")]
    [SerializeField] private PlayerThoughts sheWatchesYou_Garage;

    // 🔑 Este método que setea el follow y el look at de la virtual camera
    public void SetPlayer(PlayerController player)
    {
        playerController = player;

        if (camGarage != null)
        {
            camGarage.Follow = player.transform;
            camGarage.LookAt = player.transform;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (GameStateManager.Instance.camTriggerGarageTriggered) return;
        if (other.CompareTag("Player"))
        {
            // Asignar referencia si no está puesta en el inspector
            if (playerController == null)
            {
                playerController = other.GetComponent<PlayerController>();
                SetPlayer(playerController);
            }


            if (Random.value <= chanceToTrigger)
            {
                triggered = true;
                GameStateManager.Instance.camTriggerGarageTriggered = true; // <-- SE GUARDA GLOBAL


                // Cambiar a modo tanque
                if (playerController != null)
                    playerController.currentMode = ControlMode.Tank;

                camGarage.Priority = 20;
                crossHair.SetActive(false);

                AudioManager02.Instance.TryFindEmittersIfNull();
                if (AudioManager02.Instance.SheIsLookingAtYou_Emitter != null)
                {
                    AudioManager02.Instance.SheIsLookingAtYou_Emitter.Play();
                }
                else
                {
                    Debug.LogWarning("No se pudo reproducir el sonido: emitter no encontrado.");
                }
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

            camGarage.Priority = 5; // Vuelve a su prioridad normal
            crossHair.SetActive(true);
            triggered = false;

            DialogueManager.Instance.ShowThoughts(sheWatchesYou_Garage);
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

