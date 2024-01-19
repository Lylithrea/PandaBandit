using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.EventSystems;

public class PlayerInventoryManager : InventoryManager
{
    private static PlayerInventoryManager instance;

    private InventoryManager linkedInventory = null;

    //temporary storage for drag and drop functionality
    private bool isDraggingItem = false;
    SlotManager currentSlot = null;
    InventoryItem currentItem = new InventoryItem(null);
    GameObject dragItem = null;

    public static PlayerInventoryManager Instance
    {
        get
        {
            return instance;
        }
    }

    #region Start-Up
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

    public override void SetupInventory()
    {
        inventoryData = PlayerInventory.Instance.inventory;
        inventoryData.savePath = Application.persistentDataPath + "/InventoryData/";
        SetupSlots();
    }

    public override void SetupSlots()
    {
        inventorySlots.Clear();
        //create inventory data slots, this should later be when a file does exist or not
        inventoryData.AddSlots(slots.Count);
        for (int i = 0; i < slots.Count; i++)
        {
            SlotManager newSlot = slots[i].GetComponent<SlotManager>();
            if (newSlot != null)
            {
                inventorySlots.Add(newSlot, null);
                slots[i].GetComponent<SlotManager>().slotID = i;
                slots[i].GetComponent<SlotManager>().Setup(this);
                newSlot.updateUI();
            }
        }
    }


    public override InventoryData GetInventoryData()
    {
        return inventoryData;
    }


    #endregion

    public override void Update()
    {
        base.Update();
        updateDraggable();



        if (Input.GetKeyDown(KeyCode.Keypad4))
        {
            InventoryInputManager.Instance.LinkInventory(this);
        }
        if (Input.GetKeyDown(KeyCode.Keypad5))
        {
            //inventoryData.SaveInventoryDataToJson();
        }
        if (Input.GetKeyDown(KeyCode.Keypad6))
        {
            //inventoryData.LoadInventoryDataFromJson();
        }

    }

/*    /// <summary>
    /// Temporary method to add items to the inventory
    /// </summary>
    /// <param name="item"></param>
    public void AddItemTemp(int item)
    {
        foreach (KeyValuePair<SlotManager, InventoryItem> pair in inventorySlots)
        {
            if (pair.Value == null || pair.Value.amount == 0)
            {
                InventoryItem newItem = new InventoryItem(null);
                newItem.item = items[item - 1];
                newItem.amount = items[item - 1].maxStackSize;
                inventorySlots[pair.Key] = newItem;
                pair.Key.updateUI();
                break;
            }
        }


    }*/



    #region Tooling

    /// <summary>
    /// Overwrites the slot data, and sets the item
    /// </summary>
    /// <param name="slot"></param>
    /// <param name="item"></param>
    public void SetItemToSlot(SlotManager slot, InventoryItem item)
    {
        InventoryManager validInventory = GetValidInventory(slot);
        if (validInventory != null)
        {
            validInventory.inventorySlots[slot] = item;
            slot.updateUI();
        }
    }

    /// <summary>
    /// Adds item to slot, will take max stack size into consideration.
    /// If the max stack size is reached, any left over will be put into the orignal position
    /// </summary>
    /// <param name="slot"></param>
    /// <param name="item"></param>
    /// <param name="amount"></param>
    public void AddItemToSlot(SlotManager slot, InventoryItem item, int amount)
    {
        InventoryManager validInventory = GetValidInventory(slot);
        if (validInventory != null)
        {
            InventoryItem checkItem = GetItemFromSlot(slot);
            int newStack = amount;
            if (checkItem != null)
            {
                newStack += checkItem.amount;
            }
            if (newStack > item.item.maxStackSize)
            {
                int remainingAmount = newStack - item.item.maxStackSize;
                InventoryItem newItem = new InventoryItem(item);
                newItem.amount = item.item.maxStackSize;
                SetItemToSlot(slot, newItem);
                InventoryItem oldItem = new InventoryItem(item);
                oldItem.amount = remainingAmount;
                AddItemToSlot(currentSlot, oldItem, remainingAmount);
            }
            else
            {
                currentItem.amount = newStack;
                SetItemToSlot(slot, currentItem);
            }
            ResetItem();
        }

    }

    /// <summary>
    /// Gets the correct inventory from the slot
    /// </summary>
    /// <param name="slot"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Gets item from slot from correct inventory if it isnt null
    /// </summary>
    /// <param name="slot"></param>
    /// <returns></returns>
    public InventoryItem GetItemFromSlot(SlotManager slot)
    {
        InventoryManager validInventory = GetValidInventory(slot);
        if (validInventory != null)
        {
            InventoryItem item = validInventory.inventorySlots[slot];
            return item;
        }

        return null;
    }

    /// <summary>
    /// Removes item from slot, and leaves the amount correct or null if amount is 0 or below
    /// </summary>
    /// <param name="slot"></param>
    /// <param name="amount"></param>
    /// <returns></returns>
    public InventoryItem RemoveItemFromSlot(SlotManager slot, int amount)
    {
        InventoryManager validInventory = GetValidInventory(slot);
        
        if (validInventory != null)
        {
            InventoryItem removedItem = new InventoryItem(GetItemFromSlot(slot));
            removedItem.amount -= amount;
            //if amount is 0 or below (shouldnt be) then set item to null
            if (removedItem.amount <= 0)
            {
                validInventory.inventorySlots[slot] = null;
                slot.updateUI();
                return null;
            }
            else
            {
                //update item to new amount
                InventoryItem updatedItem = new InventoryItem(GetItemFromSlot(slot));
                updatedItem.amount -= amount;
                validInventory.inventorySlots[slot] = updatedItem;
                slot.updateUI();
                return removedItem;
            }
        }
        return null;
    }

    /// <summary>
    /// Resets the temporary data for the drag and drop function
    /// </summary>
    private void ResetItem()
    {
        currentSlot = null;
        currentItem = null;
        DestroyDragItem();
    }

    #endregion

    #region MouseInputs

    /// <summary>
    /// Handles left click functionalities
    /// </summary>
    public void OnLeftClick()
    {
        SlotManager slot = GetSlotUnderMouse();
        if (slot == null) return;

        if (!isDraggingItem)
        {
            InventoryItem iventoryItem = GetItemFromSlot(slot);
            if (iventoryItem == null) return;

            currentItem = new InventoryItem(iventoryItem);
            currentSlot = slot;
            SetItemToSlot(slot, null);
            slot.updateUI();
            CreateDraggable();
        }
        else
        {
            OnRelease(slot);
        }

    }

    /// <summary>
    /// Handles right click functionalities
    /// </summary>
    public void OnRightClick()
    {
        SlotManager slot = GetSlotUnderMouse();
        if (slot == null) return;

        if (!isDraggingItem)
        {
            InventoryItem iventoryItem = GetItemFromSlot(slot);
            if (iventoryItem == null) return;
            InventoryItem remainder = RemoveItemFromSlot(slot, Mathf.CeilToInt(iventoryItem.amount / 2f));
            currentItem = new InventoryItem(iventoryItem);
            currentItem.amount = Mathf.CeilToInt(iventoryItem.amount / 2f);
            currentSlot = slot;
            CreateDraggable();
        }
        else
        {
            OnRelease(slot);
        }
    }

    /// <summary>
    /// Gets all slots under the mouse
    /// </summary>
    /// <returns></returns>
    public List<SlotManager> GetSlotsUnderMouse()
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(pointerEventData, results);
        List<SlotManager> uiElements = new List<SlotManager>();
        foreach (var result in results)
        {
            // Filter for specific script (CustomScriptName)
            SlotManager customScript = result.gameObject.GetComponent<SlotManager>();
            if (customScript != null)
            {
                uiElements.Add(customScript);
            }
        }
        return uiElements;
    }
    
    /// <summary>
    /// Gets the first slot under the mouse
    /// </summary>
    /// <returns></returns>
    public SlotManager GetSlotUnderMouse()
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(pointerEventData, results);

        foreach (var result in results)
        {
            // Filter for specific script (CustomScriptName)
            SlotManager customScript = result.gameObject.GetComponent<SlotManager>();
            if (customScript != null)
            {
                return customScript;
            }
        }
        return null;
    }

    #endregion

    #region InteractionHandlers

    /// <summary>
    /// Creates a draggable
    /// </summary>
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

    /// <summary>
    /// Updates the position of the draggable to the mouse position
    /// </summary>
    private void updateDraggable()
    {
        if (dragItem == null) return;
        dragItem.transform.position = Input.mousePosition;
    }

    /// <summary>
    /// On Mouse click release handler. Handles the item that is being dragged going into the slot.
    /// </summary>
    /// <param name="newSlot"></param>
    public void OnRelease(SlotManager newSlot = null)
    {
        if (newSlot == null)
        {
            ResetToOldPosition();
        }
        else
        {
            if (!IsValidItemType(newSlot, currentItem)) { ResetToOldPosition(); return; }

            if (!isSlotTaken(newSlot)) return;

            if (isSameItem(newSlot)) return;

            if (isSwitchable(newSlot)) return;

            //if it gets here, it should be moved to its original position
            ResetToOldPosition();
        }
    }

    /// <summary>
    /// Check if its possible to switch the 2 items in the slots
    /// </summary>
    /// <param name="newSlot"></param>
    /// <returns></returns>
    private bool isSwitchable(SlotManager newSlot)
    {
        //see if old position is empty, as its impossible to switch when old slot still contains items
        if (GetItemFromSlot(currentSlot) == null)
        {
            //check if the item currently in the new slot can go into the old slot
            if (IsValidItemType(currentSlot, GetItemFromSlot(newSlot)))
            {
                SetItemToSlot(currentSlot, GetItemFromSlot(newSlot));
                SetItemToSlot(newSlot, currentItem);
                ResetItem();   
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Check if the items are the same, if so, add the new item to the slot
    /// </summary>
    /// <param name="newSlot"></param>
    /// <returns></returns>
    private bool isSameItem(SlotManager newSlot)
    {
        if (GetItemFromSlot(newSlot).item == currentItem.item)
        {
            //Additemtoslot does take maxstack amount into consideration
            AddItemToSlot(newSlot, currentItem, currentItem.amount);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Check if the slot is taken, if it isnt then move our item into the slot
    /// </summary>
    /// <param name="newSlot"></param>
    /// <returns></returns>
    private bool isSlotTaken(SlotManager newSlot)
    {
        InventoryItem slotItem = GetItemFromSlot(newSlot);
        if (slotItem == null || slotItem.amount <= 0)
        { 
            SetItemToSlot(newSlot, currentItem);
            ResetItem();
            return false;
        }
        return true;
    }

    /// <summary>
    /// Check if the current item can be moved into the slot
    /// </summary>
    /// <param name="newSlot"></param>
    /// <param name="itemToCheck"></param>
    /// <returns></returns>
    private bool IsValidItemType(SlotManager newSlot, InventoryItem itemToCheck)
    {
        if (itemToCheck == null)
        {
            return false;
        }
        if (newSlot.variant != ItemVariant.None && newSlot.variant != itemToCheck.item.variant)
        {
            return false;
        }
        return true;
    }

    /// <summary>
    /// Reset the item to its original slot, either sets it if its empty, or adds it if its the same item
    /// </summary>
    private void ResetToOldPosition()
    {
        if (GetItemFromSlot(currentSlot) != null)
        {
            AddItemToSlot(currentSlot, currentItem, currentItem.amount);
        }
        else
        {
            SetItemToSlot(currentSlot, currentItem);
        }
        ResetItem();
    }

    /// <summary>
    /// Destroys the drag item
    /// </summary>
    private void DestroyDragItem()
    {
        if (dragItem == null) return;
        Destroy(dragItem);
        dragItem = null;
        isDraggingItem = false;
    }

    #endregion

    #region LinkingInventory
    /// <summary>
    /// Links inventory to player inventory.
    /// </summary>
    /// <param name="inventory"></param>
    public void LinkInventory(InventoryManager inventory)
    {
        linkedInventory = inventory;
        Debug.Log("Player inventory linked with inventory: " + inventory);
    }

    /// <summary>
    /// Unlinks inventory from player inventory
    /// </summary>
    public void UnlinkInventory()
    {
        linkedInventory = null;
        Debug.Log("Player inventory unlinked");
    }
    #endregion





}
