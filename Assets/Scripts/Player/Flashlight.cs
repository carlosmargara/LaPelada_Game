using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashlight : MonoBehaviour
{
    [SerializeField] private GameObject flashlight;
    //[SerializeField] private AudioClip lightClick_Sound;

    private void Start()
    {
        flashlight.SetActive(false);
    }

    public void ToggleFlashlight()
    {
        if (FlashlightSystem.Instance.IsEquipped)
        {
            FlashlightSystem.Instance.Toggle();
            AudioManager02.Instance.PlayOneShot("event:/Fxs/FlashLightFX");
        }
    }
}
