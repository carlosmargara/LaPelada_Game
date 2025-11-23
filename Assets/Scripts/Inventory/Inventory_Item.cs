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

    [Header("Localización")]
    public LocalizedString Description;
    public LocalizedString pickupText;
    public LocalizedString pickupText02;
    public LocalizedString confirmationText;

    [Header("Info")]
    public TypeItem TypeItem;
    public bool isConsumable;
    public bool isCumulative;
    public int maxiAccumulation;

    public virtual bool IsEquipped => false;

    [HideInInspector] public int amount;

    public Inventory_Item CopyItem()
    {
        Inventory_Item newInstance = Instantiate(this);
        return newInstance;
    }

    public virtual bool UseItem()
    {
        return true;
    }

    public virtual bool EquipItem()
    {
        return true;
    }

    // --- MÉTODOS PARA OBTENER LOS TEXTOS LOCALIZADOS ---
    public string GetDescription() => Description.GetValue();
    public string GetPickupText() => pickupText.GetValue();
    public string GetPickupText02() => pickupText02.GetValue();
    public string GetConfirmationText() => confirmationText.GetValue();
}
