using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public enum InteractionType
{
    Click,
    Use,
    Equip,
    Remove
}

public class Slot_Inventory : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    public static Action<InteractionType, int> SlotInteractionEvent; //Siempre de declaramos un evento es "public static" y en este caso ademas tiene
                                                                     //dos parametros

    [SerializeField] private Image itemIcono;
    [SerializeField] private TextMeshProUGUI amountTMP;

    public int Index { get; set; }

    public void UpdateSlot(Inventory_Item item, int amount)
    {
        itemIcono.sprite = item.IsEquipped ? item.Icono_equipped : item.Icono_default;

        // Solo mostrar la cantidad si el item es acumulable
        if (item.isCumulative)
        {
            amountTMP.text = amount.ToString();
            amountTMP.gameObject.SetActive(true);
        }
        else
        {
            amountTMP.gameObject.SetActive(false);
        }
    }

    public void ActivateSlotUI(bool state)
    {
        itemIcono.gameObject.SetActive(state);
        amountTMP.gameObject.SetActive(state);
    }

    public void ClickSlot() //Se llama cuando confirmas con mouse
    {
        SlotInteractionEvent?.Invoke(InteractionType.Click, Index);
        Debug.Log("Lanzando el evento de tipo Click");
    }

    // --- EVENTOS DE MOUSE ---
    public void OnPointerEnter(PointerEventData eventData)
    {
        InventoryUI.Instance.ShowItemDescription(Index);
        AudioManager02.Instance.PlayOneShot("event:/UI/Selection_Sound (Inventary)");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        InventoryUI.Instance.HideItemDescription();
    }


    // --- NUEVOS EVENTOS DE NAVEGACIÓN (TECLADO / GAMEPAD) ---
    public void OnSelect(BaseEventData eventData)
    {
        // Cuando el slot es seleccionado con teclado o joystick
        InventoryUI.Instance.ShowItemDescription(Index);
        AudioManager02.Instance.PlayOneShot("event:/UI/Selection_Sound (Inventary)");
    }

    public void OnDeselect(BaseEventData eventData)
    {
        // Cuando se cambia de slot
        InventoryUI.Instance.HideItemDescription();
    }
}


