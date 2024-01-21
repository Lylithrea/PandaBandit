using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class InventoryData
{
    public Dictionary<int, InventoryItem> inventoryData = new Dictionary<int, InventoryItem>();
    public string fileName;

    public InventoryData(string fileName)
    {
        this.fileName = fileName;
        inventoryData = new Dictionary<int, InventoryItem>();
        Debug.Log("New inventory data created with name: " + fileName);
    }

    public string savePath;

    public void AddItemToSlot(int slotIndex, InventoryItem item)
    {
        if (inventoryData.ContainsKey(slotIndex))
        {
            Debug.Log("Added an item to slot: " + slotIndex + " with item: " + item + " in inventory: " + fileName);
            inventoryData[slotIndex] = item;
            Debug.Log("Slot contains now: " + inventoryData[slotIndex]);
            Debug.Log("Item: " + item.item);
            Debug.Log("Amount: " + item.amount);
        }
    }

    public void RemoveItemFromSlot(int slotIndex)
    {
        if (inventoryData.ContainsKey(slotIndex))
        {
            inventoryData[slotIndex] = null;
        }
    }

    public InventoryItem GetItemFromSlot(int slotIndex)
    {
        Debug.Log("Getting item from slot: " + slotIndex + " with filename: " + fileName + " and inventory: " + inventoryData) ;
        if (inventoryData.ContainsKey(slotIndex))
        {
            Debug.Log("Got item from slot: " + inventoryData[slotIndex]);
            return inventoryData[slotIndex];
        }
        return null;
    }

    public Dictionary<int, InventoryItem> GetAllItems()
    {
        return inventoryData;
    }

    public void AddSlots(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            inventoryData.Add(inventoryData.Count, null);
        }
    }

    public void RemoveSlots(int amount)
    {
        Debug.LogWarning("Removing slots still needs to be implemented!");
    }




}
