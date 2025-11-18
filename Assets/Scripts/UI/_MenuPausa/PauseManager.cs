using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject pausePanel;

    [Header("Post Processing")]
    [SerializeField] private Volume globalVolume;
    private ColorAdjustments colorAdjustments;

    [Header("Brillo")]
    [SerializeField] private Slider brightnessSlider;
    [SerializeField] private float minExposure = 0.5f;
    [SerializeField] private float maxExposure = 3.5f;
    [SerializeField] private float defaultExposure = 1.9f;
    private float currentExposure;

    [Header("Sensibilidad del mouse")]
    [SerializeField] private Slider sensitivitySlider;
    [SerializeField] private float minSensitivity = 0.1f;
    [SerializeField] private float maxSensitivity = 5f;
    private float currentSensitivity;

    [Header("Audio")]
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private float minVolume = 0f;
    [SerializeField] private float maxVolume = 1f;

    [Header("Input")]
    [SerializeField] private PlayerInput playerInput;
    private InputMapController inputMapController;
    private PlayerController player;

    private bool isPaused = false;

    private void Awake()
    {
        inputMapController = FindObjectOfType<InputMapController>();
        player = FindObjectOfType<PlayerController>();
    }

    private void Start()
    {
        // 🔸 BRILLO
        if (globalVolume != null && globalVolume.profile.TryGet(out colorAdjustments))
        {
            BrightnessManager.Instance.ApplyToVolume(globalVolume);

            float normalizedValue = Mathf.InverseLerp(
                minExposure,
                maxExposure,
                BrightnessManager.Instance.CurrentExposure
            );
            if (brightnessSlider != null)
                brightnessSlider.value = normalizedValue;
        }

        // 🔸 SENSIBILIDAD
        if (sensitivitySlider != null)
        {
            float normalizedValue = Mathf.InverseLerp(
                minSensitivity,
                maxSensitivity,
                MouseSensitivityManager.Instance.CurrentSensitivity
            );
            sensitivitySlider.value = normalizedValue;
        }

        // 🔸 AUDIO (volumen general)
        if (volumeSlider != null)
        {
            float normalizedValue = Mathf.InverseLerp(
                minVolume,
                maxVolume,
                AudioSettingsManager.Instance.CurrentVolume
            );
            volumeSlider.value = normalizedValue;
        }
    }

    public void SetBrightness(float sliderValue)
    {
        if (colorAdjustments != null)
        {
            currentExposure = Mathf.Lerp(minExposure, maxExposure, sliderValue);
            colorAdjustments.postExposure.value = currentExposure;
            BrightnessManager.Instance.SetExposure(currentExposure);
        }
    }

    public void SetMouseSensitivity(float sliderValue)
    {
        currentSensitivity = Mathf.Lerp(minSensitivity, maxSensitivity, sliderValue);
        MouseSensitivityManager.Instance.SetSensitivity(currentSensitivity);

        if (player != null)
            player.SetMouseSensitivity(currentSensitivity);
    }

    public void SetMasterVolume(float sliderValue)
    {
        float targetVolume = Mathf.Lerp(minVolume, maxVolume, sliderValue);
        AudioSettingsManager.Instance.SetVolume(targetVolume);
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        pausePanel.SetActive(isPaused);
        Time.timeScale = isPaused ? 0f : 1f;

        if (inputMapController != null)
        {
            if (isPaused)
            {
                inputMapController.SwitchToUI();
                GameStateManager.Instance.LockPlayer(20);
            }
            else
            {
                inputMapController.SwitchToPlayer();
                GameStateManager.Instance.UnlockPlayer(20);
            }
        }

        // 🔄 Sincroniza sliders al abrir el menú
        if (isPaused)
        {
            // Brillo
            if (brightnessSlider != null && colorAdjustments != null)
            {
                float normalizedValue = Mathf.InverseLerp(
                    minExposure,
                    maxExposure,
                    BrightnessManager.Instance.CurrentExposure
                );
                brightnessSlider.value = normalizedValue;
            }

            // Sensibilidad
            if (sensitivitySlider != null)
            {
                float normalizedValue = Mathf.InverseLerp(
                    minSensitivity,
                    maxSensitivity,
                    MouseSensitivityManager.Instance.CurrentSensitivity
                );
                sensitivitySlider.value = normalizedValue;
            }

            // Volumen
            if (volumeSlider != null)
            {
                float normalizedValue = Mathf.InverseLerp(
                    minVolume,
                    maxVolume,
                    AudioSettingsManager.Instance.CurrentVolume
                );
                volumeSlider.value = normalizedValue;
            }
        }
    }

    public void Button_ExitGame()
    {
        Debug.Log("Cerrando el juego...");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }
}



