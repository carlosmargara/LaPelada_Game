using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TypeItem
{
    Use,
    Equip
}

public class Inventory_Item : ScriptableObject
{
    [Header("Parametros")]
    public string ID;
    public string Name;
    public Sprite Icono_default;
    public Sprite Icono_equipped;
    public GameObject prefabModel;
    [TextArea] public string Description;

    [Header("PickupPromptPanel")]
    [TextArea] public string pickupText;
    [TextArea] public string pickupText02;
    [TextArea] public string confirmationText;

    [Header("Info")]
    public TypeItem TypeItem;
    public bool isConsumable;
    public bool isCumulative;
    public int maxiAccumulation;

    public virtual bool IsEquipped => false;

    [HideInInspector] public int amount;
    

    public Inventory_Item CopyItem() //Copio el item como una nueva instancia para que tenga un scriptable object distinto
    { 
        Inventory_Item newInstance = Instantiate(this);
        return newInstance;
    }
    
    //public virtual porque luego lo voy a sobre escribir desde otra clase(script)
    public virtual bool UseItem ()
        { return true; }

    public virtual bool EquipItem()
        { return true; }

}
