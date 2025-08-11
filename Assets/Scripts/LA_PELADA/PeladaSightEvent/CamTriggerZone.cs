using UnityEngine;
using Cinemachine;


public class CamTriggerZone : MonoBehaviour
{
    public CinemachineVirtualCamera camFija;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            camFija.Priority = 20;

            // Buscar el emitter si no está asignado todavía
            AudioManager02.Instance.TryFindEmittersIfNull();

            // Reproducir solo si lo encontró
            if (AudioManager02.Instance.SheIsLookingAtYou_Emitter != null)
            {
                AudioManager02.Instance.SheIsLookingAtYou_Emitter.Play();
            }
            else
            {
                Debug.LogWarning("No se pudo reproducir el sonido: emitter no encontrado.");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            camFija.Priority = 5; // Vuelve a su prioridad normal
            //AudioManager.Instance.studioEventEmitter.Stop();
        }
    }
}

