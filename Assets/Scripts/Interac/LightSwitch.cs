using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSwitch : Interactable
{
    [Header("Configuración de la luz")]
    [SerializeField] private Light targetLight;
    [SerializeField] private Light targetLight02;
    [SerializeField] private bool startOn = false;

    private bool isOn;

    private void Start()
    {
        isOn = startOn;
        if (targetLight != null && targetLight02 != null)
        {
            targetLight.enabled = isOn;
            targetLight02.enabled = isOn;
        }
    }

    public override void Interact()
    {
        ToggleLight();
    }

    private void ToggleLight()
    {
        isOn = !isOn;
        if (targetLight != null && targetLight02 != null)
        {
            targetLight.enabled = isOn;
            targetLight02.enabled = isOn;
        }
        AudioManager02.Instance.PlayOneShot("event:/Fxs/Click-on_off_Lightwall_Sound");
    }
}
