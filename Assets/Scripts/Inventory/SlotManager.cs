using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SlotManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    public ItemVariant variant;

    public GameObject itemObject;
    public GameObject starObject;
    public TextMeshProUGUI amountText;

    public int amount = 0;
    public int slotID = 0;

    private InventoryManager linkedInventory;

    public Color standard;
    public Color onHover;

    public void Setup(InventoryManager inventoryManager)
    {
        Debug.Log("Linking with inventory: " + inventoryManager);
        linkedInventory = inventoryManager;
        this.GetComponent<Image>().color = standard;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        this.GetComponent<Image>().color = onHover;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        this.GetComponent<Image>().color = standard;
    }

    public void SetStar(bool active)
    {
        starObject.SetActive(active);
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
            if (!inventoryItem.item.hasDiscovered)
            {
                SetStar(true);
            }


            return;
        }
        amountText.text = "";
        itemObject.SetActive(false);
        starObject.SetActive(false);
        return;

    }


}

