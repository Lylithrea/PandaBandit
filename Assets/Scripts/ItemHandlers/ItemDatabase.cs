using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
using System;
using Unity.VisualScripting;
using static UnityEditor.Timeline.Actions.MenuPriority;
using static UnityEditor.Progress;
using System.Linq;

public static class ItemDatabase
{
    //private static Dictionary<int, string> savedItems = new Dictionary<int, string>();
    private static Dictionary<int, S_ItemID> savedItems = new Dictionary<int, S_ItemID>();
    private static Dictionary<SO_Item, int> ItemIDs = new Dictionary<SO_Item, int>();
    private static Dictionary<int, SO_Item> IDItems = new Dictionary<int, SO_Item>();

    private static List<SO_Item> changedItems = new List<SO_Item>();
    
    //private static List<S_ItemID> itemsToSave = new List<S_ItemID>();
    private static List<S_ItemID> foundItems = new List<S_ItemID>();

    public static void LoadItems()
    {
        Debug.Log("Loading items...");
        LoadItemDatabase();
        GenerateIDs();
    }

    public static void UpdateItems()
    {
        PrepareToSave();
        GenerateItems();
    }


    public static SO_Item GetItem(int itemID)
    {
        //SO_Item item = AssetDatabase.LoadAssetAtPath<SO_Item>( AssetDatabase.GUIDToAssetPath(savedItems[itemID]));
        if (IDItems.ContainsKey(itemID)) return IDItems[itemID];
        Debug.Log("item is not saved...");
        return null;
    }

    public static int GetItemID(SO_Item item)
    {
        if(ItemIDs.ContainsKey(item)) return ItemIDs[item];

        Debug.Log("item is not saved...");
        return 0;
    }

    public static void SaveItemDatabase()
    {
        Debug.Log("Saving database...");
        GenerateIDs();
    }
    private static void GenerateIDs()
    {
        Debug.Log("Generating ID's...");

        LoadItemDatabase();
        GatherAllItems();
        PrepareToSave();
        GenerateItems();
    }

    private static void GenerateItems()
    {
        ItemIDs.Clear();
        IDItems.Clear();
        for(int i = 0; i < savedItems.Count; i++)
        {
            SO_Item currentItem = AssetDatabase.LoadAssetAtPath<SO_Item>(AssetDatabase.GUIDToAssetPath(savedItems[i].itemPath));
            ItemIDs.Add(currentItem, savedItems[i].itemID);
            IDItems.Add(savedItems[i].itemID, currentItem);
        }
    }

    private static void GatherAllItems()
    {
        Debug.Log("Gathering Items...");
        string[] guids = AssetDatabase.FindAssets("t:SO_Item", new[] { "Assets/Assets/Items" });

        foundItems.Clear();
        foreach (string item in guids)
        {
            SO_Item newItem2 = AssetDatabase.LoadAssetAtPath<SO_Item>(AssetDatabase.GUIDToAssetPath(item));
            S_ItemID newItem = new S_ItemID(newItem2.itemID, item, false);
            foundItems.Add(newItem);
        }
    }

    private static void PrepareToSave()
    {
        Debug.Log("Preparing to save...");

        for (int i = 0; i < foundItems.Count; i++)
        {
            Debug.Log("Processing item: " + foundItems[i].itemPath + " ...");

            CheckItemDuplication(foundItems[i]);
        }

        SaveDatabase();
    }

    private static void SaveDatabase()
    {
        Debug.Log("Saving Items...");
        CheckIfPathExists("/Items/");
        S_ItemDatabase database = new S_ItemDatabase();
        database.items = savedItems.Values.ToArray();
        string json = JsonUtility.ToJson(database);

        File.WriteAllText(Application.persistentDataPath + "/Items/ItemDatabase.json", json);
    }


    private static void CheckIfPathExists(string path)
    {
        // Create the directory if it doesn't exist
        if (!Directory.Exists(Application.persistentDataPath + path))
        {
            Directory.CreateDirectory(Application.persistentDataPath + path);
        }
    }


    private static void CheckItemDuplication(S_ItemID item)
    {
        Debug.Log("Current item: " + item.itemPath + " with id: " + item.itemID);

        if (!savedItems.ContainsKey(item.itemID) && !savedItems.ContainsValue(item))
        {
            Debug.Log("<color=green>Item ID Info: </color> Item was not saved yet.");
            int id = savedItems.Count;
            item.itemID = id;
            savedItems.Add(id, item);
        }
        else if (savedItems.ContainsKey(item.itemID) && !savedItems.ContainsValue(item))
        {
            Debug.Log("<color=orange>Item ID Warning: </color>Item was saved with same id, but different paths.");
        }
        else if (!savedItems.ContainsKey(item.itemID) && savedItems.ContainsValue(item))
        {
            Debug.Log("<color=red>Item ID Error: </color>Item was saved twice under different item ids.");
        }
        else
        {
            Debug.Log("<color=gray>Item ID Info: </color> Item was saved with correct id and path.");
        }

    }

    private static void LoadItemDatabase()
    {
        Debug.Log("Loading Items from Json...");
        if (File.Exists(Application.persistentDataPath + "/Items/ItemDatabase.json"))
        {
            string json = File.ReadAllText(Application.persistentDataPath + "/Items/ItemDatabase.json");
            S_ItemDatabase database = JsonUtility.FromJson<S_ItemDatabase>(json);
            DeloadSavedItems(database);
        }
    }

    private static void DeloadSavedItems(S_ItemDatabase database)
    {
        Debug.Log("Loading existing items...");
        savedItems.Clear();
        if (database == null) return;
        if (database.items == null) return;
        for (int i = 0; i < database.items.Length; i++)
        {
            Debug.Log("Deloading item: " + AssetDatabase.GUIDToAssetPath(database.items[i].itemPath) + " with id: " + database.items[i].itemID);
            SO_Item newItem2 = AssetDatabase.LoadAssetAtPath<SO_Item>(AssetDatabase.GUIDToAssetPath(database.items[i].itemPath));
            newItem2.itemID = database.items[i].itemID;
            savedItems.Add(database.items[i].itemID, database.items[i]);
        }
    }

}



[Serializable]
public class S_ItemDatabase
{
    public S_ItemID[] items;
}

[Serializable]
public class S_ItemID
{
    public int itemID;
    public string itemPath;
    public bool hasDiscovered;

    public S_ItemID(int id, string path, bool hasDiscovered)
    {
        itemID = id;
        itemPath = path;
        this.hasDiscovered = hasDiscovered;
    }

}