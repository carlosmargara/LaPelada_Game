using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    [SerializeField] private GameObject uiPlayerPanel;
    
    [SerializeField] private Image staminaBar;

    private float maxStamina = 100f;
    [SerializeField] private float staminaRegenRate = 10f;
    [SerializeField] private float staminaDrainRate = 20f;
    [SerializeField] private float regenDelay = 2f;

    public float currentStamina;
    private float regenTimer;
    private bool wasRunningLastFrame;

    public bool CanSprint => currentStamina > 0;

    private PlayerController playerController;

    void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
        currentStamina = maxStamina;
        regenTimer = 0f;
    }

    void Update()
    {
        bool shiftPressed = (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift));
        if (playerController.Move.magnitude > 0.1 && shiftPressed)
        {
            uiPlayerPanel.SetActive(true);
        }
        else if (currentStamina == maxStamina)
        {
            uiPlayerPanel.SetActive(false);
        }

        HandleStamina();
        UpdateStaminaBar();
    }

    private void HandleStamina()
    {
        if (playerController.isRunning && currentStamina > 0)
        {
            DrainStamina();
            regenTimer = 0f;
        }
        else
        {
            // Opción A: mientras siga presionando Shift, aunque no corra, se reinicia el delay
            if (wasRunningLastFrame)
            {
                regenTimer = 0f;
            }
            else
            {
                regenTimer += Time.deltaTime;

                if (regenTimer >= regenDelay)
                {
                    RegenerateStamina();
                }
            }
        }

        if (currentStamina <= 0)
        {
            playerController.isRunning = false;
        }

        wasRunningLastFrame = playerController.isRunning;
    }

    void DrainStamina()
    {
        if (playerController.Move.magnitude > 0.1)
        {
            currentStamina -= staminaDrainRate * Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
        }
    }

    void RegenerateStamina()
    {
        currentStamina += staminaRegenRate * Time.deltaTime;
        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
    }

    void UpdateStaminaBar()
    {
        staminaBar.fillAmount = currentStamina / maxStamina;
    }
}
