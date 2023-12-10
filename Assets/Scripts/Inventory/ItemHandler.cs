using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ItemHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{

    public SO_Item item;
    public int amount;

    private SlotManager currentSlot;
    private Vector3 currentPosition;
    InventoryManager inventoryManager;
    GameObject slotItem;
    public void Start()
    {
        if (item != null)
        {
            GetComponent<Image>().sprite = item.ItemIcon;
        }
    }

    public void Setup(SO_Item item, int amount, SlotManager slot)
    {
        this.item = item;
        this.amount = amount;
        currentSlot = slot;
        if (item != null)
        {
            GetComponent<Image>().sprite = item.ItemIcon;
        }

    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("Started dragging");
        
        
        currentSlot = this.gameObject.transform.parent.GetComponent<SlotManager>();
        currentPosition = this.gameObject.transform.position;
        
        
        slotItem = Instantiate(this.gameObject);
        slotItem.transform.position = currentPosition;
        slotItem.transform.SetParent(InventoryManager.Instance.inventoryDragParent.transform);


        //handle different inputs
        item = currentSlot.item;
        amount = currentSlot.currentAmount;

        slotItem.SetActive(false);

    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("Dragging");
        this.transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("Ended dragging");
        CheckForNewSlot();
    }

    private void CheckForNewSlot()
    {
        GraphicRaycaster raycaster = InventoryManager.Instance.raycaster;
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(pointerEventData, results);

        List<GameObject> uiElements = new List<GameObject>();

        foreach (var result in results)
        {
            // Filter for specific script (CustomScriptName)
            SlotManager customScript = result.gameObject.GetComponent<SlotManager>();
            if (customScript != null)
            {
                uiElements.Add(result.gameObject);
            }
        }

        if (uiElements.Count > 0)
        {
            HandleNewSlot(uiElements[0].GetComponent<SlotManager>());
        }
        else
        {
            this.transform.SetParent(currentSlot.gameObject.transform);
            this.gameObject.transform.position = currentPosition;
            Destroy(slotItem.gameObject);
        }

    }

    private void HandleNewSlot(SlotManager newSlot)
    {
        if (newSlot == currentSlot)
        {
            //currentSlot.AddItem()
            Destroy(slotItem.gameObject);
            return;
        }
        //see if we can move the item at all, if not we dont do anything
        if (newSlot.variant == item.variant || newSlot.variant == ItemVariant.None)
        {
            //see if the slot is null, if so just simply move it in there
            if (newSlot.item == null)
            {
                Debug.Log("New slot is empty so we can move it!");
                currentSlot.RemoveItemAmount(amount);
                newSlot.SetItem(item, amount);
            }
            //if the new slot is the same item, stack it up, any left overs stay in the original slot
            else if (newSlot.item == item)
            {
                int maxMoveCount = newSlot.item.maxStackSize - newSlot.currentAmount;
                int leftover = maxMoveCount - amount;
                Debug.Log("The new slot is the same item");
                //if leftover is lower, we have too much items to move
                if (leftover < 0)
                {
                    Debug.Log("We have too many items to move, but we full stack the new slot");
                    //we have to many we wanna move
                    currentSlot.RemoveItemAmount(amount - maxMoveCount);
                    newSlot.AddItem(item, maxMoveCount);
                }
                else
                {
                    Debug.Log("We have less than max stack amount of items, moving everything");
                    //we can move everything
                    newSlot.AddItem(item, amount);
                    currentSlot.RemoveItem();
                }

            }
            else
            {
                //we need to move the full slot amount if we wanna switch
                if (amount == currentSlot.currentAmount) 
                {
                    //can the new slot item go into our current slot?
                    if (newSlot.item.variant == currentSlot.variant || currentSlot.variant == ItemVariant.None)
                    { 
                        
                        //if we got here we can swap them
                        Debug.Log("We can swap the items!");
                        SO_Item swapItem = newSlot.item;
                        int swapAmount = newSlot.currentAmount;

                        newSlot.RemoveItem();
                        newSlot.AddItem(item, amount);

                        currentSlot.RemoveItem();
                        currentSlot.AddItem(swapItem, swapAmount);
                    }
                    else
                    {
                        Debug.Log("The item we want to swap does not adhere to our slot variant");
                    }
                }
                else
                {
                    Debug.Log("We didnt move all items");
                }

            }
        }

        Destroy(slotItem.gameObject);
        //always put image back to original slot
        this.transform.SetParent(currentSlot.gameObject.transform);
        this.gameObject.transform.position = currentPosition;
    }

    private void updateTempItem(SO_Item item, int amount)
    {
        slotItem.GetComponent<Image>().sprite = item.ItemIcon;
        slotItem.GetComponentInChildren<TextMeshProUGUI>().text = amount.ToString();
    }


}
