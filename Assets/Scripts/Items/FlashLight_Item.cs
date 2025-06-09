using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Item/FlashLight")]
public class FlashLight_Item : Inventory_Item
{
    public override bool IsEquipped => FlashlightSystem.Instance.IsEquipped;

    public override bool EquipItem()
    {
        bool newState = !FlashlightSystem.Instance.IsEquipped;
        FlashlightSystem.Instance.SetEquipped(newState);

        if (newState)
            Debug.Log("Linterna equipada. Podés usarla con F.");
        else
            Debug.Log("Linterna desequipada.");

        return true;
    }
}
