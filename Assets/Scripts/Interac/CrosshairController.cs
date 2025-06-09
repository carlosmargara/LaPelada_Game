using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrosshairController : MonoBehaviour
{
    [SerializeField] private Image crosshairImage;
    [SerializeField] private float rayDistance = 3f;
    private Color defaultColor = Color.gray;
    private Color32 interactColor = new Color32(163,3,3,255); //Color RojoOscuro
    [SerializeField] private LayerMask interactableLayer;

    private Camera cam;
    private Interactable currentInteractable; // guarda el objeto que tiene el intetacion

    void Start()
    {
        cam = Camera.main;
        if (crosshairImage != null)
            crosshairImage.color = defaultColor;
    }

    void Update()
    {
        if (!InventoryUI.Instance.IsInventoryOpen)
        {
            CheckForInteractable();
        }
        else
        {
            Debug.Log("Inventario abierto, no se lanza el raycast.");
        }


        // Comprobamos el input aquí
        if (Input.GetMouseButtonDown(0) && currentInteractable != null && !DialogueManager.Instance.IsTalking
            && !NoteManager.Instance.isDescribing && !PickupUIManager.Instance.firtTextWasShown && !InventoryUI.Instance.IsInventoryOpen)
        {
            currentInteractable.Interact();
            currentInteractable = null; // Previene múltiples interacciones con el mismo objeto si sigue en pantalla
        }
    }

    void CheckForInteractable()
    {
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        RaycastHit hit;

        Debug.DrawRay(ray.origin, ray.direction * rayDistance, Color.green);

        if (Physics.Raycast(ray, out hit, rayDistance, interactableLayer))
        {
            Interactable interactable = hit.collider.GetComponent<Interactable>();
            if (interactable != null)
            {
                SetCrosshairColor(interactColor);
                currentInteractable = interactable;
                return;
            }
        }

        SetCrosshairColor(defaultColor);
        currentInteractable = null;
    }

    void SetCrosshairColor(Color color)
    {
        if (crosshairImage != null)
            crosshairImage.color = color;
    }
}
