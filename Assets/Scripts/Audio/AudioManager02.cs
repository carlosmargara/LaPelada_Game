/*
===========================================================
 FMOD QUICK GUIDE - Uso en Unity
===========================================================

1) RuntimeManager.PlayOneShot()
   - Para reproducir sonidos cortos (SFX) que no necesitan control posterior.
   - FMOD maneja y destruye la instancia automáticamente.
   - Ejemplo: disparos, pasos, abrir puerta.
   - No permite stop, pause ni setParameter.

   Ej:
     RuntimeManager.PlayOneShot("event:/SFX/Explosion", transform.position);

-----------------------------------------------------------

2) EventInstance
   - Para sonidos que requieren control manual (start, stop, pause, parámetros).
   - Ideal para música, loops o ambientes dinámicos.
   - IMPORTANTE: siempre hacer .release() al final para liberar recursos.

   Ej:
     private EventInstance music;
     music = RuntimeManager.CreateInstance("event:/Music/Level1");
     music.start();
     music.setParameterByName("Mood", 0.5f);
     music.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
     music.release();

-----------------------------------------------------------

3) StudioEventEmitter
   - Componente en un GameObject, se configura desde el Inspector.
   - Útil para sonidos localizados que sigan la posición del objeto.
   - Se controla con .Play(), .Stop(), .SetParameter(), etc.

   Ej:
     public StudioEventEmitter emitter;
     emitter.Play();
     emitter.SetParameter("Intensity", 1.0f);
     emitter.Stop();

===========================================================
*/

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using FMODUnity;

public class AudioManager02 : MonoBehaviour
{
    public static AudioManager02 Instance;

    [SerializeField] private Ambient_PreChasing Chasing;

    [Header("Emitters")]
    [SerializeField] private FMODUnity.StudioEventEmitter musicEmitter_MainMenu;
    [SerializeField] private FMODUnity.StudioEventEmitter ambienceEmitter;
    [SerializeField] private FMODUnity.StudioEventEmitter pantingEmitter;

    [Header("Fade Durations")]
    [SerializeField] private float fadeInDuration = 2.5f;
    [SerializeField] private float fadeOutDuration = 2.5f;

    [Header("Suspense Loop")]
    [SerializeField] private float minDelaySuspense = 30f;
    [SerializeField] private float maxDelaySuspense = 60f;

    private DiffetentTypes_footSteps_with_FmodEvent diffetentTypes_FootSteps_With_FmodEvent;


    private FMOD.Studio.EventInstance Meeting_with_PELADA;
    public FMOD.Studio.EventInstance Read_Dario_LaVoz;

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

        Meeting_with_PELADA = FMODUnity.RuntimeManager.CreateInstance("event:/Chase/Meeting_with_PELADA");
        Read_Dario_LaVoz = FMODUnity.RuntimeManager.CreateInstance("event:/Chase/Read_Dario_LaVoz");
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


            //StartSuspenseLoop();
        }
        else if (scene.name == "GameOver") // o el nombre real de tu escena
        {
            StopAllFootstepsGlobal();
            StopPantingSound();
        }
    }

    // --- Música ---
    public void PlayMusic()
    {
        if (!musicEmitter_MainMenu.IsPlaying())
            musicEmitter_MainMenu.Play();
    }

    /* - PlayOneShot -
    Esta funcion lo que hace es lanzar el evento de Fmod una sola ves
    RuntimeManager.PlayoneShot, se encarga de buscar el evento, lanzarlo osea darle play 
    y una ves que termina destruilo
    */
    public void PlayOneShot(string eventPath)
    {
        RuntimeManager.PlayOneShot(eventPath);
    }

    /* - SpawnPelda - 
    Esta funcion se lanza cuando 
    el spawn de la PELADA se activa.
    Basicamente lanza el sonido que te chumba y detiene otros
    */
    public void SpawnPelada()
    {
        diffetentTypes_FootSteps_With_FmodEvent = FindObjectOfType<DiffetentTypes_footSteps_with_FmodEvent>();

        PlayOneShot("event:/Chase/SpwanPelada");
        ambienceEmitter.Stop();

        diffetentTypes_FootSteps_With_FmodEvent.StopAllFootsteps();
    }

    public void Meet_whit_PELADA()
    {

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

    #region Metdos que llaman a los Fade de la musica
    public void FadeOutMusic(float duration)
    {
        StartCoroutine(FadeOutCoroutine(musicEmitter_MainMenu, duration));
    }

    public void FadeInMusic(float duration)
    {
        StartCoroutine(FadeInCoroutine(musicEmitter_MainMenu, duration));
    }

    public void CrossfadeMusic(StudioEventEmitter newEmitter, float duration)
    {
        StartCoroutine(CrossfadeCoroutine(musicEmitter_MainMenu, newEmitter, duration));
    }
    #endregion

    // --- Ambience ---
    public void PlayAmbience(float volume)
    {
        ambienceEmitter.Play();
    }

    public void StopAmbience()
    {
        ambienceEmitter.Stop();
    }

    // --- Jumpscare ---
    public void PlaySound_MeetingWithPELADA()
    {
        Meeting_with_PELADA.start();
    }

    #region Metodo que llama Musica de Suspenso de forma Aleatoria
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
    #endregion

    #region  Corrutinas de FadeIn / FadeOut

    // --- Fades ---
    private IEnumerator FadeOutCoroutine(StudioEventEmitter emitter, float duration)
    {
        emitter.EventInstance.getVolume(out float startVol);

        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            //Mafth.Lerp(    ); _Lo que hace es devolver un valor entre dos puntos de una escala lineal 
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
    #endregion

    public void StopAllFootstepsGlobal()
    {
        var footSteps = FindObjectOfType<DiffetentTypes_footSteps_with_FmodEvent>();
        if (footSteps != null)
        {
            footSteps.StopAllFootsteps();
        }
    }

    private void OnDestroy()
    {
        Meeting_with_PELADA.release();
        Read_Dario_LaVoz.release();
    }
}
