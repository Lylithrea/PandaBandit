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

    private string savePath = Application.persistentDataPath + "/InventoryData/";

    public void AddItemToSlot(int slotIndex, InventoryItem item)
    {
        if (inventoryData.ContainsKey(slotIndex))
        {
            Debug.Log("Added an item to slot: " + slotIndex + " with item: " + item + " in inventory: " + fileName);
            inventoryData[slotIndex] = item;
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
        Debug.Log("Getting item from slot: " + slotIndex + " with inventory: " + fileName);
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

    public void LoadInventoryDataFromJson()
    {
        Debug.LogWarning("Loading inventory still needs to be implemented!");
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            inventoryData = JsonUtility.FromJson<Dictionary<int, InventoryItem>>(json);
        }
    }

    public void SaveInventoryDataToJson()
    {
        Debug.LogWarning("Saving inventory still needs to be implemented!");
        string json = JsonUtility.ToJson(inventoryData);
        File.WriteAllText(savePath, json);
    }




}
