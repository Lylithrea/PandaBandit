using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
using System;

public static class ItemDatabase
{
    private static Dictionary<int, string> savedItems = new Dictionary<int, string>();
    private static Dictionary<SO_Item, int> ItemIDs = new Dictionary<SO_Item, int>();

    public static void LoadItems()
    {
        Debug.Log("Loading items...");
        LoadItemDatabase();
    }

    public static SO_Item GetItem(int itemID)
    {
        SO_Item item = AssetDatabase.LoadAssetAtPath<SO_Item>( AssetDatabase.GUIDToAssetPath(savedItems[itemID]));
        return item;
    }

    public static int GetItemID(SO_Item item)
    {
        if(ItemIDs.ContainsKey(item)) return ItemIDs[item];

        Debug.Log("item is not saved...");
        return 0;
    }


    private static void LoadItemDatabase()
    {
        Debug.Log("Loading Items from Json...");
        if (File.Exists(Application.persistentDataPath + "/Items/ItemDatabase"))
        {
            string json = File.ReadAllText(Application.persistentDataPath + "/Items/ItemDatabase");
            S_ItemDatabase database = JsonUtility.FromJson<S_ItemDatabase>(json);
            DeloadSavedItems(database);
        }
    }

    private static void DeloadSavedItems(S_ItemDatabase database)
    {
        Debug.Log("Loading existing items...");
        savedItems.Clear();
        if (database == null) return;
        for (int i = 0; i < database.items.Length; i++)
        {
            Debug.Log("Deloading item: " + AssetDatabase.GUIDToAssetPath(database.items[i].itemPath) + " ...");
            savedItems.Add(database.items[i].itemID, database.items[i].itemPath);

            if (GetItem(database.items[i].itemID) != null)
            {
                if (!ItemIDs.ContainsKey(GetItem(database.items[i].itemID)))
                {
                    //incase we have multiple items with same id, we only add them once
                    ItemIDs.Add(GetItem(database.items[i].itemID), database.items[i].itemID);
                }
            }

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

    public S_ItemID(int id, string path)
    {
        itemID = id;
        itemPath = path;
    }

}