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

    private Dictionary<SlotManager, InventoryItem> inventorySlots = new Dictionary<SlotManager, InventoryItem>();
    private Dictionary<SlotManager, InventoryItem> equipmentSlots = new Dictionary<SlotManager, InventoryItem>();

    SlotManager currentSlot = null;
    SO_Item currentItem = null;
    int amount = 0;
    GameObject dragItem = null;


    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        raycaster = this.GetComponent<GraphicRaycaster>();
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
    }

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

        if (slot.item == null) return;

        Debug.Log("User clicked on a slot, now removing item and creating draggable.");

        currentItem = slot.item;
        amount = slot.currentAmount;
        currentSlot = slot;

        slot.RemoveItem();
        CreateDraggable();
    }


    private void CreateDraggable()
    {
        if (dragItem != null)
        {
            DestroyDragItem();
        }
        dragItem = Instantiate(draggableItem, inventoryDragParent.transform);
        dragItem.GetComponent<ItemHandler>().SetItem(currentItem, amount);
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
            currentSlot.AddItem(currentItem, amount);
            ResetItem();
        }
        else
        {
            //need to do more check later
            newSlot.AddItem(currentItem, amount);
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

    private void ResetItem()
    {
        currentSlot = null;
        currentItem = null;
        amount = 0;
        DestroyDragItem();
    }


}
