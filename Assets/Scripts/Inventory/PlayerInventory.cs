using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory instance;
    public InventoryData inventory = new InventoryData("playerInventory");
    public int inventorySize = 10;


    public ref InventoryData GetInventory()
    {
        return ref inventory;
    }

    public static PlayerInventory Instance
    {
        get
        {
            return instance;
        }
    }

    void singletonCreation()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    public void Awake()
    {
        singletonCreation();
    }

    public void Start()
    {
        inventory = new InventoryData("playerInventory");
        inventory.AddSlots(inventorySize);
    }



}
