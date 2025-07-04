using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    //Eventos que se comunican con el AudioManager 
    public static System.Action OnStaminaDepleted;
    public static System.Action OnStaminaRecovered;

    [SerializeField] private GameObject uiPlayerPanel;
    
    [SerializeField] private Image staminaBar;

    private float maxStamina = 100f;
    [SerializeField] private float staminaRegenRate = 10f;
    [SerializeField] private float staminaDrainRate = 20f;
    [SerializeField] private float regenDelay = 2f;

    public float CurrentStamina { get; private set; }
    public bool IsExhausted => CurrentStamina <= 0.1f;
    public bool IsRecovered => CurrentStamina >= maxStamina;

    //public float currentStamina;
    private float regenTimer;
    private bool wasRunningLastFrame;

    //public bool CanSprint => currentStamina > 0;

    private PlayerController playerController;

    void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
        CurrentStamina = maxStamina;
        regenTimer = 0f;
    }

    void Update()
    {
        bool shiftPressed = (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift));
        if (playerController.Move.magnitude > 0.1 && shiftPressed)
        {
            uiPlayerPanel.SetActive(true);
        }
        else if (CurrentStamina == maxStamina)
        {
            uiPlayerPanel.SetActive(false);
        }

        HandleStamina();
        UpdateStaminaBar();           
    }

    private void HandleStamina()
    {
        bool wasExhausted = IsExhausted;

        if (playerController.isRunning && CurrentStamina > 0)
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

        if (CurrentStamina <= 0)
        {
            playerController.isRunning = false;
        }

        wasRunningLastFrame = playerController.isRunning;

        // Disparar eventos si el estado cambió
        if (IsExhausted && !wasExhausted)
            OnStaminaDepleted?.Invoke();
        else if (IsRecovered)
        {
            Debug.Log("EVENTO: Stamina recuperada al 100%");
            OnStaminaRecovered?.Invoke();
        }
    }

    void DrainStamina()
    {
        if (playerController.Move.magnitude > 0.1)
        {
            CurrentStamina -= staminaDrainRate * Time.deltaTime;
            CurrentStamina = Mathf.Clamp(CurrentStamina, 0, maxStamina);
        }
    }

    void RegenerateStamina()
    {
        CurrentStamina += staminaRegenRate * Time.deltaTime;
        CurrentStamina = Mathf.Clamp(CurrentStamina, 0, maxStamina);
    }

    void UpdateStaminaBar()
    {
        staminaBar.fillAmount = CurrentStamina / maxStamina;
    }
}
