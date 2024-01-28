using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using OpenCover.Framework.Model;
using Unity.VisualScripting;

public class InventoryInputManager : MonoBehaviour
{
    private List<InventoryManager> linkedInventories = new List<InventoryManager>();

    private static InventoryInputManager instance;

    //temporary storage for drag and drop functionality
    private bool isDraggingItem = false;
    SlotManager currentSlot = null;
    InventoryItem currentItem = new InventoryItem(null);
    GameObject dragItem = null;
    public GraphicRaycaster raycaster;
    [SerializeField] public GameObject inventoryDragParent;
    public GameObject draggableItem;

    public static InventoryInputManager Instance
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
        ItemDatabase.LoadItems();
    }

    #endregion

    private SlotManager firstSlot = null;
    private SlotManager firstDragSlot = null;
    private List<SlotManager> hoveredSlots = new List<SlotManager>();


    #region InputManagement

    private void ManageLeftClickUp()
    {
        if (InputManager.Instance.ShiftPressed)
        {
            //No item-> All items in next valid inv slot
            //Item, empty slot -> Place items in slot
            //Item, taken slot -> Move taken slot items to old inventory slot if possible, and place this in the slot
            Debug.Log("Handling Shift Left Click...");
            HandleShiftLeftClickUp();
            //If multiple stored then divide items equally over all slots
            //If 1 slot is stored and mouse isnt over it anymore -> Dont do anything
        }
        else
        {
            //No item -> All items of slot in hand
            //Item, empty slot -> Place items in slot
            //Item, taken slot -> Place items in slot, take taken slot items in hand
            Debug.Log("Handling Left Click...");
            HandleLeftClickUp();
        }
        firstSlot = null;
        firstDragSlot = null;
    }

    private void HandleShiftLeftClickUp()
    {
        SlotManager mouseSlot = GetSlotUnderMouse();
        if (mouseSlot == null) return;
        if (isDraggingItem)
        {
            //Check if we are still on the same slot
            if (firstSlot == mouseSlot)
            {
                //Check if its a valid inventory and slot variant
                if (!IsValidInventorySlot(firstSlot, currentItem)) return;

                InventoryItem item = GetItemFromSlot(firstSlot);
                if (item == null)
                {
                    //If item is null we move our item into the slot
                    SetItemToSlot(firstSlot, currentItem);
                    ResetDragItem();
                }
                else
                {
                    //If there is an item we want to move into we need to check if we can switch
                    //Check if the slots item is allowed in our old slot
                    if (IsValidInventorySlot(currentSlot, item))
                    {
                        if (GetItemFromSlot(firstDragSlot) == null)
                        {
                            //We can switch both items
                            SetItemToSlot(firstSlot, currentItem);
                            SetItemToSlot(currentSlot, item);
                            ResetDragItem();
                        }

                    }
                }
            }


        }
        else
        {
            //Needs to have more than 1 inventory as else there isnt another inventory to move to
            if (linkedInventories.Count > 1)
            {
                //Handle first possible slot to move to, or most valid slot
                if (firstSlot == mouseSlot)
                {
                    InventoryItem item = GetItemFromSlot(firstSlot);
                    if (item == null) return;
                    SlotManager emptySlot = GetEmptySlotFromInventory(GetValidInventory(firstSlot), item);
                    if (emptySlot == null) return;
                    if (GetItemFromSlot(emptySlot) == null)
                    {
                        SetItemToSlot(emptySlot, item);
                        ClearItemSlot(firstSlot);
                    }
                    else
                    {
                        int value = item.amount;
                        Debug.Log("value " + value);
                        Debug.Log("Adding items to new slot " + item.amount);
                        int leftover = AddItemToSlotNew(emptySlot, item, value);
                        Debug.Log("Left over: " + leftover);
                        Debug.Log("Item Amount 1: " + item.amount);
                        Debug.Log("value Amount 1: " + value);
                        Debug.Log("value calc: " + (value - (value - leftover)));
                        RemoveItemFromSlotNew(firstSlot, value - leftover);
                        Debug.Log("Item Amount 2: " + item.amount);
                        Debug.Log("value Amount 2: " + value);
                        Debug.Log("first slot Amount 2: " + GetItemFromSlot(firstSlot).amount);
                    }

                }
            }
        }
    }

    private void HandleLeftClickUp()
    {
        //No item -> All items of slot in hand

        SlotManager mouseSlot = GetSlotUnderMouse();
        if (mouseSlot == null) return;
        if (isDraggingItem)
        {
            //Item, empty slot -> Place items in slot
            //Item, taken slot -> Place items in slot, take taken slot items in hand

            //Check if our item can go into the slot
            if (!IsValidInventorySlot(firstSlot, currentItem)) return;

            InventoryItem item = GetItemFromSlot(firstSlot);
            if (item == null)
            {
                //if the slot is empty just put our item in
                SetItemToSlot(firstSlot, currentItem);
                ResetDragItem();
            }
            else
            {
                //We pick up the item put it in our hand, and put the our item into the slot
                if (item.item == currentItem.item)
                {
                    int leftover = AddItemToSlotNew(firstSlot, item, currentItem.amount);
                    currentItem.amount -= (currentItem.amount - leftover);
                    UpdateDraggable();
                }
                else
                {
                    InventoryItem tempItem = item;
                    SetItemToSlot(firstSlot, currentItem);
                    currentItem = tempItem;
                    UpdateDraggable();
                }

            }

            //If multiple stored then divide items equally over all slots
            //If 1 slot is stored and mouse isnt over it anymore -> Dont do anything

        }
        else
        {
            //Make sure we are still on the same slot
            if (firstSlot == mouseSlot)
            {
                InventoryItem item = GetItemFromSlot(firstSlot);
                if (item == null) return;
                if (item.amount == 0) return;

                currentItem = new InventoryItem(item);
                currentItem.amount = item.amount;
                currentSlot = firstSlot;

                if (item.item != null)
                {
                    item.item.hasDiscovered = true;
                    ItemDatabase.UpdateItems();
                    firstSlot.SetStar(false);
                }
                ClearItemSlot(firstSlot);
                firstSlot.updateUI();
                CreateDraggable();
            }

        }
    }

    private void ManageLeftClickDown()
    {
        //Get slot under mouse and store it
        //Wait for other things to happen
        firstSlot = GetSlotUnderMouse();
        if (!isDraggingItem)
        {
            firstDragSlot = GetSlotUnderMouse();
        }
        Debug.Log("Left Click Down");
    }

    private void ManageRightClickUp()
    {
        if (InputManager.Instance.ShiftPressed)
        {
            //No item -> One of items in hand (can increase count in hand)
            //Item, empty slot -> Put half stack of items in slot
            //Item, taken slot -> 

            //If multiple stored then add 1 item to all hovered over slots
            //If 1 slot is stored and mouse isnt over it anymore -> Dont do anything
            Debug.Log("Handling Shift Right Click...");
            HandleShiftRightClickUp();
        }
        else
        {
            //No item -> Half of items in hand
            //Item, empty slot -> Put 1 item in slot (can increase with more clicks)
            //Item, taken slot -> 
            Debug.Log("Handling Right Click...");
            HandleRightClickUp();
        }
        firstSlot = null;
        firstDragSlot = null;
    }

    private void HandleShiftRightClickUp()
    {
        SlotManager mouseSlot = GetSlotUnderMouse();
        if (mouseSlot == null) return;
        if (isDraggingItem)
        {
            //Check if our item can go into the slot
            if (!IsValidInventorySlot(firstSlot, currentItem)) return;
            InventoryItem item = GetItemFromSlot(firstSlot);
            if (item == null)
            {
                //if the slot is empty put half of our items in
                int splitAmount = Mathf.CeilToInt((float)currentItem.amount / 2);
                int leftover = AddItemToSlotNew(firstSlot, currentItem, splitAmount);
                currentItem.amount -= splitAmount  - leftover;
                UpdateDraggable();
                
            }
            else
            {
                if (item.item == currentItem.item)
                {
                    //if the slot is same as our item
                    int splitAmount = Mathf.CeilToInt((float)currentItem.amount / 2);
                    int leftover = AddItemToSlotNew(firstSlot, currentItem, splitAmount);
                    currentItem.amount -= splitAmount - leftover;
                    UpdateDraggable();
                }
                else
                {
                    //???
                }
            }
        }
        else
        {
            //No item -> One of items in hand (can increase count in hand)

            //Make sure we are still on the same slot
            if (firstSlot == mouseSlot)
            {
                InventoryItem item = GetItemFromSlot(firstSlot);
                if (item == null) return;

                if (currentItem != null)
                {
                    if (currentItem.item != item.item) return;
                    if (currentItem.amount < item.item.maxStackSize)
                    {
                        currentItem.amount += 1;
                        updateDraggable();
                    }
                }
                else
                {
                    currentItem = new InventoryItem(item);
                    currentItem.amount = 1;
                    currentSlot = firstSlot;
                    CreateDraggable();
                }
                

                if (item.item != null)
                {
                    item.item.hasDiscovered = true;
                    ItemDatabase.UpdateItems();
                    firstSlot.SetStar(false);
                }
                //ClearItemSlot(firstSlot);
                firstSlot.updateUI();
                
            }
        }





    }

    private void HandleRightClickUp()
    {
        //No item -> Half of items in hand
        //Item, empty slot -> Put 1 item in slot (can increase with more clicks)
        //Item, taken slot -> Check if its the same item, if so add 1
        // If not, then ?
        SlotManager mouseSlot = GetSlotUnderMouse();
        if (mouseSlot == null) return;
        if (isDraggingItem)
        {
            //Check if our item can go into the slot
            if (!IsValidInventorySlot(firstSlot, currentItem)) return;

            InventoryItem item = GetItemFromSlot(firstSlot);
            if (item == null)
            {
                //if the slot is empty put half of our items in
                int leftover = AddItemToSlotNew(firstSlot, currentItem, 1);
                currentItem.amount -= 1 - leftover;
                UpdateDraggable();
            }
            else
            {
                //Item, taken slot -> 
                if (item.item == currentItem.item)
                {
                    int leftover = AddItemToSlotNew(firstSlot, currentItem, 1);
                    currentItem.amount -= 1 - leftover;
                    UpdateDraggable();
                }
                else
                {
                    //???
                }
            }
        }
        else
        {
            if (firstSlot == mouseSlot)
            {
                //No item -> One of items in hand (can increase count in hand)
                InventoryItem item = GetItemFromSlot(firstSlot);
                if (item == null) return;

                int splitAmount = Mathf.CeilToInt((float)item.amount / 2);
                int leftover = RemoveItemFromSlotNew(firstSlot, splitAmount);
                currentItem = new InventoryItem(item);
                currentItem.amount = splitAmount - leftover;
                currentSlot = firstSlot;
                firstSlot.updateUI();
                CreateDraggable();
                
                
            }

        }

        //If multiple stored then add 1 item to all hovered over slots
        //If 1 slot is stored and mouse isnt over it anymore -> Dont do anything
    }


    private void ManageRightClickDown()
    {
        //Get slot under mouse and store it
        //Wait for other things to happen
        firstSlot = GetSlotUnderMouse();
        if (!isDraggingItem)
        {
            firstDragSlot = GetSlotUnderMouse();
        }
        Debug.Log("Right Click Down");
    }

    private void DragHandler()
    {
        //Run while when either left or right click is down
        //Update consistently -> only when item in hand
        //Else only once
    }

    #endregion


    public void Update()
    {
        updateDraggable();
        if (Input.GetKeyDown(KeyCode.Keypad5))
        {
            foreach (InventoryManager manager in linkedInventories)
            {
                manager.inventoryData.PrepareToSaveInventory();
            }

        }
        if (Input.GetKeyDown(KeyCode.Keypad6))
        {
            foreach (InventoryManager manager in linkedInventories)
            {
                manager.inventoryData.LoadInventoryDataFromJson();
                manager.UpdateAllSlots();
            }
        }

    }



    public void Start()
    {
        raycaster = this.GetComponent<GraphicRaycaster>();
        InputManager.Instance.onLeftClickDown += ManageLeftClickDown;
        InputManager.Instance.onLeftClickUp += ManageLeftClickUp;
        InputManager.Instance.onRightClickDown += ManageRightClickDown;
        InputManager.Instance.onRightClickUp += ManageRightClickUp;
    }

    public void OnDisable()
    {
        InputManager.Instance.onLeftClickDown -= ManageLeftClickDown;
        InputManager.Instance.onLeftClickUp -= ManageLeftClickUp;
        InputManager.Instance.onRightClickDown -= ManageRightClickDown;
        InputManager.Instance.onRightClickUp -= ManageRightClickUp;
    }


    #region Tooling

    // Data will be set in the data class
    // Data class can be adjusted from outside only methods
    // Visualization goes through inventory manager
    // Inventory links slots to data with numbers


    private SlotManager GetEmptySlotFromInventory(InventoryManager exceptedInventory, InventoryItem item = null)
    {
        if (linkedInventories.Count < 2) return null;
        //Checks for an slot with the same variant and with the same item, and if we can add items to it
        for (int i = 0; i < linkedInventories.Count; i++)
        {
            //Not allowed to run when we do not provide an item
            if (item == null) break;
            if (linkedInventories[i] == exceptedInventory) continue;
            //First check if there is an empty slot free of the variant
            foreach (KeyValuePair<SlotManager, InventoryItem> slot in linkedInventories[i].inventorySlots)
            {
                if (slot.Key.variant != item.item.variant) continue;
                if (slot.Value == null) continue;
                if (GetItemFromSlot(slot.Key) == null) continue;
                if (GetItemFromSlot(slot.Key).item != item.item) continue;
                if (GetItemFromSlot(slot.Key).amount >= item.item.maxStackSize) continue;
                return slot.Key;
            }
        }
        //Check for an empty slot with same item variant
        for (int i = 0; i < linkedInventories.Count; i++)
        {
            if (item == null) break;
            if (item.item.variant == ItemVariant.None) break;
            if (linkedInventories[i] == exceptedInventory) continue;

            //If there isnt, then check if there is an empty slot for the none variant
            foreach (KeyValuePair<SlotManager, InventoryItem> slot in linkedInventories[i].inventorySlots)
            {
                if (slot.Key.variant != item.item.variant) continue;
                if (slot.Value != null) continue;
                return slot.Key;
            }
        }



        //Check for an slot with the same item and variant none
        for (int i = 0; i < linkedInventories.Count; i++)
        {
            if (item == null) break;
            if (linkedInventories[i] == exceptedInventory) continue;

            //If there isnt, then check if there is an empty slot for the none variant
            foreach (KeyValuePair<SlotManager, InventoryItem> slot in linkedInventories[i].inventorySlots)
            {
                if (slot.Key.variant != ItemVariant.None) continue;
                if (slot.Key == null) continue;
                if (GetItemFromSlot(slot.Key) == null) continue;
                if (GetItemFromSlot(slot.Key).item != item.item) continue;
                if (GetItemFromSlot(slot.Key).amount >= item.item.maxStackSize) continue;
                return slot.Key;
            }
        }
        //Check for an empty slot with variant none
        for (int i = 0; i < linkedInventories.Count; i++)
        {
            if (linkedInventories[i] == exceptedInventory) continue;

            //If there isnt, then check if there is an empty slot for the none variant
            foreach (KeyValuePair<SlotManager, InventoryItem> slot in linkedInventories[i].inventorySlots)
            {
                if (slot.Key.variant != ItemVariant.None) continue;
                if (slot.Value != null) continue;
                return slot.Key;
            }
        }
        //if there isnt, then dont do aything
        return null;
    }


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
            //validInventory.inventorySlots[slot] = item;
            validInventory.inventoryData.AddItemToSlot(slot.slotID, item);
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
                //Can cause infinity loop
                AddItemToSlot(currentSlot, oldItem, remainingAmount);
            }
            else
            {
                currentItem.amount = newStack;
                SetItemToSlot(slot, currentItem);
            }
            ResetDragItem();
        }

    }

    /// <summary>
    /// Adds item to a slot, returns 0 if we could add all items, returns the remainder if it reached its max
    /// </summary>
    /// <param name="slot"></param>
    /// <param name="item"></param>
    /// <param name="amount"></param>
    /// <returns></returns>
    private int AddItemToSlotNew(SlotManager slot, InventoryItem item, int amount)
    {
        InventoryItem slotItem = GetItemFromSlot(slot);

        //If slot is empty we can just simply add the amount
        if (slotItem == null)
        {
            slotItem = new InventoryItem(item);
            slotItem.amount = amount;
            SetItemToSlot(slot, slotItem);
            return 0;
        }
       
        //Check if there is an item in the slot that does not match our item, we dont do anything
        if (slotItem.item != item.item) return amount;

        //Else we gotta add the item, but make sure it doesnt go over max count
        if (slotItem.amount >= item.item.maxStackSize) return amount;
        else
        {
            int overload = item.item.maxStackSize - slotItem.amount - amount;
            //if overload is higher than 0, it means we could add all items
            //if overload is below 0 that number is the amount we couldnt add
            if (overload >= 0)
            {
                slotItem.amount += amount;
                SetItemToSlot(slot, slotItem);
                return 0;
            }
            else
            {
                slotItem.amount = item.item.maxStackSize;
                SetItemToSlot(slot, slotItem);
                return Mathf.Abs(overload);
            }
        }
    }



    /// <summary>
    /// Gets the correct inventory from the slot
    /// </summary>
    /// <param name="slot"></param>
    /// <returns></returns>
    public InventoryManager GetValidInventory(SlotManager slot)
    {
        foreach (InventoryManager inventory in linkedInventories)
        {
            if (inventory.slots.Count > slot.slotID)
            {
                if (inventory.slots[slot.slotID] == slot.gameObject)
                {
                    return inventory;
                }
            }
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
            InventoryItem item = validInventory.inventoryData.GetItemFromSlot(slot.slotID);
            Debug.Log("Getting item from inventory " + validInventory + " and slot with id: " + slot.slotID + " with item: " + item);
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
                validInventory.inventoryData.RemoveItemFromSlot(slot.slotID);
                slot.updateUI();
                return null;
            }
            else
            {
                //update item to new amount
                InventoryItem updatedItem = new InventoryItem(GetItemFromSlot(slot));
                updatedItem.amount -= amount;
                validInventory.inventoryData.AddItemToSlot(slot.slotID, updatedItem);
                slot.updateUI();
                return removedItem;
            }
        }
        return null;
    }


    private int RemoveItemFromSlotNew (SlotManager slot, int amount, InventoryItem item = null)
    {
        InventoryItem slotItem = GetItemFromSlot(slot);
        if (slotItem == null) return amount;
        if (item != null)
        {
            if (slotItem.item != item.item)
            {
                Debug.LogWarning("Trying to remove an item from a slot that does not match.");
                return amount;
            }
        }

        int newvalue = slotItem.amount - amount;
        if (newvalue < 0)
        {
            slotItem.amount = 0;
            ClearItemSlot(slot);
            slot.updateUI();
            Debug.LogWarning("Removed more items from slot than the slot contained.");
            return Mathf.Abs(newvalue);
        }

        slotItem.amount -= amount;
        if (slotItem.amount == 0)
        {
            ClearItemSlot(slot);
        }
        slot.updateUI();
        return 0;
        
    }


    public void ClearItemSlot(SlotManager slot)
    {
        InventoryManager validInventory = GetValidInventory(slot);

        if (validInventory != null)
        {
            validInventory.inventoryData.RemoveItemFromSlot(slot.slotID);
            slot.updateUI();
        }
    }


    /// <summary>
        /// Resets the temporary data for the drag and drop function
        /// </summary>
        private void ResetDragItem()
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
        //Handle that if slot underneath is not the same as the initial click dont do anything
        //Except if we hold the mouse button and start moving to other slots

        //When not dragging item, dont do anything on click
        //Do everything on movement


        //Onmouse click -> get slot underneath mouse
        //Check if slots under mouse change while release hasnt been called yet
        //If slots do change while no release, then distribute item over slots
        //On release on same slot just get the item if no other slots have been hit
        //If there is no slot under mouse anymore and no other slot has been hit


        Debug.Log("Input Left Click!" + isDraggingItem);
        SlotManager slot = GetSlotUnderMouse();
        Debug.Log("Slot: " + slot);
        if (slot == null) return;

        if (!isDraggingItem)
        {
            InventoryItem iventoryItem = GetItemFromSlot(slot);
            if (iventoryItem == null) return;

            currentItem = new InventoryItem(iventoryItem);
            currentSlot = slot;

            if (iventoryItem.item != null)
            {
                iventoryItem.item.hasDiscovered = true;
                ItemDatabase.UpdateItems();
                slot.SetStar(false);
            }
            ClearItemSlot(slot);
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
        Debug.Log("Input Right Click!");
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
    /// Gets the first slot under the mouse, if it doesnt find one, it will return null
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

    private void UpdateDraggable()
    {
        if (dragItem == null) return;
        if (currentItem.amount <= 0)
        {
            ResetDragItem();
            return;
        }
        dragItem.GetComponent<ItemHandler>().SetItem(currentItem);
        dragItem.GetComponent<ItemHandler>().UpdateUI();
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
            if (GetValidInventory(newSlot) == null) { ResetToOldPosition(); return; }

            if (!IsValidItemType(newSlot, currentItem)) { ResetToOldPosition(); return; }

            if (!isSlotTaken(newSlot)) return;

            if (isSameItem(newSlot)) return;

            if (isSwitchable(newSlot)) return;

            //if it gets here, it should be moved to its original position
            ResetToOldPosition();
        }
    }

    private bool IsValidInventorySlot(SlotManager slot, InventoryItem item)
    {
        if(GetValidInventory(slot) == null) return false;
        if(!IsValidItemType(slot, item)) return false;
        return true;
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
                ResetDragItem();
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
            ResetDragItem();
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
        ResetDragItem();
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
        linkedInventories.Add(inventory);
        Debug.Log("Inventory linked with inventory: " + inventory);
    }

    /// <summary>
    /// Unlinks inventory from player inventory
    /// </summary>
    public void UnlinkInventory(InventoryManager inventory)
    {
        foreach (InventoryManager linkInventory in linkedInventories)
        {
            if (linkInventory == inventory)
            {
                linkedInventories.Remove(inventory);
                Debug.Log("Player inventory unlinked");
                return;
            }
        }

    }
    #endregion

}
