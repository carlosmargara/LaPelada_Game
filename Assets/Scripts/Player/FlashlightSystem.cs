using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashlightSystem : Singleton<FlashlightSystem>
{
    [SerializeField] private GameObject flashlightObject;

    private bool isEquipped = false;

    public void SetEquipped(bool value)
    {
        isEquipped = value;

        if (!value)
            flashlightObject.SetActive(false); // Asegura que se apague si se desequipa
    }

    public bool IsEquipped => isEquipped;

    public void Toggle()
    {
        if (!isEquipped) return;

        flashlightObject.SetActive(!flashlightObject.activeSelf);
    }
}