using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorageInventoryManager : InventoryManager
{


    // Update is called once per frame
    public override void Update()
    {
        base.Update();

        if (Input.GetKeyDown(KeyCode.Keypad0))
        {
            InventoryInputManager.Instance.LinkInventory(this);
        }

    }
}
