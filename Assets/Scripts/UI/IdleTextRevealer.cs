using UnityEngine;
using UnityEngine.UI; // O usar TMPro si usás TextMeshPro

public class IdleTextRevealer : MonoBehaviour
{
    public GameObject creepyMessage; // Asignalo desde el inspector
    public float idleTimeToShow = 3f; // Segundos sin mover el mouse

    private float idleTimer = 0f;
    private Vector3 lastMousePosition;

    void Start()
    {
        if (creepyMessage != null)
            creepyMessage.SetActive(false); // Ocultar al iniciar

        lastMousePosition = Input.mousePosition;
    }

    void Update()
    {
        // Si el mouse se mueve, reiniciamos el contador
        if (Input.mousePosition != lastMousePosition)
        {
            idleTimer = 0f;
            if (creepyMessage.activeSelf)
                creepyMessage.SetActive(false);
        }
        else
        {
            idleTimer += Time.deltaTime;

            if (idleTimer >= idleTimeToShow && !creepyMessage.activeSelf)
            {
                creepyMessage.SetActive(true);
            }
        }

        lastMousePosition = Input.mousePosition;
    }
}

