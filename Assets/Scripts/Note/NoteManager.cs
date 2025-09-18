using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NoteManager : Singleton<NoteManager>
{
    [Header("Notas")]
    [SerializeField] private GameObject panelNote;
    [SerializeField] private Image foregroundImage;

    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI pageText;
    [SerializeField] private TextMeshProUGUI pageCounterText;

    [SerializeField] private Button nextButton;
    [SerializeField] private Button backButton;

    private NoteData currentNote;
    private int currentPageIndex;

    [Space]

    [Header("Panel Interacion")]
    [SerializeField] private GameObject panelTextInterac;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Button yesButton;
    [SerializeField] private Button noButton;
    private string currentTextAnimating; // Guarda el texto que se está mostrando

    public Note_Interaction noteInteraction { get; set; }

    private bool textAmin;
    public bool isDescribing { get; private set; }
    private bool firtTextWasShown;

    [Space]
    [SerializeField] private GameObject pelada;
    [SerializeField] private GameObject spawnerPelada;

    private void Start()
    {
        panelNote.SetActive(false);
        panelTextInterac.SetActive(false);
    }

    private void Update()
    {
        if (panelNote.activeSelf || panelTextInterac.activeSelf)
        {
            GameStateManager.Instance.LockPlayer(priority: 1);
        }
        else
        {
            GameStateManager.Instance.UnlockPlayer(priority: 1);
        }

        if (panelTextInterac.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ClosedPanelTextInterac();
                return;
            }

            if ((Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)))
            {
                if (!textAmin)
                {
                    SkipTextAnimation(); // Acelera animación
                    return;
                }

                if (firtTextWasShown)
                {
                    ShowInteracText_Second();
                }
            }
        }
    }


    #region logica de notas
    public void ShowSetupNotes(NoteData note)
    {
        currentNote = note;
        currentPageIndex = 0;

        panelNote.SetActive(true);

        title.text = note.noteTitle;
        foregroundImage.sprite = note.foregroundImage;

        UpdatePageText();
    }

    public void Button_ExitPanelNote()
    {
        Debug.Log("_EXIT");
        panelNote.SetActive(false);
        AudioManager02.Instance.PlayOneShot("event:/UI/Note_Close");

        if (currentNote != null && currentNote.activaIA) // Primero cheque�s esto
        {
            pelada.SetActive(true);
            spawnerPelada.SetActive(true);
            Debug.Log("�La Pelada fue activada por la nota!");

            /*
            //Todo este chorizo es para lanzar un Audio, TENGO QUE REFACTORIZAR!!!
            AudioManager.Instance.MusicSourse.clip = AudioManager.Instance.suspenso;
            AudioManager.Instance.MusicSourse.loop = false;
            if(!AudioManager.Instance.MusicSourse.isPlaying)
            {
                AudioManager.Instance.MusicSourse.Play();
            }
            */
            //AudioManager.Instance.PlayMusic(AudioManager.Instance.suspenso, false);
            //AudioManager02.Instance.Read_Dario_LaVoz.start();
        }

        currentNote = null;

        ClosedPanelTextInterac();
    }

    public void Button_NextPanelNote()
    {
        if (currentNote == null || currentPageIndex >= currentNote.pages.Count - 1) return;

        currentPageIndex++;
        UpdatePageText();
        Debug.Log("_NEXT");
    }

    public void Button_BackPanelNote()
    {
        if (currentNote == null || currentPageIndex <= 0) return;

        currentPageIndex--;
        UpdatePageText();
        Debug.Log("_BACK");
    }

    private void UpdatePageText()
    {
        if (currentNote != null)
        {
            pageText.text = currentNote.pages[currentPageIndex];
            pageCounterText.text = $"P�gina {currentPageIndex + 1}/{currentNote.pages.Count}";

            // Activar o desactivar botones
            backButton.gameObject.SetActive(currentPageIndex > 0);
            nextButton.gameObject.SetActive(currentPageIndex < currentNote.pages.Count - 1);
        }
    }
    #endregion

    #region lociga de panel Interacion
    private void ShowTextAmin(string text)
    {
        isDescribing = true;
        currentTextAnimating = text; // Guardamos el texto actual
        StopAllCoroutines(); // Por si se estaba animando otro texto
        StartCoroutine(AminText(text));
    }

    private IEnumerator AminText(string text)
    {
        textAmin = false;
        messageText.text = "";
        char[] chars = text.ToCharArray();

        for (int i = 0; i < chars.Length; i++)
        {
            messageText.text += chars[i];
            yield return new WaitForSeconds(0.03f);
        }

        textAmin = true;
    }

    public void ShowInteracText_First(NoteData note)
    {
        panelTextInterac.SetActive(true);
        ShowTextAmin(note.interacText01);
        firtTextWasShown = true;
        yesButton.gameObject.SetActive(false);
        noButton.gameObject.SetActive(false);
    }

    private void ShowInteracText_Second()
    {
        if (firtTextWasShown && noteInteraction != null)
        {
            string secondtext = noteInteraction.Data.interacText02;
            ShowTextAmin(secondtext);
            firtTextWasShown = false;
            yesButton.gameObject.SetActive(true);
            noButton.gameObject.SetActive(true);
        }
    }

    private void ClosedPanelTextInterac()
    {
        panelTextInterac.SetActive(false);
        isDescribing = false;
        Debug.Log("Cerre panel " + panelTextInterac);
    }

    public void YesButton() //Esto esta en el Onclick del boton
    {
        isDescribing = false;
        ShowSetupNotes(noteInteraction.Data);
        AudioManager02.Instance.PlayOneShot("event:/UI/Note_Open");
        Debug.Log("_Yes");
    }

    public void NoButton() //Esto esta en el Onclik del boton
    {
        isDescribing = false;
        ClosedPanelTextInterac();
        Debug.Log("_No");
    }
    #endregion

    private void SkipTextAnimation()
    {
        StopAllCoroutines();
        messageText.text = currentTextAnimating; // Mostrar todo el texto al instante
        textAmin = true;
    }

    private GameObject FindInChildrenIncludingInactive(GameObject parent, string name)
    {
        Transform[] children = parent.GetComponentsInChildren<Transform>(true); // true = incluye inactivos
        foreach (Transform child in children)
        {
            if (child.name == name)
                return child.gameObject;
        }
        return null;
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
        // Intentamos encontrar el CanvasPanel persistente o recién cargado
        GameObject canvas = GameObject.Find("CanvasPanel");
        if (canvas == null)
        {
            Debug.LogWarning("No se encontró CanvasPanel en la escena.");
            return;
        }

        // UI Notas
        if (panelNote == null)
            panelNote = FindInChildrenIncludingInactive(canvas, "Note Panel");

        if (foregroundImage == null)
            foregroundImage = FindInChildrenIncludingInactive(canvas, "foreground - Image")?.GetComponent<Image>();

        if (title == null)
            title = FindInChildrenIncludingInactive(canvas, "Title - Text (TMP)")?.GetComponent<TextMeshProUGUI>();

        if (pageText == null)
            pageText = FindInChildrenIncludingInactive(canvas, "PageText - Text (TMP)")?.GetComponent<TextMeshProUGUI>();

        if (pageCounterText == null)
            pageCounterText = FindInChildrenIncludingInactive(canvas, "pageCounterText - Text (TMP)")?.GetComponent<TextMeshProUGUI>();

        if (nextButton == null)
            nextButton = FindInChildrenIncludingInactive(canvas, "Next - Button")?.GetComponent<Button>();

        if (backButton == null)
            backButton = FindInChildrenIncludingInactive(canvas, "Back - Button")?.GetComponent<Button>();

        // UI Interacción
        if (panelTextInterac == null)
            panelTextInterac = FindInChildrenIncludingInactive(canvas, "Note Interac");

        if (messageText == null)
            messageText = FindInChildrenIncludingInactive(canvas, "interacText - Text (TMP)")?.GetComponent<TextMeshProUGUI>();

        if (yesButton == null)
            yesButton = FindInChildrenIncludingInactive(canvas, "Yes - Button")?.GetComponent<Button>();

        if (noButton == null)
            noButton = FindInChildrenIncludingInactive(canvas, "No - Button")?.GetComponent<Button>();

        // Pelada
        if (pelada == null)
            pelada = GameObject.Find("La Pelada");

        if (spawnerPelada == null)
            spawnerPelada = GameObject.Find("Spawner_PELADA");

        // Desactivar paneles por defecto
        if (panelNote != null) panelNote.SetActive(false);
        if (panelTextInterac != null) panelTextInterac.SetActive(false);

        Debug.Log("NoteManager: referencias cargadas tras escena " + scene.name);
    }
}

