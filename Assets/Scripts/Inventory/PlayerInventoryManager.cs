using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.EventSystems;

public class PlayerInventoryManager : InventoryManager
{

    [BoxGroup("Debug Tooling")]
    public List<SO_Item> items = new List<SO_Item>();

    #region Start-Up

    public override void SetupInventory()
    {
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


    protected override InventoryData GetInventoryData()
    {
        return PlayerInventory.Instance.GetInventory();
    }


    #endregion

    public override void Update()
    {
        base.Update();


        if (Input.GetKeyDown(KeyCode.Keypad4))
        {
            InventoryInputManager.Instance.LinkInventory(this);
        }
/*        if (Input.GetKeyDown(KeyCode.Keypad5))
        {
            inventoryData.PrepareToSaveInventory();
        }
        if (Input.GetKeyDown(KeyCode.Keypad6))
        {
            inventoryData.LoadInventoryDataFromJson();
            UpdateAllSlots();
        }*/

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


    /// <summary>
    /// Temporary method to add items to the inventory
    /// </summary>
    /// <param name="item"></param>
    public void AddItemTemp(int item)
    {
        Dictionary<int, InventoryItem> playerInventory = PlayerInventory.Instance.GetInventory().inventoryData;

        for (int i = 0; i < playerInventory.Count; i++)
        {
            if (playerInventory[i] == null)
            {
                InventoryItem newItem = new InventoryItem(null);
                newItem.item = items[item - 1];
                newItem.amount = items[item - 1].maxStackSize;
                PlayerInventory.instance.GetInventory().AddItemToSlot(i, newItem);
                slots[i].GetComponent<SlotManager>().updateUI();
                break;
            }
        }
    }



}
