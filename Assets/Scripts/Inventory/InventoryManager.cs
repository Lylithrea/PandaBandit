using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[Serializable]
public class InventoryItem
{
    public SO_Item item;
    public int amount;

    public InventoryItem(InventoryItem item)
    {
        if (item == null)
        {
            this.item = null;
            this.amount = 0;
        }
        else
        {
            this.item = item.item;
            this.amount = item.amount;
        }

    }
    public InventoryItem(SO_Item item, int amount)
    {
        if (item == null)
        {
            this.item = null;
            this.amount = 0;
        }
        else
        {
            this.item = item;
            this.amount = amount;
        }

    }
}

public class InventoryManager : MonoBehaviour
{

    [SerializeField] public GameObject inventoryDragParent;
    public GraphicRaycaster raycaster;
    public GameObject draggableItem;

    public List<GameObject> slots = new List<GameObject>();
    public Dictionary<SlotManager, InventoryItem> inventorySlots = new Dictionary<SlotManager, InventoryItem>();

    protected InventoryData localInventoryData;

    public InventoryData inventoryData {
        get { return GetInventoryData(); }
    }

    
    /// <summary>
    /// Returns the correct inventory, as this is different per inherented class.
    /// </summary>
    /// <returns></returns>
    protected virtual InventoryData GetInventoryData()
    {
        if (localInventoryData == null)
        {
            localInventoryData = new InventoryData(gameObject.name);
            Debug.LogWarning("Did not have a local inventory data yet for object: " + gameObject.name);
        }
        return localInventoryData;
    }

    // Start is called before the first frame update
    public virtual void Start()
    {
        raycaster = this.GetComponent<GraphicRaycaster>();
        SetupInventory();
    }

    public virtual void SetupInventory()
    {
        inventoryData.savePath = Application.persistentDataPath + "/InventoryData/";
        SetupSlots();
    }

    public virtual void SetupSlots()
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

    public void UpdateAllSlots()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            slots[i].GetComponent<SlotManager>().updateUI();
        }
    }

    public virtual void Update()
    {
    }







    

}
