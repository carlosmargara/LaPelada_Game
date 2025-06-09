using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) && firtTextWasShown)
        {
            ShowInteracText_Second();
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

        if (currentNote != null && currentNote.activaIA) // Primero chequeás esto
        {
            pelada.SetActive(true);
            spawnerPelada.SetActive(true);
            Debug.Log("¡La Pelada fue activada por la nota!");

            /*
            //Todo este chorizo es para lanzar un Audio, TENGO QUE REFACTORIZAR!!!
            AudioManager.Instance.MusicSourse.clip = AudioManager.Instance.suspenso;
            AudioManager.Instance.MusicSourse.loop = false;
            if(!AudioManager.Instance.MusicSourse.isPlaying)
            {
                AudioManager.Instance.MusicSourse.Play();
            }
            */
            AudioManager.Instance.PlayMusic(AudioManager.Instance.suspenso, false);
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
            pageCounterText.text = $"Página {currentPageIndex + 1}/{currentNote.pages.Count}";

            // Activar o desactivar botones
            backButton.gameObject.SetActive(currentPageIndex > 0);
            nextButton.gameObject.SetActive(currentPageIndex < currentNote.pages.Count - 1);
        }
    }
    #endregion

    #region lociga de panel Interacion
    private void ShowTextAmin(string text)
    {
        isDescribing = true;  // Comienza a describir
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
            firtTextWasShown= false;
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
        Debug.Log("_Yes");        
    }
    
    public void NoButton() //Esto esta en el Onclik del boton
    {
        isDescribing = false;
        ClosedPanelTextInterac();
        Debug.Log("_No");
    }
    #endregion
}
