using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeManager : MonoBehaviour
{
    public static FadeManager Instance;

    [SerializeField] private GameObject blackFade_Image;
    [SerializeField] private CanvasGroup fadeCanvasGroup;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private PlayerThoughts introThoughts;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        blackFade_Image.SetActive(true);
        StartCoroutine(FadeOutAndStartIntro());
    }

    private IEnumerator FadeOutAndStartIntro()
    {
        yield return StartCoroutine(FadeOut());

        // Acá lanzamos los pensamientos del jugador
        if (introThoughts != null)
        {
            DialogueManager.Instance.ShowThoughts(introThoughts);
        }
    }

    public IEnumerator FadeIn()
    {
        fadeCanvasGroup.gameObject.SetActive(true);
        fadeCanvasGroup.alpha = 0f;

        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            yield return null;
        }

        fadeCanvasGroup.alpha = 1f;
    }

    public IEnumerator FadeOut()
    {
        // Espera 2 segundos antes de comenzar el fundido
        yield return new WaitForSeconds(1.5f);

        // Arranca con la pantalla completamente negra
        fadeCanvasGroup.alpha = 1f;

        float timer = 0f;

        // Mientras no se haya alcanzado la duración total del fundido
        while (timer < fadeDuration)
        {
            // Incrementa el temporizador con el tiempo transcurrido desde el último frame
            timer += Time.deltaTime;

            // Calcula el porcentaje del tiempo completado (de 0 a 1)
            float t = timer / fadeDuration;

            // Interpola el alpha desde 1 (negro) hasta 0 (transparente)
            fadeCanvasGroup.alpha = Mathf.Lerp(1f, 0f, t);

            // Espera al próximo frame
            yield return null;
        }

        // Asegura que el alpha termine en 0 (por si quedó algún resto decimal)
        fadeCanvasGroup.alpha = 0f;

        // Desactiva el canvas una vez que se terminó el fundido
        fadeCanvasGroup.gameObject.SetActive(false);
    }
}

