using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SlotManager : MonoBehaviour
{

    public ItemVariant variant;

    public GameObject itemObject;
    public TextMeshProUGUI amountText;

    public int amount = 0;
    public int slotID = 0;

    private InventoryManager linkedInventory;

    public void Setup(InventoryManager inventoryManager)
    {
        Debug.Log("Linking with inventory: " + inventoryManager);
        linkedInventory = inventoryManager;
    }


    public void updateUI()
    {
        Debug.Log("linked inventory: " + linkedInventory);
        Debug.Log("inventory data: " + linkedInventory.GetInventoryData());
        Debug.Log("item: " + linkedInventory.GetInventoryData().GetItemFromSlot(slotID));
        InventoryItem inventoryItem = linkedInventory.GetInventoryData().GetItemFromSlot(slotID);

        if (inventoryItem != null)
        {
            if (inventoryItem.amount == 0)
            {
                amountText.text = "";
                itemObject.SetActive(false);
                return;
            }

            itemObject.SetActive(true);
            itemObject.GetComponent<Image>().sprite = inventoryItem.item.ItemIcon;
            if (inventoryItem.item.maxStackSize == 1)
            {
                amountText.text = "";
            }
            else
            {
                amountText.text = inventoryItem.amount.ToString();
            }
            amount = inventoryItem.amount;
            return;
        }
        amountText.text = "";
        itemObject.SetActive(false);
        return;

    }

/*    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("User clicked on a slot!");
        PlayerInventoryManager.Instance.HandleClick(this);
    }*/
}

