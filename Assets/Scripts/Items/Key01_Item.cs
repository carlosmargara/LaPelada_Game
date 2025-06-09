using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Item/Key01")]
public class Key01_Item : Inventory_Item
{
    public override bool UseItem()
    {
        Debug.Log("Llave usada. " + Name);
        return true;
    }
}
