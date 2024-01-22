using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

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
        Debug.LogWarning("Trying to load inventory: " + fileName);
        if (File.Exists(Application.persistentDataPath + "/InventoryData/" + fileName))
        {
            string json = File.ReadAllText(Application.persistentDataPath + "/InventoryData/" + fileName);
            inventoryItems newDatabase = JsonUtility.FromJson<inventoryItems>(json);
            ConvertDatabaseToItems(newDatabase);
        }
    }

    private void ConvertDatabaseToItems(inventoryItems database)
    {
        ClearInventoryData();
        foreach (inventoryItem item in database.items)
        {
            InventoryItem newInvItem = new InventoryItem(ItemDatabase.GetItem(item.itemID), item.amount);
            inventoryData[item.slot] = newInvItem;
            Debug.Log("Converted an item on slot: " + item.slot + " to item: " + newInvItem.item);
        }

    }

    private void ClearInventoryData()
    {
        for (int i = 0; i < inventoryData.Count; i++)
        {
            inventoryData[i] = null;
        }
    }


    public void PrepareToSaveInventory()
    {
        Debug.LogWarning("Trying to save inventory: " + fileName);


        inventoryItems newDatabase = new inventoryItems();
        //newDatabase.items = new inventoryItem[inventoryData.Count];

        List<inventoryItem> newItems = new List<inventoryItem>();

        for(int i =  0; i < inventoryData.Count; i++)
        {
            if (inventoryData[i] != null)
            {
                inventoryItem newItem = new inventoryItem(i, ItemDatabase.GetItemID(inventoryData[i].item), inventoryData[i].amount);
                newItems.Add(newItem);
                Debug.Log("Prepared item to save: " + inventoryData[i].item + " on slot: " + i);
            }
        }

        newDatabase.items = new inventoryItem[newItems.Count];
        newDatabase.items = newItems.ToArray();
        SaveInventoryDataToJson(newDatabase);
    }




    public void SaveInventoryDataToJson(inventoryItems inventory)
    {
        CheckIfPathExists("/InventoryData/");
        //string json = JsonUtility.ToJson(inventoryData);

        string json = JsonUtility.ToJson(inventory);
        Debug.Log("Trying to save: " + json);
        File.WriteAllText(Application.persistentDataPath + "/InventoryData/" + fileName, json);
    }

    private void CheckIfPathExists(string path)
    {
        Debug.Log("Path: " + Application.persistentDataPath + path);
        // Create the directory if it doesn't exist
        if (!Directory.Exists(Application.persistentDataPath + path))
        {
            Directory.CreateDirectory(Application.persistentDataPath + path);
        }
    }



}


public class Test
{
    public string title;
}

[Serializable]
public class inventoryItems
{
    public inventoryItem[] items;
}

[Serializable]
public class inventoryItem
{
    public int slot;
    public int itemID;
    public int amount;

    public inventoryItem(int inventorySlot, int itemID, int amount)
    {
        slot = inventorySlot;
        this.itemID = itemID;
        this.amount = amount;
    }
}