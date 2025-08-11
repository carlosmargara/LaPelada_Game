using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkingLight : MonoBehaviour
{
    public float minIntensity = 0.5f; // Intensidad mínima de la luz
    public float maxIntensity = 1.5f; // Intensidad máxima de la luz
    public float blinkSpeed = 1.0f; // Velocidad de parpadeo

    private Light pointLight;

    void Start()
    {
        pointLight = GetComponent<Light>();

        StartCoroutine(Blink());

    }

    
    IEnumerator Blink()
    {
        while (true)
        {
            // Cambia la intensidad de la luz entre los valores mínimo y máximo
            pointLight.intensity = Mathf.Lerp(minIntensity, maxIntensity, Mathf.PingPong(Time.time * blinkSpeed, 1.0f));
            yield return null;
        }
    }
}
