using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class BrightnessManager : MonoBehaviour
{
    public static BrightnessManager Instance { get; private set; }

    public float CurrentExposure { get; private set; } = 1.9f; // valor inicial por defecto

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetExposure(float exposure)
    {
        CurrentExposure = exposure;
    }

    public void ApplyToVolume(Volume volume)
    {
        if (volume != null && volume.profile.TryGet(out ColorAdjustments colorAdj))
        {
            colorAdj.postExposure.value = CurrentExposure;
        }
    }
}

