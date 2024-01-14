using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ItemHandler : MonoBehaviour
{

    public SO_Item item;
    public int amount;

    public Image itemIcon;
    public TextMeshProUGUI itemAmount;


    public void UpdateUI()
    {
        itemIcon.sprite = item.ItemIcon;

        itemAmount.text = amount.ToString();
        if (item.maxStackSize == 1) itemAmount.text = "";
    }

    public void SetItem(InventoryItem item)
    {
        this.item = item.item;
        this.amount = item.amount;
    }



}
