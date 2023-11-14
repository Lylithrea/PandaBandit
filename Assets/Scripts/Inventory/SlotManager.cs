using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SlotManager : MonoBehaviour
{

    public ItemVariant variant;
    public SO_Item item;
    public int currentAmount;
    public Transform iconSlotTransform;


    //inventory data needs to be serialized eventually

    public void Start()
    {
        if (iconSlotTransform.childCount != 0)
        {
            item = iconSlotTransform.GetComponentInChildren<ItemHandler>().item;
        }
    }

    public void RemoveItem(SO_Item newItem, int amount)
    {
        currentAmount -= amount;
        if (currentAmount == 0)
        {
            item = null;
        }
        if (currentAmount < 0)
        {
            Debug.LogWarning("You somehow managed to remove more items than the slot contains...");
        }
    }


    public bool TryToAddItem(SO_Item newItem, int amount)
    {
        if (variant == ItemVariant.None || variant == newItem.variant)
        {
            if (item == null)
            {
                Debug.Log("Item slot is empty and has matching variant");
                AddItem(newItem);
                return true;
            }
            else if( item == newItem)
            {
                if (item.maxStackSize > currentAmount)
                {
                    //check we can stack all items or part of it
                    Debug.Log("Item slot has the same item and isnt fully stacked yet");
                    AddItem(newItem);
                    return true;
                }
                //switch items?
                Debug.Log("Item slot has the same item but is full");
                return false;
            }
        }
        Debug.Log("The variant does not match");

        return false;
    }

    private void AddItem(SO_Item newItem)
    {
        Debug.Log("Added item");
        item = newItem;
    }


}

