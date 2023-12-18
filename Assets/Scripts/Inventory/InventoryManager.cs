using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class InventoryItem
{
    public SO_Item item;
    public int amount;
}

public class InventoryManager : MonoBehaviour
{

    [SerializeField] public GameObject inventoryDragParent;
    private static InventoryManager instance;
    public GraphicRaycaster raycaster;
    public GameObject draggableItem;

    public List<GameObject> layouts = new List<GameObject>();
    public int currentInventoryLayout = 0;

    public List<GameObject> slots = new List<GameObject>();
    private Dictionary<SlotManager, InventoryItem> inventorySlots = new Dictionary<SlotManager, InventoryItem>();

    //temporary storage for drag and drop functionality
    SlotManager currentSlot = null;
    InventoryItem currentItem = null;
    int amount = 0;
    GameObject dragItem = null;

    [BoxGroup("Debug Tooling")]
    public List<SO_Item> items = new List<SO_Item>();

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        raycaster = this.GetComponent<GraphicRaycaster>();
        SetupSlots();
    }

    private void SetupSlots()
    {
        inventorySlots.Clear();
        for(int i = 0; i < slots.Count; i++)
        {
            SlotManager newSlot = slots[i].GetComponent<SlotManager>();
            if (newSlot != null)
            {
                inventorySlots.Add(newSlot, null);
                newSlot.updateUI();
            }
        }
    }

    public static InventoryManager Instance
    {
        get
        {
            return instance;
        }
    }


    public void Update()
    {
        updateDraggable();
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            AddItem(1);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            AddItem(2);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            AddItem(3);
        }
    }


    public void AddItem(int item)
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

    public InventoryItem GetItemFromSlot(SlotManager slot)
    {
        InventoryItem item = null;
        if (inventorySlots.ContainsKey(slot))
        {
            item = inventorySlots[slot];
        }
        return item;
    }

    public void AddItemToSlot(SlotManager slot, InventoryItem item)
    {
        if (inventorySlots.ContainsKey(slot))
        {
            inventorySlots[slot] = item;
            slot.updateUI();
        }

    }

    public void RemoveItemFromSlot(SlotManager slot)
    {
        if (inventorySlots.ContainsKey(slot))
        {
            inventorySlots[slot] = null;
            slot.updateUI();
        }
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

    public void OnClick(SlotManager slot)
    {
        // TODO: Add functionality which clicks
        InventoryItem iventoryItem = GetItemFromSlot(slot);
        if (iventoryItem == null) return;

        Debug.Log("User clicked on a slot, now removing item and creating draggable.");

        currentItem = iventoryItem;
        currentSlot = slot;

        //slot.RemoveItem();
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
        //TODO:
        //Check typing of slot
        // Check if filled with item
        // Check if switachable with item

        //


        Debug.Log("User clicked on a slot, now adding item to old or new slot.");
        if (newSlot == null)
        {
            Debug.Log("User clicked on a non slot item, now returning to old slot");
            //move it back to old slot
            //currentSlot.AddItem(currentItem, amount);
            AddItemToSlot(currentSlot, currentItem);
            currentSlot.updateUI();
            ResetItem();
        }
        else
        {
            //need to do more check later
            //newSlot.AddItem(currentItem, amount);
            AddItemToSlot(newSlot, currentItem);
            newSlot.updateUI();
            ResetItem();
        }
    }

    private void DestroyDragItem()
    {
        if (dragItem == null) return;
        Destroy(dragItem);
        dragItem = null;
        isDraggingItem = false;
    }

    #endregion

    private void ResetItem()
    {
        currentSlot = null;
        currentItem = null;
        amount = 0;
        DestroyDragItem();
    }


}
