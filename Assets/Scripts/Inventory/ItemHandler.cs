using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ItemHandler : MonoBehaviour
{

    public SO_Item item;
    public int amount;

    public Image itemIcon;
    public TextMeshProUGUI itemAmount;

    private SlotManager currentSlot;
    private Vector3 currentPosition;

    GameObject slotItem;

    public void UpdateUI()
    {
        itemIcon.sprite = item.ItemIcon;

        itemAmount.text = amount.ToString();
        if (item.maxStackSize == 1) itemAmount.text = "";
    }

    public void SetItem(InventoryItem item)
    {
        this.item = item.item;
        this.amount = item.amount;
    }

    private void HandleNewSlot(SlotManager newSlot)
    {
        if (newSlot == currentSlot)
        {
            //currentSlot.AddItem()
            Destroy(slotItem.gameObject);
            return;
        }
        //see if we can move the item at all, if not we dont do anything
        if (newSlot.variant == item.variant || newSlot.variant == ItemVariant.None)
        {
            //see if the slot is null, if so just simply move it in there
            if (newSlot.item == null)
            {
                Debug.Log("New slot is empty so we can move it!");
                currentSlot.RemoveItemAmount(amount);
                newSlot.SetItem(item, amount);
            }
            //if the new slot is the same item, stack it up, any left overs stay in the original slot
            else if (newSlot.item == item)
            {
                int maxMoveCount = newSlot.item.maxStackSize - newSlot.currentAmount;
                int leftover = maxMoveCount - amount;
                Debug.Log("The new slot is the same item");
                //if leftover is lower, we have too much items to move
                if (leftover < 0)
                {
                    Debug.Log("We have too many items to move, but we full stack the new slot");
                    //we have to many we wanna move
                    currentSlot.RemoveItemAmount(amount - maxMoveCount);
                    newSlot.AddItem(item, maxMoveCount);
                }
                else
                {
                    Debug.Log("We have less than max stack amount of items, moving everything");
                    //we can move everything
                    newSlot.AddItem(item, amount);
                    currentSlot.RemoveItem();
                }

            }
            else
            {
                //we need to move the full slot amount if we wanna switch
                if (amount == currentSlot.currentAmount) 
                {
                    //can the new slot item go into our current slot?
                    if (newSlot.item.variant == currentSlot.variant || currentSlot.variant == ItemVariant.None)
                    { 
                        
                        //if we got here we can swap them
                        Debug.Log("We can swap the items!");
                        SO_Item swapItem = newSlot.item;
                        int swapAmount = newSlot.currentAmount;

                        newSlot.RemoveItem();
                        newSlot.AddItem(item, amount);

                        currentSlot.RemoveItem();
                        currentSlot.AddItem(swapItem, swapAmount);
                    }
                    else
                    {
                        Debug.Log("The item we want to swap does not adhere to our slot variant");
                    }
                }
                else
                {
                    Debug.Log("We didnt move all items");
                }

            }
        }

        Destroy(slotItem.gameObject);
        //always put image back to original slot
        this.transform.SetParent(currentSlot.gameObject.transform);
        this.gameObject.transform.position = currentPosition;
    }



}
