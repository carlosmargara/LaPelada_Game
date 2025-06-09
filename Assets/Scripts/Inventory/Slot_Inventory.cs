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

public class Slot_Inventory : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
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
    
    public void ClickSlot()
    {
        SlotInteractionEvent?.Invoke(InteractionType.Click, Index);
        Debug.Log("Lanzando el evento de tipo Click");
        /*
        {
            if (SlotInteractionEvent != null)
            {
                SlotInteractionEvent.Invoke(InteractionType.Click, Index);
            }
        }
        */
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        InventoryUI.Instance.ShowItemDescription(Index); 
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        InventoryUI.Instance.HideItemDescription(); 
    }
}
