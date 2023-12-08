using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;

public class SlotManager : MonoBehaviour
{

    public ItemVariant variant;
    public SO_Item item;
    public int currentAmount;
    public Transform iconSlotTransform;
    public GameObject itemObject;
    public TextMeshProUGUI amountText;


    public void Start()
    {
        //itemObject.GetComponent<ItemHandler>().Setup(item, currentAmount, this);
        updateUI();
    }

    public void RemoveItem(SO_Item newItem, int amount)
    {
        currentAmount -= amount;
        if (currentAmount == 0)
        {
            item = null;
        }
        if (currentAmount < 0)
        {
            Debug.LogWarning("You somehow managed to remove more items than the slot contains...");
        }
    }

    public void SetItem(SO_Item item, int amount)
    {
        this.item = item;
        this.currentAmount = amount;
        updateUI();
    }

    public void AddItem(SO_Item newItem, int amount)
    {
        item = newItem;
        this.currentAmount += amount;
        updateUI();
    }

    public void RemoveItemAmount(int amount)
    {
        currentAmount -= amount;
        if (currentAmount == 0)
        {
            item = null;
        }
        updateUI();
    }

    public void RemoveItem()
    {
        item = null;
        currentAmount = 0;
        updateUI();
    }

    private void updateUI()
    {
        if (item == null || currentAmount == 0)
        {
            amountText.text = "";
            itemObject.SetActive(false);
        }
        else
        {
            itemObject.SetActive(true);
            itemObject.GetComponent<Image>().sprite = item.ItemIcon;
            if (item.maxStackSize == 1)
            {
                amountText.text = "";
            }
            else
            {
                amountText.text = currentAmount.ToString();
            }
            //itemObject.GetComponentInChildren<Image>().sprite = item.ItemIcon;
        }
    }


}

