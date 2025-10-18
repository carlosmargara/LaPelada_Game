using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerInteraction : MonoBehaviour
{
    private CrosshairController crosshairController;
    private Flashlight flashlightScript;
    private PauseManager pauseManager;

    private float blockInteractTimer;

    private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindCrosshair();
        flashlightScript = GetComponent<Flashlight>();
        pauseManager = FindObjectOfType<PauseManager>(true);
    }

    private void Start()
    {
        FindCrosshair();
        flashlightScript = GetComponent<Flashlight>();
        pauseManager = FindObjectOfType<PauseManager>(true);
    }

    private void Update()
    {
        if (blockInteractTimer > 0)
            blockInteractTimer -= Time.deltaTime;
    }

    public void BlockInteractFor(float duration)
    {
        blockInteractTimer = duration;
    }

    private void FindCrosshair()
    {
        crosshairController = FindObjectOfType<CrosshairController>();
        if (crosshairController == null)
            Debug.LogWarning("No se encontró CrosshairController en la escena " + SceneManager.GetActiveScene().name);

    }

    // --- Interactuar con objetos ---
    public void OnInteract(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (context.performed && crosshairController != null)
        {
            crosshairController.TryInteract();
        }
    }

    public void OnInventory(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        if (InventoryUI.Instance != null)
        {
            InventoryUI.Instance.ToggleInventory();
        }
    }

    public void OnFlashlight(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        if (flashlightScript != null)
            flashlightScript.ToggleFlashlight();
        else
            Debug.LogWarning("No se encontró el script Flashlight en el jugador.");
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        if (pauseManager == null)
        {
            pauseManager = FindObjectOfType<PauseManager>(true);
            if (pauseManager == null)
            {
                Debug.LogWarning("No se encontró PauseManager en la escena.");
                return;
            }
        }

        pauseManager.TogglePause();
    }
}




