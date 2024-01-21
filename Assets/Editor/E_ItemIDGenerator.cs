using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using System.Linq;
using Unity.VisualScripting;

public class E_ItemIDGenerator : EditorWindow
{

    public Dictionary<int, string> savedItems = new Dictionary<int, string>();
    //public Dictionary<int, string> itemsToSave = new Dictionary<int, string>();
    List<S_ItemID> itemsToSave = new List<S_ItemID>();
    public List<string> foundItems = new List<string>();


    [MenuItem("Window/Item ID Generator")]
    public static void ShowWindow()
    {
        GetWindow<E_ItemIDGenerator>("Item ID Generator");
    }

    public void OnGUI()
    {
        GUILayout.Label("To generate new item ID's, it will fill empty ID slots, and only add items that aren't included yet.");
        if (GUILayout.Button("Generate ID's"))
        {
            GenerateIDs();
        }
    }

    public void GenerateIDs()
    {
        Debug.Log("Generating ID's...");
        GatherAllItems();
        LoadInventoryDataFromJson();
        PrepareToSave();
    }

    public void GatherAllItems()
    {
        Debug.Log("Gathering Items...");
        string[] guids = AssetDatabase.FindAssets("t:SO_Item", new[] { "Assets/Assets/Items" });

        foundItems.Clear();
        foreach (string item in guids)
        {
            foundItems.Add(item);
        }
    }

    private void PrepareToSave()
    {
        Debug.Log("Preparing to save...");
        S_ItemDatabase newItems = new S_ItemDatabase();

        

        //newItems.items = new ItemID[foundItems.Count];



        for (int i = 0; i < foundItems.Count; i++)
        {
            Debug.Log("Processing item: " + AssetDatabase.GUIDToAssetPath(foundItems[i]) + " ...");


            S_ItemID newItem = new S_ItemID(i, foundItems[i]);
            CheckItemDuplication(newItem);
        }


        S_ItemID[] items2 = itemsToSave.ToArray();
        newItems.items = new S_ItemID[items2.Length];
        newItems.items = items2;

        //newItems.items.AddRange(itemsToSave);
        SaveInventoryDataToJson(newItems);
    }

    private void CheckItemDuplication(S_ItemID item)
    {
        if(!savedItems.ContainsKey(item.itemID) && !savedItems.ContainsValue(item.itemPath))
        {
            Debug.Log("<color=green>Item ID Info: </color> Item was not saved yet.");
            itemsToSave.Add(new S_ItemID(itemsToSave.Count, item.itemPath));
        }
        else if (savedItems.ContainsKey(item.itemID) && !savedItems.ContainsValue(item.itemPath))
        {
            Debug.Log("<color=orange>Item ID Warning: </color>Item was saved with same id, but different paths.");
        }
        else if (!savedItems.ContainsKey(item.itemID) && savedItems.ContainsValue(item.itemPath))
        {
            Debug.Log("<color=red>Item ID Error: </color>Item was saved twice under different item ids.");
        }
        else
        {
            Debug.Log("<color=gray>Item ID Info: </color> Item was saved with correct id and path.");
        }

    }


    public void SaveInventoryDataToJson(S_ItemDatabase items)
    {
        Debug.Log("Saving Items...");
        CheckIfPathExists("/Items/");

        string json = JsonUtility.ToJson(items);

        File.WriteAllText(Application.persistentDataPath + "/Items/ItemDatabase", json);
    }

    private void LoadInventoryDataFromJson()
    {
        Debug.Log("Loading Items from Json...");
        if (File.Exists(Application.persistentDataPath + "/Items/ItemDatabase"))
        {
            string json = File.ReadAllText(Application.persistentDataPath + "/Items/ItemDatabase");
            S_ItemDatabase database = JsonUtility.FromJson<S_ItemDatabase>(json);
            DeloadSavedItems(database);
        }
    }

    private void CheckIfPathExists(string path)
    {
        // Create the directory if it doesn't exist
        if (!Directory.Exists(Application.persistentDataPath + path))
        {
            Directory.CreateDirectory(Application.persistentDataPath + path);
        }
    }

    private void DeloadSavedItems(S_ItemDatabase database)
    {
        Debug.Log("Loading existing items...");
        savedItems.Clear();
        itemsToSave.Clear();
        if (database == null) return;
        for (int i = 0; i < database.items.Length; i++)
        {
            Debug.Log("Deloading item: " + AssetDatabase.GUIDToAssetPath(database.items[i].itemPath) + " ...");
            savedItems.Add(database.items[i].itemID, database.items[i].itemPath);
        }
        itemsToSave.AddRange(database.items);
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