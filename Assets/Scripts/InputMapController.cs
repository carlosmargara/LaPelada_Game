using UnityEngine;
using UnityEngine.InputSystem;

public class InputMapController : MonoBehaviour
{
    private PlayerInput playerInput;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    public void SwitchToUI()
    {
        if (playerInput.currentActionMap.name == "UI") return;
        playerInput.SwitchCurrentActionMap("UI");
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void SwitchToPlayer()
    {
        if (playerInput.currentActionMap.name == "Player") return;
        playerInput.SwitchCurrentActionMap("Player");
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}

