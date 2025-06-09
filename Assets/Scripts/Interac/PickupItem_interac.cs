using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupItem_interac : Interactable
{
    [SerializeField] public Inventory_Item Ref_ScriptableObject;
    [SerializeField] private int amountToAdd = 1;

    public override void Interact()
    {
        if (Inventory.Instance != null)
        {
            PickupUIManager.Instance.ShowPickupPrompt(this);
        }
    }

    public void Pickup()
    {
        if (Inventory.Instance != null)
        {
            Inventory.Instance.AddItem(Ref_ScriptableObject, amountToAdd);
            Debug.Log("Recogiste (Pickup): " + Ref_ScriptableObject.Name);
        }
    }
}