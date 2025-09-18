using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class InventoryUI : Singleton<InventoryUI>
{
    [Header("Panel Descripcion")]
    [SerializeField] private GameObject panelDescription;
    [SerializeField] private TextMeshProUGUI textDescription;

    [Header("Panel Pickup")]
    [SerializeField] private GameObject panelPickup;

    [Space]
    [SerializeField] private GameObject panelInventory;

    [Space]
    [SerializeField] private Slot_Inventory slot;
    [SerializeField] private Transform container;

    private Coroutine typingCoroutine;
    private List<Slot_Inventory> availableSlots = new List<Slot_Inventory>();

    // Getter seguro para el inventario
    public bool IsInventoryOpen => panelInventory != null && panelInventory.activeSelf;

    protected override void Awake()
    {
        base.Awake();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ReassignReferences(scene);
    }

    private void Start()
    {
        // Inicializamos slots
        InitializeInventory();

        // Reasignamos referencias en la primera escena jugable
        ReassignReferences(SceneManager.GetActiveScene());
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void ReassignReferences(Scene scene)
    {
        GameObject canvas = GameObject.Find("CanvasPanel"); // tu canvas principal siempre activo
        if (canvas != null)
        {
            // Buscar paneles aunque estén desactivados
            panelInventory = canvas.transform.Find("Panel Inventory")?.gameObject;
            panelDescription = canvas.transform.Find("Panel Description")?.gameObject;
            panelPickup = canvas.transform.Find("PickupPromptPanel")?.gameObject;
            container = canvas.transform.Find("Panel Inventory/background/foreground")?.transform;


            // Buscar TextMeshPro en el panel de descripción
            textDescription = panelDescription?.GetComponentInChildren<TextMeshProUGUI>(true);

            // Cargar prefab del slot desde Resources
            if (slot == null)
                slot = Resources.Load<Slot_Inventory>("Prefabs/Slot - Button");

            Debug.Log($"[InventoryUI] Referencias reasignadas en la escena {scene.name}");
        }
        else
        {
            Debug.LogWarning("[InventoryUI] No se encontró CanvasPanel en la escena");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.Z))
        {
            ToggleInventory();
        }

        if (IsInventoryOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleInventory();
        }
    }

    private void ToggleInventory()
    {
        if (panelInventory == null) return;

        bool willOpen = !panelInventory.activeSelf;
        panelInventory.SetActive(willOpen);

        if (willOpen)
            GameStateManager.Instance.LockPlayer(priority: 2);
        else
            GameStateManager.Instance.UnlockPlayer(priority: 2);
    }

    private void InitializeInventory()
    {
        if (container == null || slot == null) return;

        for (int i = 0; i < Inventory.Instance.Numberslot; i++)
        {
            Slot_Inventory newSlot = Instantiate(slot, container);
            newSlot.Index = i;
            availableSlots.Add(newSlot);
        }
    }

    public void DrawItemInInventory(Inventory_Item itemToAdd, int amount, int itemIndex)
    {
        if (itemIndex < 0 || itemIndex >= availableSlots.Count) return;

        Slot_Inventory slot = availableSlots[itemIndex];
        if (itemToAdd != null)
        {
            slot.ActivateSlotUI(true);
            slot.UpdateSlot(itemToAdd, amount);
        }
        else
        {
            slot.ActivateSlotUI(false);
        }
    }

    #region Description Panel
    public void ShowItemDescription(int index)
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);

        string description = "Empty";
        if (Inventory.Instance.Items[index] != null)
            description = Inventory.Instance.Items[index].Description;

        panelDescription.SetActive(true);
        textDescription.text = "";
        typingCoroutine = StartCoroutine(TypeText(description));
    }

    public void HideItemDescription()
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);

        panelDescription.SetActive(false);
        textDescription.text = "";
    }

    private IEnumerator TypeText(string fullText)
    {
        textDescription.text = "";
        foreach (char c in fullText)
        {
            textDescription.text += c;
            yield return new WaitForSeconds(0.05f);
        }
    }
    #endregion
}
