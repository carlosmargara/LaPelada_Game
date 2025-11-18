using UnityEngine;

public class AudioSettingsManager : MonoBehaviour
{
    public static AudioSettingsManager Instance { get; private set; }

    private FMOD.Studio.Bus masterBus;
    private float currentVolume = 1f;
    private bool isMuted = false;

    public float CurrentVolume => currentVolume;
    public bool IsMuted => isMuted;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        masterBus = FMODUnity.RuntimeManager.GetBus("bus:/");
    }

    public void SetVolume(float volume)
    {
        currentVolume = volume;
        masterBus.setVolume(currentVolume);

        // Si el volumen sube de 0, se desmutea
        if (currentVolume > 0 && isMuted)
        {
            isMuted = false;
            masterBus.setMute(false);
        }
    }

    public void ToggleMute()
    {
        isMuted = !isMuted;
        masterBus.setMute(isMuted);
    }
}

