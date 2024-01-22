using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SlotManager : MonoBehaviour, IPointerEnterHandler
{

    public ItemVariant variant;

    public GameObject itemObject;
    public GameObject starObject;
    public TextMeshProUGUI amountText;

    public int amount = 0;
    public int slotID = 0;

    private InventoryManager linkedInventory;

    public void Setup(InventoryManager inventoryManager)
    {
        Debug.Log("Linking with inventory: " + inventoryManager);
        linkedInventory = inventoryManager;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        starObject.SetActive(false);
    }

    public void updateUI()
    {
        InventoryItem inventoryItem = linkedInventory.inventoryData.GetItemFromSlot(slotID);

        if (inventoryItem != null)
        {
            if (inventoryItem.amount == 0)
            {
                amountText.text = "";
                itemObject.SetActive(false);
                starObject.SetActive(false);
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
            this.gameObject.GetComponent<Animator>().SetTrigger("ItemUpdate");
            starObject.SetActive(true);
            Debug.LogWarning("TODO: Add functionality so that the star only shows with new items.");
            return;
        }
        amountText.text = "";
        itemObject.SetActive(false);
        starObject.SetActive(false);
        return;

    }


}

