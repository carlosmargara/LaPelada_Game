using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class Inventory : Singleton<Inventory>
{
    [SerializeField] private int numberSlot;
    public int Numberslot => numberSlot;

    [Header("Items")]
    [SerializeField] private Inventory_Item[] items;
    public Inventory_Item[] Items => items; //propiedad publica que me devuelve los resultados del array de items

    private void Start()
    {
        items = new Inventory_Item[numberSlot]; //le dijo al array la cantidad de item que hay, numero fijo porque es un array 
    }
    
    public void AddItem(Inventory_Item itemToAdd, int amount)
    {
        if (itemToAdd == null)
        {
            return;
        }

        List<int> indexes = CheckStock(itemToAdd.ID);
        if(itemToAdd.isCumulative) //se puede acumular 
        {
            if(indexes.Count > 0)
            {
                for (int i = 0; i < indexes.Count; i++)
                {
                    if (items[indexes[i]].amount < itemToAdd.maxiAccumulation)
                    {
                        items[indexes[i]].amount += amount;
                        if(items[indexes[i]].amount > itemToAdd.maxiAccumulation)
                        {
                            int difference = items[indexes[i]].amount - itemToAdd.maxiAccumulation;
                            items[indexes[i]].amount = itemToAdd.maxiAccumulation;
                            AddItem(itemToAdd, difference);
                        }

                        InventoryUI.Instance.DrawItemInInventory(itemToAdd, items[indexes[i]].amount, indexes[i]);
                        return;
                    }
                }
            }
        }
        
        if (amount <= 0)
        {
            return ;
        }
        
        if (amount > itemToAdd.maxiAccumulation)
        {
            AddItemToAvailableSlot(itemToAdd, itemToAdd.maxiAccumulation);
            amount -= itemToAdd.maxiAccumulation;
            AddItem(itemToAdd, amount);
        }
        else
        {
            AddItemToAvailableSlot(itemToAdd, amount);
        }
    }

    private  List<int> CheckStock(string itemID) //Verificar existencias 
    {
        List<int> indexesResult = new List<int>();
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] != null)
            {
                if (items[i].ID == itemID)
                {
                    indexesResult.Add(i);
                }
            }
        }        
        return indexesResult;
    }
    
    //Este metodo lo que hace es agregar un item a un slot basio
    private void AddItemToAvailableSlot(Inventory_Item item, int amount) //añadir item en slot disponible
    {
        for(int i = 0;i < items.Length;i++)
        {
            if (items[i] == null) //si el item es null quiere decir que el slot esta disponible
            {
                items[i] = item.CopyItem();
                items[i].amount = amount;
                InventoryUI.Instance.DrawItemInInventory(item, amount, i);
                return; //Salimos al terminar porque al ser un ciclo for sino llenaria todos los slot de ese item
            }
        }
    }

    #region Evento: Escucha el evento lanzado por Slot_Inventory
    private void OnEnable()
    {
        Slot_Inventory.SlotInteractionEvent += HandleSlotInteraction;
    }

    private void OnDisable()
    {
        Slot_Inventory.SlotInteractionEvent -= HandleSlotInteraction;
    }

    private void HandleSlotInteraction(InteractionType type, int index)
    {
        if (type != InteractionType.Click) return;

        Inventory_Item item = Items[index];
        if (item == null) return; //Se fija que haya un item en el slot que clikie, sino sale 

        ItemInteractionManager.Instance.InteractWithItem(item, index);
    }
    #endregion

}
