using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UI_Manager_MainMenu : MonoBehaviour
{
    [Header("Paneles de UI")]
    [SerializeField] private GameObject panel01;
    [SerializeField] private GameObject panel02;
    [SerializeField] private GameObject panelOptions;

    [Header("Ref TMP")]
    [SerializeField] private TextMeshProUGUI pressStar_TMP;

    private UI_MainMenu_Input input;
    private List<Button> panel02Buttons = new List<Button>();
    private int currentIndex = 0;

    private bool submitPressedRecently = false;
    private float submitCooldown = 0.5f;

    private bool navCooldown = false;
    private float navDelay = 0.15f;

    private Coroutine blinkCoroutine;

    private void Awake()
    {
        input = new UI_MainMenu_Input();
    }

    private void OnEnable()
    {
        input.UI.Enable();
        input.UI.Submit.canceled += OnSubmit;
        input.UI.Cancel.performed += OnCancel;
        input.UI.Navigate.performed += OnNavigate;
    }

    private void OnDisable()
    {
        if (blinkCoroutine != null)
            StopCoroutine(blinkCoroutine);

        input.UI.Submit.canceled -= OnSubmit;
        input.UI.Cancel.performed -= OnCancel;
        input.UI.Navigate.performed -= OnNavigate;
        input.UI.Disable();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        panel01.SetActive(true);
        panel02.SetActive(false);
        panelOptions.SetActive(false);

        blinkCoroutine = StartCoroutine(BlinkText());
    }

    // ======================
    // 🔹 INPUT HANDLERS 🔹
    // ======================

    private void OnSubmit(InputAction.CallbackContext ctx)
    {
        if (!ctx.canceled || submitPressedRecently) return;

        submitPressedRecently = true;
        StartCoroutine(ResetSubmitCooldown());

        if (panel01.activeSelf)
        {
            Debug.Log("[OnSubmit] Cambiando de Panel01 → Panel02");
            panel01.SetActive(false);
            panel02.SetActive(true);
            InitializePanel02Buttons();
            SelectButton(0);
        }
        else if (panel02.activeSelf)
        {
            Debug.Log("[OnSubmit] Ejecutando botón seleccionado en Panel02");
            GameObject selected = EventSystem.current.currentSelectedGameObject;
            if (selected != null)
            {
                Button btn = selected.GetComponent<Button>();
                if (btn != null)
                {
                    Debug.Log($"[OnSubmit] Click simulado en: {btn.name}");
                    btn.onClick.Invoke();
                }
            }
        }
    }

    private IEnumerator ResetSubmitCooldown()
    {
        yield return new WaitForSeconds(submitCooldown);
        submitPressedRecently = false;
    }

    private void OnCancel(InputAction.CallbackContext ctx)
    {
        if (panelOptions.activeSelf)
        {
            panelOptions.SetActive(false);
            panel02.SetActive(true);
            InitializePanel02Buttons();
            SelectButton(0);
        }
    }

    private void OnNavigate(InputAction.CallbackContext ctx)
    {
        if (!panel02.activeSelf || panel02Buttons.Count == 0) return;

        Vector2 nav = ctx.ReadValue<Vector2>();

        // Sincronizar currentIndex con el botón actualmente seleccionado
        GameObject selected = EventSystem.current.currentSelectedGameObject;
        if (selected != null)
        {
            int index = panel02Buttons.IndexOf(selected.GetComponent<Button>());
            if (index != -1) currentIndex = index;
        }
        else
        {
            // Si no hay ningún botón seleccionado, seleccionar el primero
            SelectButton(0);
            currentIndex = 0;
        }

        // Solo mover si no hay cooldown
        if (navCooldown) return;

        if (nav.y > 0.5f)
            MoveSelection(-1); // Arriba
        else if (nav.y < -0.5f)
            MoveSelection(1); // Abajo
    }

    // ======================
    // 🔹 NAVEGACIÓN 🔹
    // ======================

    private void InitializePanel02Buttons()
    {
        panel02Buttons.Clear();
        foreach (Button b in panel02.GetComponentsInChildren<Button>(true))
        {
            if (b.gameObject.activeSelf)
                panel02Buttons.Add(b);
        }
    }

    private void MoveSelection(int direction)
    {
        if (panel02Buttons.Count == 0 || navCooldown) return;

        // 🔹 Desmarcar el botón anterior
        if (currentIndex >= 0 && currentIndex < panel02Buttons.Count)
        {
            var prevTextChanger = panel02Buttons[currentIndex].GetComponentInChildren<HoverTextColorChanger>();
            if (prevTextChanger != null)
                prevTextChanger.Unhighlight();
        }

        // 🔹 Actualizar índice cíclico
        currentIndex += direction;
        if (currentIndex < 0) currentIndex = panel02Buttons.Count - 1;
        else if (currentIndex >= panel02Buttons.Count) currentIndex = 0;

        // 🔹 Seleccionar nuevo botón
        SelectButton(currentIndex);

        // 🔹 Resaltar texto del botón nuevo
        var newTextChanger = panel02Buttons[currentIndex].GetComponentInChildren<HoverTextColorChanger>();
        if (newTextChanger != null)
            newTextChanger.Highlight();

        StartCoroutine(NavCooldownCoroutine());
    }

    private void SelectButton(int index)
    {
        if (index < 0 || index >= panel02Buttons.Count) return;
        EventSystem.current.SetSelectedGameObject(panel02Buttons[index].gameObject);
    }

    private IEnumerator NavCooldownCoroutine()
    {
        navCooldown = true;
        yield return new WaitForSeconds(navDelay);
        navCooldown = false;
    }

    // ======================
    // 🔹 UI EFFECTS 🔹
    // ======================

    private IEnumerator BlinkText()
    {
        while (true)
        {
            pressStar_TMP.enabled = !pressStar_TMP.enabled;
            yield return new WaitForSeconds(0.5f);
        }
    }

    // ======================
    // 🔹 BOTONES 🔹
    // ======================

    public void ButtonPlay() => SceneLoader.LoadScene("LaPeladaTeAcosaFuerte");

    public void ButtonOptions()
    {
        panel02.SetActive(false);
        panelOptions.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void ButtonBack()
    {
        panelOptions.SetActive(false);
        panel02.SetActive(true);
        InitializePanel02Buttons();
        SelectButton(0);
    }

    public void ButtonExit()
    {
        Debug.Log("Cerrando el juego...");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }
}


