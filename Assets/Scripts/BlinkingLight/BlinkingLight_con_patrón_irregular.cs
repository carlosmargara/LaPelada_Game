using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class BlinkingLight_con_patrón_irregular : MonoBehaviour
{
    [Header("Intensidad")]
    public float minIntensity = 0.5f;
    public float maxIntensity = 1.5f;

    [Header("Parpadeo base")]
    public float blinkSpeed = 1.0f;      // Velocidad del ruido base
    public float noiseScale = 1.0f;      // Suavidad del ruido base

    [Header("Temblor aleatorio")]
    public float flickerProbability = 0.1f;  // Probabilidad de temblor por frame
    public float flickerAmount = 0.2f;       // Qué tanto se desvía la intensidad base
    public float flickerSpeed = 10f;         // Qué tan rápido vuelve a la base

    private Light pointLight;
    private float randomSeed;
    private float currentIntensity;

    [SerializeField] private FMODUnity.StudioEventEmitter studioEventEmitter;

    void Start()
    {
        pointLight = GetComponent<Light>();
        randomSeed = Random.Range(0f, 100f);
        StartCoroutine(Blink());

        studioEventEmitter.Play();
    }

    IEnumerator Blink()
    {
        while (true)
        {
            // Base con ruido Perlin
            float time = Time.time * blinkSpeed;
            float baseValue = Mathf.PerlinNoise(randomSeed, time * noiseScale);
            float baseIntensity = Mathf.Lerp(minIntensity, maxIntensity, baseValue);

            // Ocasionalmente agregarle una variación aleatoria
            if (Random.value < flickerProbability)
            {
                float flicker = Random.Range(-flickerAmount, flickerAmount);
                baseIntensity += flicker;
            }

            // Clamp para que nunca se pase de los valores
            baseIntensity = Mathf.Clamp(baseIntensity, minIntensity, maxIntensity);

            // Suavizar el paso de una intensidad a otra
            currentIntensity = Mathf.Lerp(currentIntensity, baseIntensity, Time.deltaTime * flickerSpeed);
            pointLight.intensity = currentIntensity;

            yield return null;
        }
    }
}
