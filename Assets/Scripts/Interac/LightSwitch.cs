using UnityEngine;
using FMODUnity;

public class LightSwitch : Interactable
{
    [Header("Configuración de luces")]
    [SerializeField] private Light luzFija;
    [SerializeField] private BlinkingLight_con_patrón_irregular luzParpadeo; // el script, no el Light

    [SerializeField] private bool startOn = false;

    private bool isOn;

    FMOD.Studio.PLAYBACK_STATE state;

    private void Start()
    {
        isOn = startOn;
        // Luz fija
        if (luzFija != null) luzFija.enabled = isOn;
        // Parpadeo (esto prende/apaga luz + sonido)
        if (luzParpadeo != null) luzParpadeo.enabled = isOn;
    }

    public override void Interact()
    {
        Toggle();
    }

    private void Toggle()
    {
        isOn = !isOn;

        if (luzFija != null) luzFija.enabled = isOn;
        if (luzParpadeo != null) luzParpadeo.enabled = isOn;

        AudioManager02.Instance.PlayOneShot("event:/Fxs/Click-on_off_Lightwall_Sound");
        /*
        if (state == FMOD.Studio.PLAYBACK_STATE.PLAYING)
        {
            AudioManager02.Instance.Meeting_with_PELADA.getPlaybackState(out state);
            AudioManager02.Instance.Meeting_with_PELADA.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
        */
    }
}

