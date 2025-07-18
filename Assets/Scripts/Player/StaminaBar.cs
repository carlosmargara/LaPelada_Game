using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    //Eventos que se comunican con el AudioManager para lanzar SoundStamina
    public static System.Action OnStaminaDepleted;
    public static System.Action OnStaminaRecovered;

    [SerializeField] private GameObject uiPlayerPanel;

    [SerializeField] private Image staminaBar;

    [Header("Otros paneles que ocultan el UIPlayer")]
    [SerializeField] private List<GameObject> panelsThatHideUIPlayer;

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
        //Chequear si algún otro panel está activo
        if (IsAnyOtherPanelActive())
        {
            if (uiPlayerPanel.activeSelf)
                uiPlayerPanel.SetActive(false);
        }
        else
        {
            HandleStaminaUI();
        }

        HandleStamina();
        UpdateStaminaBar();
    }

    private bool IsAnyOtherPanelActive()
    {
        foreach (var panel in panelsThatHideUIPlayer)
        {
            if (panel != null && panel.activeSelf)
                return true;
        }
        return false;
    }


    private void HandleStaminaUI()
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
            // Opci�n A: mientras siga presionando Shift, aunque no corra, se reinicia el delay
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

        // Disparar eventos si el estado cambi�
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
