using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;
using UnityEngine.InputSystem;

public class PickupUIManager : Singleton<PickupUIManager>
{
    [Header("UI Elements")]
    [SerializeField] private GameObject panel;
    [Space]
    [SerializeField] private TextMeshProUGUI messageText;
    [Space]
    [SerializeField] private GameObject _yesButton;
    [SerializeField] private GameObject _noButton;

    [Header("3D Preview")]
    [SerializeField] private GameObject itemPreviewHolder;
    [SerializeField] private GameObject itemView_rawImage; //esta es la referencia del gameObject que contiene la rawImage que muestra el objeto3D
    [SerializeField] private RawImage itemPreviewImage; // Asegurate que esta sea tu RawImage en el canvas
    [SerializeField] private RenderTexture renderTexture;
    [SerializeField] private Camera previewCamera;

    [Header("Navegación UI")]
    [SerializeField] private float mouseInactiveTime = 1.2f;
    [SerializeField] private float mouseMoveThreshold = 2f;
    private Vector2 lastMousePosition;
    private float lastMouseMoveTime;
    private GameObject firstSelectedButton;
    private bool navigationActive;


    private GameObject currentPreviewObject;

    private float inputCooldown = 0f;
    private string currentTextAnimating; //variable que guarda que texto se esta mostrando igual que en NPCManager(DialogueManager)

    public bool firtTextWasShown;
    private bool secondTextWasShown;
    private bool descriptionItemAmin;
    private PickupItem_interac currentItem;

    private bool mouseActive;
    public bool IsMouseActive => mouseActive;
    public bool IsActive => panel.activeSelf;

    private PlayerInteraction playerInteraction;

    private bool justOpenedPanel = false;
    private float lastSubmitTime = -1f;



    private void Start()
    {
        playerInteraction = FindObjectOfType<PlayerInteraction>();

        panel.SetActive(false);

        if (previewCamera != null)
        {
            previewCamera.targetTexture = renderTexture;
            previewCamera.gameObject.SetActive(false); // Se activa s�lo cuando hace falta
        }
    }

    private void Update()
    {
        if (!panel.activeSelf) return;

        if (inputCooldown > 0f)
        {
            inputCooldown -= Time.deltaTime;
            return;
        }

        if (currentPreviewObject != null)
        {
            currentPreviewObject.transform.Rotate(Vector3.up, 45 * Time.deltaTime);
        }
    }

    private void LateUpdate()
    {
        if (!panel.activeSelf) return;

        if (Mouse.current != null)
        {
            Vector2 currentMousePos = Mouse.current.position.ReadValue();

            if (Vector2.Distance(currentMousePos, lastMousePosition) > mouseMoveThreshold)
            {
                lastMouseMoveTime = Time.time;
                mouseActive = true; // mouse activo

                // Solo borramos selección si antes estaba seleccionada por teclado
                if (EventSystem.current.currentSelectedGameObject != null)
                    EventSystem.current.SetSelectedGameObject(null);
            }

            lastMousePosition = currentMousePos;

            // Si el mouse no se movió por un tiempo → volver a teclado
            if (Time.time - lastMouseMoveTime > mouseInactiveTime)
                mouseActive = false;
        }
    }

    public void OnSubmit(InputAction.CallbackContext ctx)
    {
        if (!panel.activeSelf || !ctx.performed) return;

        // Prevenir doble submit
        if (Time.time - lastSubmitTime < 0.25f) return;
        lastSubmitTime = Time.time;

        // Si el texto todavía se está animando → saltarlo
        if (!descriptionItemAmin)
        {
            SkipTextAnimation();
            return;
        }

        // Si estamos en el primer texto → mostrar el segundo texto con botones
        if (firtTextWasShown && !secondTextWasShown)
        {
            ShowSecondTextPickup(currentItem);
            return;
        }

        // Después de mostrar todo y los botones ya fueron usados → cerrar panel / recoger ítem
        if (secondTextWasShown && !(_yesButton.activeSelf || _noButton.activeSelf))
        {
            PastNextAction();
        }
    }


    public void OnClick(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        OnSubmit(ctx); // 🔁 Click izquierdo actúa igual que barra espaciadora
    }


    public void OnCancel(InputAction.CallbackContext ctx)
    {
        if (!panel.activeSelf) return;

        // Cerrar panel directamente
        ClosePanel();
    }

    public void OnNavigate(InputAction.CallbackContext ctx)
    {
        if (!panel.activeSelf || !ctx.performed || mouseActive) return;

        Vector2 move = ctx.ReadValue<Vector2>();
        if (move == Vector2.zero) return;

        GameObject current = EventSystem.current.currentSelectedGameObject;

        // Si no hay selección, elegimos el primer botón visible
        if (current == null)
        {
            GameObject first = _yesButton.activeSelf ? _yesButton : _noButton;
            EventSystem.current.SetSelectedGameObject(first);
            return;
        }

        Selectable sel = current.GetComponent<Selectable>();
        if (sel == null) return;

        Selectable next = null;
        if (Mathf.Abs(move.y) > Mathf.Abs(move.x))
            next = move.y > 0 ? sel.FindSelectableOnUp() : sel.FindSelectableOnDown();
        else
            next = move.x > 0 ? sel.FindSelectableOnRight() : sel.FindSelectableOnLeft();

        if (next != null)
            EventSystem.current.SetSelectedGameObject(next.gameObject);
    }

    // Llamado desde PlayerInteraction -> OnPoint
    public void OnPoint(InputAction.CallbackContext ctx)
    {
        if (!panel.activeSelf || !ctx.performed) return;

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

    public void ShowPickupPrompt(PickupItem_interac item)
    {
        currentItem = item;
        panel.SetActive(true);
        GameStateManager.Instance.LockPlayer(priority: 3);

        InputMapController inputMapController = FindObjectOfType<InputMapController>();
        inputMapController?.SwitchToUI();

        // 🔹 Resetear cualquier input residual de Submit
        if (inputMapController != null)
        {
            InputAction submitAction = inputMapController.GetComponent<PlayerInput>()
                .currentActionMap.FindAction("Submit");
            submitAction?.Reset();
        }

        justOpenedPanel = true;
        StartCoroutine(ResetJustOpenedFlag());

        ShowTextAmin(item.Ref_ScriptableObject.pickupText);
        _yesButton.SetActive(false);
        _noButton.SetActive(false);

        firtTextWasShown = true;
        inputCooldown = 0.5f;
        lastSubmitTime = Time.time; // 🛡️ Previene doble submit accidental

        HideItemPreview();

        if (Mouse.current != null)
        {
            lastMousePosition = Mouse.current.position.ReadValue();
            lastMouseMoveTime = Time.time;
        }

        firstSelectedButton = _yesButton;
    }


    private IEnumerator ResetJustOpenedFlag()
    {

        yield return new WaitForSeconds(0.4f); // 🔄 esperar un poco más
        justOpenedPanel = false;
    }

    private void ShowSecondTextPickup(PickupItem_interac item)
    {
        currentItem = item;
        ShowTextAmin(item.Ref_ScriptableObject.pickupText02);

        _yesButton.SetActive(true);
        _noButton.SetActive(true);

        // Si el mouse no se movió → seleccionar botón por teclado
        if (!mouseActive)
            EventSystem.current.SetSelectedGameObject(_yesButton);
        else
            EventSystem.current.SetSelectedGameObject(null);

        secondTextWasShown = true;
        firtTextWasShown = false;
    }


    public void ConfirmPickup()
    {
        AudioManager02.Instance.PlayOneShot("event:/Fxs/PickUp Item");

        StopAllCoroutines();
        ShowTextAmin(currentItem.Ref_ScriptableObject.confirmationText);
        itemView_rawImage.SetActive(true);
        _yesButton.SetActive(false);
        _noButton.SetActive(false);
        secondTextWasShown = true;
        firtTextWasShown = false;

        ShowItemPreview(currentItem.Ref_ScriptableObject);


        // 🔥 MARCAR COMO RECOGIDO EN EL GAME STATE
        GameStateManager.Instance.MarkItemPicked(currentItem.Ref_ScriptableObject.ID);

        Destroy(currentItem.gameObject);
    }

    public void ClosePanel()
    {
        StopAllCoroutines();
        panel.SetActive(false);
        itemView_rawImage.SetActive(false);
        secondTextWasShown = false;
        descriptionItemAmin = false;
        firtTextWasShown = false;
        currentItem = null;

        HideItemPreview();

        playerInteraction.BlockInteractFor(0.2f);
        GameStateManager.Instance.UnlockPlayer(priority: 3); // desbloqueamos solo al cerrar
                                                             // AudioManager02.Instance.PlayOneShot("event:/UI/ClosePanel");

        InputMapController inputMapController = FindObjectOfType<InputMapController>();
        inputMapController?.SwitchToPlayer();

    }

    private void PastNextAction()
    {
        currentItem.Pickup();
        ClosePanel();
    }

    private void ShowItemPreview(Inventory_Item itemData)
    {
        if (itemData == null || itemData.prefabModel == null || itemPreviewHolder == null) return;

        currentPreviewObject = Instantiate(itemData.prefabModel, itemPreviewHolder.transform);
        currentPreviewObject.transform.localPosition = Vector3.zero;
        currentPreviewObject.transform.localRotation = Quaternion.identity;
        currentPreviewObject.transform.localScale = Vector3.one;

        if (previewCamera != null)
        {
            previewCamera.gameObject.SetActive(true);
        }
    }

    private void HideItemPreview()
    {
        if (currentPreviewObject != null)
        {
            Destroy(currentPreviewObject);
        }

        if (previewCamera != null)
        {
            previewCamera.gameObject.SetActive(false);
        }
    }

    private void ShowTextAmin(string text)
    {
        currentTextAnimating = text; // guarda el texto actual
        StartCoroutine(AminText(text));
    }

    private IEnumerator AminText(string text)
    {
        descriptionItemAmin = false;
        messageText.text = "";
        char[] chars = text.ToCharArray();

        for (int i = 0; i < chars.Length; i++)
        {
            messageText.text += chars[i];
            yield return new WaitForSeconds(0.03f);
        }

        descriptionItemAmin = true;
    }

    private void SkipTextAnimation()
    {
        StopAllCoroutines();
        messageText.text = currentTextAnimating; // muestra directamente el texto actual
        descriptionItemAmin = true;
    }
}
