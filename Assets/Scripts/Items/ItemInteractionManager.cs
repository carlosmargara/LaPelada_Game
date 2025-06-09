using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInteractionManager : Singleton<ItemInteractionManager>
{
    public void InteractWithItem(Inventory_Item item, int index)
    {
        if (item == null) return;

        switch (item.TypeItem)
        {
            case TypeItem.Use:
                // Verifica si estamos en zona de llave
                if (KeyUseZone.PlayerIsInKeyZone && item.ID == KeyUseZone.CurrentKeyZoneID)
                {
                    if (item.UseItem())
                    {
                        if (item.isConsumable)
                        {
                            item.amount--;
                            if (item.amount <= 0)
                            {
                                Inventory.Instance.Items[index] = null;
                            }
                        }
                        else 
                        {
                            item.amount--;
                            if(item.amount <= 0)
                            {
                                Inventory.Instance.Items[index] = null;
                                InventoryUI.Instance.DrawItemInInventory(null , 0 , index);
                                Debug.Log("Llave usada correctamente en la zona.");
                                return;
                            }
                        }

                        InventoryUI.Instance.DrawItemInInventory(item, item.amount, index);
                        Debug.Log("Llave usada correctamente en la zona.");
                    }
                    else
                    {
                        Debug.Log("La llave no se pudo usar.");
                    }
                }
                else
                {
                    Debug.Log("No podés usar ese ítem acá.");
                }
                break;

            case TypeItem.Equip:
                if (item.EquipItem()) //aca al llamar al EquipItem ya estoy equipando la linterna -- Ver clase "FlashLight_Item" --
                {
                    Debug.Log("Has equipado el Item: " + item.Name);
                    InventoryUI.Instance.DrawItemInInventory(item, item.amount, index);

                    // Es solo un Debug informativo 
                    if (item is FlashLight_Item)
                    {
                        Debug.Log("La linterna ha sido equipada y ahora se puede usar con F.");
                    }
                }
                break;
        }
    }
}

