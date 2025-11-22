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

        // 🔥 Si este ítem ya fue recogido en esta partida → desaparecer del mundo
        if (GameStateManager.Instance.IsItemPicked(Ref_ScriptableObject.ID))
        {
            Destroy(gameObject);
            return;
        }
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