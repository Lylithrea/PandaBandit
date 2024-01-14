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
}

public class InventoryManager : MonoBehaviour
{

    [SerializeField] public GameObject inventoryDragParent;
    public GraphicRaycaster raycaster;
    public GameObject draggableItem;

    public List<GameObject> layouts = new List<GameObject>();
    public int currentInventoryLayout = 0;

    public List<GameObject> slots = new List<GameObject>();
    public Dictionary<SlotManager, InventoryItem> inventorySlots = new Dictionary<SlotManager, InventoryItem>();



    



    // Start is called before the first frame update
    void Start()
    {
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



    public virtual void Update()
    {
    }





    

}
