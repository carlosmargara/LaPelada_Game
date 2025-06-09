using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VignettePulse : MonoBehaviour
{
    public Image vignetteImage;         // Asegurate que tenga un componente Image
    public float pulseSpeed = 2f;       // Velocidad del parpadeo
    public float alphaAmount = 0.2f;    // Cuánto cambia el alpha (máximo +/- sobre el original)

    private float originalAlpha;

    void Start()
    {
        if (vignetteImage == null) vignetteImage = GetComponent<Image>();

        originalAlpha = vignetteImage.color.a;
    }

    void Update()
    {
        float alpha = originalAlpha + Mathf.Sin(Time.time * pulseSpeed) * alphaAmount;
        alpha = Mathf.Clamp01(alpha); // Asegura que el alpha esté entre 0 y 1

        Color color = vignetteImage.color;
        color.a = alpha;
        vignetteImage.color = color;
    }
}
