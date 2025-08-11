using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupItem_interac : Interactable
{
    private DiffetentTypes_footSteps_with_FmodEvent footSteps_Player;
    [SerializeField] public Inventory_Item Ref_ScriptableObject;
    [SerializeField] private int amountToAdd = 1;

    void Start()
    {
        footSteps_Player = FindObjectOfType<DiffetentTypes_footSteps_with_FmodEvent>();
    }

    public override void Interact()
    {
        if (Inventory.Instance != null)
        {
            PickupUIManager.Instance.ShowPickupPrompt(this);
            footSteps_Player.StopAllFootsteps();
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