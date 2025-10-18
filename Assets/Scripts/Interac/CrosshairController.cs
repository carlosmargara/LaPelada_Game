using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CrosshairController : MonoBehaviour
{
    [SerializeField] private Image crosshairImage;
    [SerializeField] private float rayDistance = 3f;
    private Color defaultColor = Color.gray;
    private Color32 interactColor = new Color32(163, 3, 3, 255); // Color RojoOscuro
    [SerializeField] private LayerMask interactableLayer;

    [SerializeField] private float interactCooldown = 0.2f;
    private float interactTimer = 0f;

    private Camera cam;
    private Interactable currentInteractable;

    // Animación
    [SerializeField] private float fadeDuration = 0.2f;
    [SerializeField] private Vector3 activeScale = Vector3.one;
    [SerializeField] private Vector3 hiddenScale = Vector3.zero;
    private Coroutine fadeCoroutine;

    void Start()
    {
        cam = Camera.main;
        if (crosshairImage != null)
        {
            crosshairImage.color = defaultColor;
            crosshairImage.transform.localScale = hiddenScale; // empieza escondido
            SetCrosshairVisible(true); // aparece suave al inicio
        }
    }

    void Update()
    {
        // Cooldown
        if (interactTimer > 0)
            interactTimer -= Time.deltaTime;


        // --- Bloquear raycast si hay paneles abiertos ---
        if (!IsAnyPanelOpen())
        {
            SetCrosshairVisible(true);
            CheckForInteractable();
        }
        else
        {
            SetCrosshairVisible(false);
            currentInteractable = null;
            return;
        }
    }

    public void TryInteract()
    {
        if (currentInteractable != null && interactTimer <= 0f && !IsAnyPanelOpen())
        {
            // cheque extra antes de llamar
            if (!DialogueManager.Instance.IsTalking)
            {
                Debug.Log("------ INTERACTUO CON EL OBJETO -------");
                currentInteractable.Interact();
                currentInteractable = null;
                interactTimer = interactCooldown;
            }
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

    bool IsAnyPanelOpen()
    {
        return DialogueManager.Instance.IsTalking
            || NoteManager.Instance.isDescribing
            || PickupUIManager.Instance.firtTextWasShown
            || InventoryUI.Instance.IsInventoryOpen;
    }

    // --- Animación crosshair ---
    void SetCrosshairVisible(bool visible)
    {
        if (crosshairImage == null) return;

        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeCrosshair(visible));
    }

    IEnumerator FadeCrosshair(bool show)
    {
        float elapsed = 0f;
        Color startColor = crosshairImage.color;
        Color endColor = startColor;
        endColor.a = show ? 1f : 0f;

        Vector3 startScale = crosshairImage.transform.localScale;
        Vector3 endScale = show ? activeScale : hiddenScale;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeDuration;

            crosshairImage.color = Color.Lerp(startColor, endColor, t);
            crosshairImage.transform.localScale = Vector3.Lerp(startScale, endScale, t);

            yield return null;
        }

        crosshairImage.color = endColor;
        crosshairImage.transform.localScale = endScale;
    }
}
