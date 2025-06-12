using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
===========================
 AudioManager - Guía rápida
===========================

🎵 MÉTODOS DISPONIBLES:

1. PlayMusic(AudioClip clip, bool loop = false, bool forceRestart = false)
   ▶ Reproduce un clip de música inmediatamente, sin fades.
   ✅ Útil para: cortar música anterior y reproducir algo nuevo al instante.

2. FadeOutMusic(float duration)
   ▶ Baja el volumen progresivamente y detiene la música.
   ✅ Útil para: silenciar la música suavemente.

3. FadeInMusic(AudioClip clip, float duration, bool loop = false)
   ▶ Empieza una nueva música desde volumen 0 y la sube lentamente.
   ✅ Útil para: introducir música sin que suene de golpe.

4. CrossfadeTo(AudioClip newClip, float duration, bool loop = false)
   ▶ Transición suave: baja el volumen del track actual y sube el del nuevo.
   ✅ Útil para: cambiar el ambiente sin romper la inmersión (ej. suspenso → persecución).

🎯 EJEMPLOS DE USO:

// Cambio brusco
AudioManager.Instance.PlayMusic(AudioManager.Instance.chasing);

// Silenciar música
AudioManager.Instance.FadeOutMusic(2f);

// Empezar una música suavemente
AudioManager.Instance.FadeInMusic(AudioManager.Instance.suspenso, 2f);

// Cambio suave entre temas
AudioManager.Instance.CrossfadeTo(AudioManager.Instance.chasing, 3f);

*/

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    private AudioSource musicSource, soundFx, ambSound;

    [SerializeField] private float fadeIn;
    [SerializeField] private float fadeOut;

    [Space]

    [Header("Music")]
    [SerializeField] private AudioClip laPeladaTeAcosaFuerte;
    [SerializeField] public AudioClip suspenso, chasing;

    [Header("SoundFX")]
    [SerializeField] public AudioClip pickUP_Sound;
    [SerializeField] public AudioClip pantingSound;

    [Header("Ambience")]
    [SerializeField] private AudioClip ambPlazaArtes;

    public AudioSource SoundFX => soundFx;

    //patron Singleton
    private void Awake()
    {
        // Singleton: si ya hay uno, destruimos este
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // Lo hacemos persistente y guardamos la instancia
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        soundFx = transform.GetChild(0).GetComponent<AudioSource>(); //asigna a la variable el audioSource
        ambSound = transform.GetChild(1).GetComponent<AudioSource>();

        musicSource = GetComponent<AudioSource>();

        if (!musicSource.isPlaying)
        {
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "LaPeladaTeAcosaFuerte")
        {
            PlayAmbience(ambPlazaArtes,0.4f);
            
            FadeOutMusic(5f);
            /*
            // Cambiar a otra música
            musicSource.Stop();
            musicSource.clip = laPeladaTeAcosaFuerte;
            musicSource.loop = false;
            musicSource.Play();
            */
            //FadeInMusic(laPeladaTeAcosaFuerte,fadeIn,false);
        }
    }

    public void PlaySoundFX(AudioClip clip, float vol)
    {
        Debug.Log("_Se lanzo el SoundFX");
        soundFx.PlayOneShot(clip, vol);
    }

    public void PlayAmbience(AudioClip clip, float vol)
    {
        ambSound.clip = clip;
        ambSound.volume = vol;
        ambSound.loop = true;
        ambSound.Play();
    }



    #region Logica que hace que la musica empieze de una!
    public void PlayMusic(AudioClip clip, bool loop = false, bool forceRestart = false)
    {
        if (musicSource == null || clip == null) return;

        // Solo cambiamos el clip si es distinto o si queremos reiniciarlo forzosamente
        if (musicSource.clip != clip || forceRestart)
        {
            musicSource.Stop();
            musicSource.clip = clip;
            musicSource.loop = loop;
            musicSource.Play();
        }
        else if (!musicSource.isPlaying)
        {
            musicSource.loop = loop;
            musicSource.Play();
        }
    }
    #endregion

    #region Logica - FadeOut
    public void FadeOutMusic(float duration)
    {
        if (musicSource.isPlaying)
            StartCoroutine(FadeOutCoroutine(duration));
    }

    private IEnumerator FadeOutCoroutine(float duration)
    {
        float startVolume = musicSource.volume;

        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            musicSource.volume = Mathf.Lerp(startVolume, 0f, t / duration);
            yield return null;
        }

        musicSource.volume = 0f;
        musicSource.Stop();
    }
    #endregion

    #region Logica FadeIn
    public void FadeInMusic(AudioClip clip, float duration, bool loop = false)
    {
        StartCoroutine(FadeInCoroutine(clip, duration, loop));
    }

    private IEnumerator FadeInCoroutine(AudioClip clip, float duration, bool loop)
    {
        musicSource.clip = clip;
        musicSource.loop = loop;
        musicSource.volume = 0f;
        musicSource.Play();

        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            musicSource.volume = Mathf.Lerp(0f, 1f, t / duration);
            yield return null;
        }

        musicSource.volume = 1f;
    }
    #endregion

    #region Logica Crossfade
    public void CrossfadeTo(AudioClip newClip, float duration, bool loop = false)
    {
        StartCoroutine(CrossfadeCoroutine(newClip, duration, loop));
    }

    private IEnumerator CrossfadeCoroutine(AudioClip newClip, float duration, bool loop)
    {
        float startVolume = musicSource.volume;

        // Fade out
        for (float t = 0; t < duration / 2f; t += Time.deltaTime)
        {
            musicSource.volume = Mathf.Lerp(startVolume, 0f, t / (duration / 2f));
            yield return null;
        }

        musicSource.Stop();
        musicSource.clip = newClip;
        musicSource.loop = loop;
        musicSource.Play();

        // Fade in
        for (float t = 0; t < duration / 2f; t += Time.deltaTime)
        {
            musicSource.volume = Mathf.Lerp(0f, startVolume, t / (duration / 2f));
            yield return null;
        }

        musicSource.volume = startVolume;
    }
    #endregion

}
