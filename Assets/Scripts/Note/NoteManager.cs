using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

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
    [SerializeField] private Button exitButton;

    private NoteData currentNote;
    private int currentPageIndex;

    [Space]

    [Header("Panel Interacción")]
    [SerializeField] private GameObject panelTextInterac;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Button yesButton;
    [SerializeField] private Button noButton;
    private string currentTextAnimating;

    public Note_Interaction noteInteraction { get; set; }

    private bool textAmin;
    public bool isDescribing { get; private set; }
    private bool firtTextWasShown;

    private float inputCooldown = 0f;
    public bool IsDescribing => isDescribing;

    // --- NUEVO: Lógica de navegación (copiada del PickupUIManager) ---
    [Header("Navegación UI")]
    [SerializeField] private float mouseInactiveTime = 1.2f;
    [SerializeField] private float mouseMoveThreshold = 2f;
    private Vector2 lastMousePosition;
    private float lastMouseMoveTime;
    private GameObject firstSelectedButton;
    private bool mouseActive;
    public bool IsMouseActive => mouseActive;

    //private PlayerInteraction playerInteraction;

    private void Start()
    {
        //playerInteraction = FindObjectOfType<PlayerInteraction>();

        panelNote.SetActive(false);
        panelTextInterac.SetActive(false);
    }

    private void Update()
    {
        // --- Cooldown ---
        if (inputCooldown > 0f)
            inputCooldown -= Time.deltaTime;

        if (panelNote.activeSelf || panelTextInterac.activeSelf)
        {
            GameStateManager.Instance.LockPlayer(priority: 1);
        }
        else
        {
            GameStateManager.Instance.UnlockPlayer(priority: 1);
        }
    }

    private void LateUpdate()
    {
        if (!panelTextInterac.activeSelf && !panelNote.activeSelf) return;

        // Detectar movimiento del mouse
        if (Mouse.current != null)
        {
            Vector2 currentMousePos = Mouse.current.position.ReadValue();

            if (Vector2.Distance(currentMousePos, lastMousePosition) > mouseMoveThreshold)
            {
                lastMouseMoveTime = Time.time;
                mouseActive = true; // mouse activo

                // Si había algo seleccionado con teclado, lo deseleccionamos
                if (EventSystem.current.currentSelectedGameObject != null)
                    EventSystem.current.SetSelectedGameObject(null);
            }

            lastMousePosition = currentMousePos;

            // Si el mouse no se movió por un tiempo → volver a teclado
            if (Time.time - lastMouseMoveTime > mouseInactiveTime)
                mouseActive = false;
        }
    }

    #region New Input Sistem ----- ActionMap "UI" ------
    public void OnNavigate(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;

        // Panel de interacción
        if (panelTextInterac.activeSelf && !mouseActive)
        {
            if (EventSystem.current.currentSelectedGameObject == null)
            {
                firstSelectedButton = yesButton.gameObject.activeSelf ? yesButton.gameObject :
                                       noButton.gameObject.activeSelf ? noButton.gameObject : null;
                if (firstSelectedButton != null)
                    EventSystem.current.SetSelectedGameObject(firstSelectedButton);
            }
        }

        // Panel de nota
        else if (panelNote.activeSelf && !mouseActive)
        {
            if (EventSystem.current.currentSelectedGameObject == null)
            {
                if (nextButton.gameObject.activeSelf)
                    firstSelectedButton = nextButton.gameObject;
                else if (backButton.gameObject.activeSelf)
                    firstSelectedButton = backButton.gameObject;
                else if (exitButton != null && exitButton.gameObject.activeSelf)
                    firstSelectedButton = exitButton.gameObject;

                if (firstSelectedButton != null)
                    EventSystem.current.SetSelectedGameObject(firstSelectedButton);
            }
        }
    }

    public void OnPoint(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;

        if (Mouse.current != null)
        {
            Vector2 currentMousePos = Mouse.current.position.ReadValue();

            if (Vector2.Distance(currentMousePos, lastMousePosition) > mouseMoveThreshold)
            {
                lastMouseMoveTime = Time.time;
                mouseActive = true;

                if (EventSystem.current.currentSelectedGameObject != null)
                    EventSystem.current.SetSelectedGameObject(null);
            }

            lastMousePosition = currentMousePos;
        }
    }

    public void OnSubmit(InputAction.CallbackContext ctx)
    {
        if (!panelNote.activeSelf && !panelTextInterac.activeSelf) return;
        if (inputCooldown > 0f) return;
        inputCooldown = 0.15f;
        /*
        // --- Panel de nota ---
        if (panelNote.activeSelf && panelTextInterac.activeSelf)
        {
            // Teclado / Gamepad: solo si hay algo seleccionado
            var selected = EventSystem.current.currentSelectedGameObject;
            if (selected == null) return;

            if (selected == nextButton.gameObject)
                Button_NextPanelNote();
            else if (selected == backButton.gameObject)
                Button_BackPanelNote();
            else if (selected == exitButton.gameObject)
                Button_ExitPanelNote();
        }
        */
        // --- Panel de interacción ---
        if (panelTextInterac.activeSelf)
        {
            if (!textAmin)
            {
                SkipTextAnimation();
            }
            else if (firtTextWasShown)
            {
                ShowInteracText_Second();
            }
            /*
            else
            {
                var selected = EventSystem.current.currentSelectedGameObject;
                if (selected != null)
                {
                    var button = selected.GetComponent<Button>();
                    if (button != null)
                        button.onClick.Invoke();
                }
            }
            */
        }
    }

    public void OnClick(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        Debug.Log("_ENTRO EN ONCLICK UI");
        OnSubmit(ctx); // reutilizamos la misma lógica
    }

    public void OnCancel()
    {
        if (!panelTextInterac.activeSelf && !panelNote.activeSelf) return;
        if (inputCooldown > 0f) return;
        inputCooldown = 0.15f;

        if (panelTextInterac.activeSelf)
        {
            ClosedPanelTextInterac();
            return;
        }
    }
    #endregion

    #region lógica notas
    public void ShowSetupNotes(NoteData note)
    {
        currentNote = note;
        currentPageIndex = 0;

        panelNote.SetActive(true);

        title.text = note.GetTitle();
        foregroundImage.sprite = note.foregroundImage;
        UpdatePageText();

        // Inicializa mouse y selección
        if (Mouse.current != null)
        {
            lastMousePosition = Mouse.current.position.ReadValue();
            lastMouseMoveTime = Time.time;
        }
        mouseActive = false;

        // Auto-selecciona el primer botón según prioridad
        if (nextButton.gameObject.activeSelf)
            EventSystem.current.SetSelectedGameObject(nextButton.gameObject);
        else if (backButton.gameObject.activeSelf)
            EventSystem.current.SetSelectedGameObject(backButton.gameObject);
        else if (exitButton != null && exitButton.gameObject.activeSelf)
            EventSystem.current.SetSelectedGameObject(exitButton.gameObject);

    }

    public void Button_ExitPanelNote()
    {
        panelNote.SetActive(false);
        AudioManager02.Instance.PlayOneShot("event:/UI/Note_Close");

        if (currentNote != null && currentNote.activaIA)
        {
            if (!GameStateManager.Instance.peladaTriggered)
            {
                AI_VersionWithState pelada_ = FindObjectOfType<AI_VersionWithState>(true);
                PeladaSpawner spawner = FindObjectOfType<PeladaSpawner>(true);

                if (pelada_ != null) pelada_.gameObject.SetActive(true);
                if (spawner != null) spawner.gameObject.SetActive(true);

                GameStateManager.Instance.peladaTriggered = true;
            }
        }

        currentNote = null;
        ClosedPanelTextInterac();
    }

    public void Button_NextPanelNote()
    {
        if (currentNote == null || currentPageIndex >= currentNote.GetPageCount() - 1) return;


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
            pageText.text = currentNote.GetPage(currentPageIndex);
            pageCounterText.text = $"Página {currentPageIndex + 1}/{currentNote.GetPageCount()}";

            backButton.gameObject.SetActive(currentPageIndex > 0);
            nextButton.gameObject.SetActive(currentPageIndex < currentNote.GetPageCount() - 1);

            // 🔹 NUEVO: reestablecer selección si estás usando teclado/gamepad
            if (!mouseActive)
            {
                GameObject newSelected = null;

                if (nextButton.gameObject.activeSelf)
                    newSelected = nextButton.gameObject;
                else if (backButton.gameObject.activeSelf)
                    newSelected = backButton.gameObject;
                else if (exitButton.gameObject.activeSelf)
                    newSelected = exitButton.gameObject;

                if (newSelected != null)
                    EventSystem.current.SetSelectedGameObject(newSelected);
            }
        }
    }
    #endregion

    #region panel Interacción
    private void ShowTextAmin(string text)
    {
        isDescribing = true;
        currentTextAnimating = text;
        StopAllCoroutines();
        textAmin = false;
        inputCooldown = 0.1f;
        StartCoroutine(AminText(text));
    }

    private IEnumerator AminText(string text)
    {
        textAmin = false;
        messageText.text = "";
        foreach (char c in text)
        {
            messageText.text += c;
            yield return new WaitForSeconds(0.03f);
        }
        textAmin = true;
    }

    public void ShowInteracText_First(NoteData note)
    {
        panelTextInterac.SetActive(true);
        inputCooldown = 0.25f;
        textAmin = false;
        firtTextWasShown = false;

        InputMapController inputMapController = FindObjectOfType<InputMapController>();
        if (inputMapController != null)
        {
            inputMapController.SwitchToUI();
        }

        ShowTextAmin(note.GetInteracText01());
        firtTextWasShown = true;

        yesButton.gameObject.SetActive(false);
        noButton.gameObject.SetActive(false);

        // Guardamos posición inicial del mouse
        if (Mouse.current != null)
        {
            lastMousePosition = Mouse.current.position.ReadValue();
            lastMouseMoveTime = Time.time;
        }

        // Guardamos el primer botón para navegación
        firstSelectedButton = yesButton.gameObject;
    }

    private void ShowInteracText_Second()
    {
        if (firtTextWasShown && noteInteraction != null)
        {
            string secondtext = noteInteraction.Data.GetInteracText02();
            ShowTextAmin(secondtext);
            firtTextWasShown = false;
            yesButton.gameObject.SetActive(true);
            noButton.gameObject.SetActive(true);

            // Selecciona automáticamente el botón "Yes"
            EventSystem.current.SetSelectedGameObject(yesButton.gameObject);
        }
    }

    private void ClosedPanelTextInterac()
    {
        //playerInteraction.BlockInteractFor(0.2f);
        panelTextInterac.SetActive(false);
        isDescribing = false;

        InputMapController inputMapController = FindObjectOfType<InputMapController>();
        if (inputMapController != null)
        {
            inputMapController.SwitchToPlayer();
        }
    }

    public void YesButton()
    {
        isDescribing = false;
        ShowSetupNotes(noteInteraction.Data);
        AudioManager02.Instance.PlayOneShot("event:/UI/Note_Open");
    }

    public void NoButton()
    {
        isDescribing = false;
        ClosedPanelTextInterac();
    }
    #endregion

    private void SkipTextAnimation()
    {
        StopAllCoroutines();
        messageText.text = currentTextAnimating;
        textAmin = true;
    }
    /*
    // --- OnSceneLoaded igual que antes ---
    private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameObject canvas = GameObject.Find("CanvasPanel");
        if (canvas == null)
        {
            Debug.LogWarning("No se encontró CanvasPanel en la escena.");
            return;
        }

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

        if (panelTextInterac == null)
            panelTextInterac = FindInChildrenIncludingInactive(canvas, "Note Interac");

        if (messageText == null)
            messageText = FindInChildrenIncludingInactive(canvas, "interacText - Text (TMP)")?.GetComponent<TextMeshProUGUI>();

        if (yesButton == null)
            yesButton = FindInChildrenIncludingInactive(canvas, "Yes - Button")?.GetComponent<Button>();

        if (noButton == null)
            noButton = FindInChildrenIncludingInactive(canvas, "No - Button")?.GetComponent<Button>();

        if (panelNote != null) panelNote.SetActive(false);
        if (panelTextInterac != null) panelTextInterac.SetActive(false);

        Debug.Log("NoteManager: referencias cargadas tras escena " + scene.name);
    }

    private GameObject FindInChildrenIncludingInactive(GameObject parent, string name)
    {
        Transform[] children = parent.GetComponentsInChildren<Transform>(true);
        foreach (Transform child in children)
        {
            if (child.name == name)
                return child.gameObject;
        }
        return null;
    }
    */
}




