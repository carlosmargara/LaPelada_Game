using System.Collections;
using UnityEngine;
using FMODUnity;

public class BlinkingLight_con_patrón_irregular : MonoBehaviour
{
    [Header("Intensidad")]
    public float minIntensity = 0.5f;
    public float maxIntensity = 1.5f;

    [Header("Parpadeo base")]
    public float blinkSpeed = 1.0f; // Velocidad del ruido base
    public float noiseScale = 1.0f; // Suavidad del ruido base

    [Header("Temblor aleatorio")]
    public float flickerProbability = 0.1f; // Probabilidad de temblor por frame
    public float flickerAmount = 0.2f; // Qué tanto se desvía la intensidad base
    public float flickerSpeed = 10f; // Qué tan rápido vuelve a la base

    private Light pointLight;
    private float randomSeed;
    private float currentIntensity;
    private Coroutine blinkRoutine;

    [SerializeField] private StudioEventEmitter studioEventEmitter;

    private void Awake()
    {
        pointLight = GetComponent<Light>();
        randomSeed = Random.Range(0f, 100f);
    }

    private void OnEnable()
    {
        // Asegura que la luz se vea al habilitar este componente
        if (pointLight != null) pointLight.enabled = true;

        // Arranca el parpadeo
        if (blinkRoutine == null) blinkRoutine = StartCoroutine(Blink());

        // Arranca el sonido
        if (studioEventEmitter != null) studioEventEmitter.Play();
    }

    private void OnDisable()
    {
        // Apaga la luz visualmente
        if (pointLight != null) pointLight.enabled = false;

        // Frena el parpadeo
        if (blinkRoutine != null)
        {
            StopCoroutine(blinkRoutine);
            blinkRoutine = null;
        }

        // Frena el sonido para que al volver empiece de cero
        if (studioEventEmitter != null)
        {
            // Parada inmediata (sin fade). Si preferís con fade, usá studioEventEmitter.Stop();
            studioEventEmitter.EventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        }
    }

    IEnumerator Blink()
    {
        while (true)
        {
            float time = Time.time * blinkSpeed;
            float baseValue = Mathf.PerlinNoise(randomSeed, time * noiseScale);
            float baseIntensity = Mathf.Lerp(minIntensity, maxIntensity, baseValue);

            if (Random.value < flickerProbability)
            {
                float flicker = Random.Range(-flickerAmount, flickerAmount);
                baseIntensity += flicker;
            }

            baseIntensity = Mathf.Clamp(baseIntensity, minIntensity, maxIntensity);
            currentIntensity = Mathf.Lerp(currentIntensity, baseIntensity, Time.deltaTime * flickerSpeed);
            if (pointLight != null) pointLight.intensity = currentIntensity;

            yield return null;
        }
    }
}

