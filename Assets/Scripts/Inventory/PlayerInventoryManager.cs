using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.EventSystems;

public class PlayerInventoryManager : InventoryManager
{
    private static PlayerInventoryManager instance;

    private InventoryManager linkedInventory = null;

    [BoxGroup("Debug Tooling")]
    public List<SO_Item> items = new List<SO_Item>();



    //temporary storage for drag and drop functionality
    SlotManager currentSlot = null;
    InventoryItem currentItem = new InventoryItem(null);
    int amount = 0;
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

    public void OnEnable()
    {
        InputManager.Instance.onLeftClick += OnLeftClick;
        InputManager.Instance.onRightClick += OnRightClick;
    }

    public void OnDisable()
    {
        InputManager.Instance.onLeftClick -= OnLeftClick;
        InputManager.Instance.onRightClick -= OnRightClick;
    }

    #endregion

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
                InventoryItem newItem = new InventoryItem(null);
                newItem.item = items[item - 1];
                newItem.amount = items[item - 1].maxStackSize;
                inventorySlots[pair.Key] = newItem;
                pair.Key.updateUI();
                break;
            }
        }

    }



    #region Tooling

    public void SetItemToSlot(SlotManager slot, InventoryItem item)
    {
        InventoryManager validInventory = GetValidInventory(slot);
        if (validInventory != null)
        {
            validInventory.inventorySlots[slot] = item;
            slot.updateUI();
        }
    }

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
            return item;
        }

        return null;
    }

    public InventoryItem RemoveItemFromSlot(SlotManager slot, int amount)
    {
        InventoryManager validInventory = GetValidInventory(slot);
        
        if (validInventory != null)
        {
            InventoryItem removedItem = new InventoryItem(GetItemFromSlot(slot));
            removedItem.amount -= amount;
            if (removedItem.amount <= 0)
            {
                validInventory.inventorySlots[slot] = null;
                slot.updateUI();
                return null;
            }
            else
            {
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
        amount = 0;
        DestroyDragItem();
    }

    #endregion

    #region MouseInputs

    public void OnLeftClick()
    {
        SlotManager slot = GetSlotUnderMouse();
        if (slot == null) return;

        if (!isDraggingItem)
        {
            OnClick(slot);
        }
        else
        {
            OnRelease(slot);
        }

    }

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

    private bool isDraggingItem = false;

    public void OnClick(SlotManager slot)
    {
        InventoryItem iventoryItem = GetItemFromSlot(slot);
        if (iventoryItem == null) return;

        currentItem = new InventoryItem(iventoryItem);
        currentSlot = slot;
        SetItemToSlot(slot, null);
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
            if (!IsValidItemType(newSlot, currentItem)) 
            {
                //move it to old slot
                if (GetItemFromSlot(currentSlot) != null)
                {
                    AddItemToSlot(currentSlot, currentItem, currentItem.amount);
                    ResetItem();
                }
                else
                {
                    SetItemToSlot(currentSlot, currentItem);
                    ResetItem();
                }
                return;
            }

            //check if its empty
            // add item to slot
            if (!isSlotTaken(newSlot)) return;

            //is it same item?
            // add item to slot until max stacks
            // remaining item amount back to same slot
            if (isSameItem(newSlot)) return;

            //can we switch the items around?
            if (isSwitchable(newSlot)) return;

            //move it to old slot
            if (GetItemFromSlot(currentSlot) != null)
            {
                AddItemToSlot(currentSlot, currentItem, currentItem.amount);
                ResetItem();
            }
            else
            {
                SetItemToSlot(currentSlot, currentItem);
                ResetItem();
            }

        }
    }

    private bool isSwitchable(SlotManager newSlot)
    {
        //see if old position is empty
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
            return false;
        }
        return false;
    }

    private bool isSameItem(SlotManager newSlot)
    {
        if (GetItemFromSlot(newSlot).item == currentItem.item)
        {
            AddItemToSlot(newSlot, currentItem, currentItem.amount);
            return true;
        }
        return false;
    }

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

    private void ResetToOldPosition()
    {
        SetItemToSlot(currentSlot, currentItem);
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
