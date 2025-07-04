using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using FMODUnity;

public class AudioManager02 : MonoBehaviour
{
    public static AudioManager02 Instance;

    [SerializeField] private Ambient_PreChasing Chasing;

    [Header("Emitters")]
    [SerializeField] private FMODUnity.StudioEventEmitter musicEmitter;
    [SerializeField] private FMODUnity.StudioEventEmitter fx_ClosedDoor;
    [SerializeField] private FMODUnity.StudioEventEmitter pickupEmitter;
    [SerializeField] private FMODUnity.StudioEventEmitter ambienceEmitter;
    [SerializeField] private FMODUnity.StudioEventEmitter pantingEmitter;
    [SerializeField] private FMODUnity.StudioEventEmitter jumpScareEmitter;
    [SerializeField] private FMODUnity.StudioEventEmitter suspenseEmitter;
    [SerializeField] private FMODUnity.StudioEventEmitter flashLightEmitter;

    [Header("Fade Durations")]
    //[SerializeField] private float fadeInDuration = 2f;
    [SerializeField] private float fadeOutDuration = 2f;

    [Header("Suspense Loop")]
    [SerializeField] private float minDelaySuspense = 30f;
    [SerializeField] private float maxDelaySuspense = 60f;

    private Coroutine suspenseRoutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {

        PlayMusic(); // si querés que arranque música base
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;

        //Chasing.eventPlayChasinSound += PlayJumpScareSound;
        StaminaBar.OnStaminaDepleted += PlayPantingSound;
        StaminaBar.OnStaminaRecovered += StopPantingSound;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        //Chasing.eventPlayChasinSound -= PlayJumpScareSound;
        StaminaBar.OnStaminaDepleted -= PlayPantingSound;
        StaminaBar.OnStaminaRecovered -= StopPantingSound;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "LaPeladaTeAcosaFuerte")
        {
            PlayAmbience(0.5f);
            FadeOutMusic(fadeOutDuration);
            //Chasing.ambiente.start();

            StartSuspenseLoop();
        }
    }

    // --- FX ---
    public void PlaySoundFX_ClosedDoor()
    {
        fx_ClosedDoor.Play();
    }

    public void FlashLight_ON_OFF()
    {
        flashLightEmitter.Play();
    }

    public void PlayPickupSound()
    {
        pickupEmitter.Play();
    }

    // --- Música ---
    public void PlayMusic()
    {
        if (!musicEmitter.IsPlaying())
            musicEmitter.Play();
    }

    public void FadeOutMusic(float duration)
    {
        StartCoroutine(FadeOutCoroutine(musicEmitter, duration));
    }

    public void FadeInMusic(float duration)
    {
        StartCoroutine(FadeInCoroutine(musicEmitter, duration));
    }

    public void CrossfadeMusic(StudioEventEmitter newEmitter, float duration)
    {
        StartCoroutine(CrossfadeCoroutine(musicEmitter, newEmitter, duration));
    }

    // --- Ambience ---
    public void PlayAmbience(float volume)
    {
            ambienceEmitter.Play();
    }

    // --- Panting ---
    private void PlayPantingSound()
    {

       
 
            pantingEmitter.Play();
        
    }

    private void StopPantingSound()
    {
        pantingEmitter.Stop();
    }

    // --- Jumpscare ---
    private void PlayJumpScareSound()
    {
        jumpScareEmitter.Play();
    }
    
    // --- Suspense Loop ---
    private void StartSuspenseLoop()
    {
        Debug.Log("Se llamo al Sonido que te CHUMBA_");
        if (suspenseRoutine != null)
            StopCoroutine(suspenseRoutine);

        suspenseRoutine = StartCoroutine(SuspenseMusicLoop());
    }

    private IEnumerator SuspenseMusicLoop()
    {
        while (true)
        {
            float waitTime = Random.Range(minDelaySuspense, maxDelaySuspense);
            yield return new WaitForSeconds(waitTime);

            Chasing.ambiente.start();
            Debug.Log("🎵 Lanzando música de suspenso aleatoria");

            yield return new WaitForSeconds(35f); // o el largo del evento + buffer
        }
    }
    
    // --- Fades ---
    private IEnumerator FadeOutCoroutine(StudioEventEmitter emitter, float duration)
    {
        emitter.EventInstance.getVolume(out float startVol);

        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            float vol = Mathf.Lerp(startVol, 0f, t / duration);
            emitter.EventInstance.setVolume(vol);
            yield return null;
        }

        emitter.EventInstance.setVolume(0f);
        emitter.Stop();
    }


    private IEnumerator FadeInCoroutine(StudioEventEmitter emitter, float duration)
    {
        emitter.EventInstance.setVolume(0f);
        emitter.Play();

        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            float vol = Mathf.Lerp(0f, 1f, t / duration);
            emitter.EventInstance.setVolume(vol);
            yield return null;
        }

        emitter.EventInstance.setVolume(1f);
    }


    private IEnumerator CrossfadeCoroutine(StudioEventEmitter fromEmitter, StudioEventEmitter toEmitter, float duration)
    {
        float half = duration / 2f;

        fromEmitter.EventInstance.getVolume(out float fromStartVol);

        // Fade out del actual
        for (float t = 0; t < half; t += Time.deltaTime)
        {
            float vol = Mathf.Lerp(fromStartVol, 0f, t / half);
            fromEmitter.EventInstance.setVolume(vol);
            yield return null;
        }

        fromEmitter.EventInstance.setVolume(0f);
        fromEmitter.Stop();

        // Fade in del nuevo
        toEmitter.EventInstance.setVolume(0f);
        toEmitter.Play();

        for (float t = 0; t < half; t += Time.deltaTime)
        {
            float vol = Mathf.Lerp(0f, 1f, t / half);
            toEmitter.EventInstance.setVolume(vol);
            yield return null;
        }

        toEmitter.EventInstance.setVolume(1f);
    }

}
