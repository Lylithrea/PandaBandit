using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class PlayerInventoryManager : InventoryManager
{
    private static PlayerInventoryManager instance;

    private InventoryManager linkedInventory = null;

    [BoxGroup("Debug Tooling")]
    public List<SO_Item> items = new List<SO_Item>();



    //temporary storage for drag and drop functionality
    SlotManager currentSlot = null;
    InventoryItem currentItem = null;
    int amount = 0;
    GameObject dragItem = null;

    public static PlayerInventoryManager Instance
    {
        get
        {
            return instance;
        }
    }

    void singletonCreation()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    public void Awake()
    {
        singletonCreation();
    }




    public override void Update()
    {
        base.Update();
        updateDraggable();

        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            AddItemTemp(1);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            AddItemTemp(2);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            AddItemTemp(3);
        }
    }

    public void AddItemTemp(int item)
    {
        foreach (KeyValuePair<SlotManager, InventoryItem> pair in inventorySlots)
        {
            if (pair.Value == null || pair.Value.amount == 0)
            {
                InventoryItem newItem = new InventoryItem();
                newItem.item = items[item - 1];
                newItem.amount = items[item - 1].maxStackSize;
                inventorySlots[pair.Key] = newItem;
                pair.Key.updateUI();
                Debug.Log("Added an item! : " + newItem.item.ItemName + " with amount: " + newItem.amount);
                break;
            }
        }

    }



    #region Tooling

    public void AddItemToSlot(SlotManager slot, InventoryItem item)
    {
        InventoryManager validInventory = GetValidInventory(slot);
        if (validInventory != null)
        {
            validInventory.inventorySlots[slot] = item;
            slot.updateUI();
        }

    }

    public InventoryManager GetValidInventory(SlotManager slot)
    {
        if (inventorySlots.ContainsKey(slot))
        {
            return this;
        }
        else if (linkedInventory == null || linkedInventory.inventorySlots.ContainsKey(slot))
        {
            return linkedInventory;
        }
        return null;
    }

    public InventoryItem GetItemFromSlot(SlotManager slot)
    {

        InventoryManager validInventory = GetValidInventory(slot);
        if (validInventory != null)
        {
            InventoryItem item = validInventory.inventorySlots[slot];
            Debug.Log("Got item from slot! item: " + item + " from slot: " + slot);
            return item;
        }

        return null;
    }

    public void RemoveItemFromSlot(SlotManager slot)
    {
        InventoryManager validInventory = GetValidInventory(slot);
        if (validInventory != null)
        {
            validInventory.inventorySlots[slot] = null;
            slot.updateUI();
        }
    }

    /// <summary>
    /// Resets the temporary data for the drag and drop function
    /// </summary>
    private void ResetItem()
    {
        currentSlot = null;
        currentItem = null;
        amount = 0;
        DestroyDragItem();
    }

    #endregion

    #region InteractionHandlers

    private bool isDraggingItem = false;

    public void HandleClick(SlotManager slot)
    {
        //handle if we already had clicked on a slot before
        if (!isDraggingItem)
        {
            OnClick(slot);
        }
        else
        {
            OnRelease(slot);
        }
    }

    public void OnClick(SlotManager slot, ClickVariant variant)
    {
        // TODO: Add functionality which clicks
        InventoryItem iventoryItem = GetItemFromSlot(slot);
        if (iventoryItem == null) return;

        currentItem = iventoryItem;
        currentSlot = slot;

        RemoveItemFromSlot(slot);
        slot.updateUI();
        CreateDraggable();
    }


    private void CreateDraggable()
    {
        if (dragItem != null)
        {
            DestroyDragItem();
        }
        dragItem = Instantiate(draggableItem, inventoryDragParent.transform);
        dragItem.GetComponent<ItemHandler>().SetItem(currentItem);
        dragItem.GetComponent<ItemHandler>().UpdateUI();
        Debug.Log("Created a draggable item.");
        isDraggingItem = true;
    }

    private void updateDraggable()
    {
        if (dragItem == null) return;
        dragItem.transform.position = Input.mousePosition;
    }

    public void OnRelease(SlotManager newSlot = null)
    {
        if (newSlot == null)
        {
            //move it back to old slot
            ResetToOldPosition();
        }
        else
        {
            //check if slot is valid for item type
            if (!IsValidItemType(newSlot, currentItem)) return;

            //check if its empty
            // add item to slot
            if (!isSlotTaken(newSlot)) return;

            //is it same item?
            // add item to slot until max stacks
            // remaining item amount back to same slot
            if (isSameItem(newSlot)) return;

            //can we switch the items around?
            if (isSwitchable(newSlot)) return;

            //move it to new slot
            ResetToOldPosition();
        }
    }

    private bool isSwitchable(SlotManager newSlot)
    {
        //see if old position is empty
        if (GetItemFromSlot(currentSlot) == null)
        {
            //check if the item currently in the new slot can go into the old slot
            if(IsValidItemType(currentSlot, GetItemFromSlot(currentSlot)))
            {
                AddItemToSlot(currentSlot, GetItemFromSlot(newSlot));
                AddItemToSlot(newSlot, currentItem);
                ResetItem();
                return true;
            }
            return false;
        }
        return false;
    }

    private bool isSameItem(SlotManager newSlot)
    {
        if (GetItemFromSlot(newSlot) == currentItem)
        {
            InventoryItem item = GetItemFromSlot(newSlot);
            int newStack = item.amount + currentItem.amount;

            if (newStack > item.item.maxStackSize)
            {
                int remainingAmount = newStack - item.item.maxStackSize;
                item.amount = item.item.maxStackSize;
                AddItemToSlot(newSlot, item);
                InventoryItem newItem = item;
                newItem.amount = remainingAmount;
                AddItemToSlot(currentSlot, newItem);
            }
            else
            {
                currentItem.amount = newStack;
                AddItemToSlot(newSlot, currentItem);
            }
            ResetItem();
            return true;
        }
        return false;
    }



    private bool isSlotTaken(SlotManager newSlot)
    {
        InventoryItem slotItem = GetItemFromSlot(newSlot);
        if (slotItem == null || slotItem.amount <= 0)
        {
            AddItemToSlot(newSlot, currentItem);
            ResetItem();
            return false;
        }
        return true;
    }


    private bool IsValidItemType(SlotManager newSlot, InventoryItem itemToCheck)
    {
        Debug.Log("Check item type with slot: " + newSlot + " and item: " + itemToCheck);
        if (newSlot.variant != ItemVariant.None && newSlot.variant != itemToCheck.item.variant)
        {
            return false;
        }
        return true;
    }

    private void ResetToOldPosition()
    {
        AddItemToSlot(currentSlot, currentItem);
        ResetItem();
    }

    private void DestroyDragItem()
    {
        if (dragItem == null) return;
        Destroy(dragItem);
        dragItem = null;
        isDraggingItem = false;
    }

    #endregion


    #region LinkingInventory
    public void LinkInventory(InventoryManager inventory)
    {
        linkedInventory = inventory;
        Debug.Log("Player inventory linked with inventory: " + inventory);
    }

    public void UnlinkInventory()
    {
        linkedInventory = null;
        Debug.Log("Player inventory unlinked");
    }
    #endregion





}

public enum ClickVariant
{
    Left,
    Right,
    ShiftLeft,
    ShiftRight
}
