using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using TMPro;
using UnityEngine.EventSystems;

public class InventoryUI : Singleton<InventoryUI>
{
    [Header("Panel Descripcion")]
    [SerializeField] private GameObject panelDescription;
    [SerializeField] private TextMeshProUGUI textDescription;
    [Header("PanelPickup")]
    [SerializeField] private GameObject panelPickup;

    [Space]

    [SerializeField] private MonoBehaviour playerController;    
    [SerializeField] private GameObject panelInventory;

    [Space]

    [SerializeField] private Slot_Inventory slot;
    [SerializeField] private Transform container;

    private Coroutine typingCoroutine;

    List<Slot_Inventory> availableSlots = new List<Slot_Inventory>();

    public bool IsInventoryOpen => panelInventory.activeSelf == true;

    private void Start()
    {
        InitializeInventory();
    }

    void Update()
    {
        // Toggle inventario (sin verificar panelPickup, eso lo maneja GameStateManager)
        if (Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.Z))
        {
            ToggleInventory();
        }
    }

    private void ToggleInventory()
    {
        bool willOpen = !panelInventory.activeSelf;
        panelInventory.SetActive(willOpen);

        if (willOpen)
        {
            GameStateManager.Instance.LockPlayer(priority: 2); // Prioridad alta
        }
        else
        {
            GameStateManager.Instance.UnlockPlayer(priority: 2);
        }
    }

    private void InitializeInventory()
    {
        for (int i = 0; i < Inventory.Instance.Numberslot; i++)
        {
            Slot_Inventory newSlot = Instantiate(slot, container);
            newSlot.Index = i;
            availableSlots.Add(newSlot);

        }
    }

    public void DrawItemInInventory(Inventory_Item itemToAdd, int amount, int itemIndex) //Dibujar item en el inventario
    {
        Slot_Inventory slot = availableSlots[itemIndex];
        if(itemToAdd != null)
        {
            slot.ActivateSlotUI(true);
            slot.UpdateSlot(itemToAdd, amount);
        }
        else
        {
            slot.ActivateSlotUI(false);
        }
    }

    //Es la que esta en funcionamiento ahora
    #region Description panel - La otra forma - 
    /*basicamente lo que hace esta region
     * es mostrar el panel de descripcion de los item
     * cuando el cursor pasa por arriba de los slots,
     * tiene una corrutina para mostrar la descripcion tipo maquina de escribir
     */
    public void ShowItemDescription(int index)
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        string description = "Empty";

        if (Inventory.Instance.Items[index] != null)
        {
            description = Inventory.Instance.Items[index].Description;
        }

        panelDescription.SetActive(true);
        textDescription.text = ""; // limpiar antes de mostrar
        typingCoroutine = StartCoroutine(TypeText(description));
    }

    public void HideItemDescription()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        panelDescription.SetActive(false);
        textDescription.text = "";
    }

    private IEnumerator TypeText(string fullText)
    {
        textDescription.text = "";
        foreach (char c in fullText)
        {
            textDescription.text += c;
            yield return new WaitForSeconds(0.05f); // velocidad del tipeo
        }
    }
    #endregion
}
