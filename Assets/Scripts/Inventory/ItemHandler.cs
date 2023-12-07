using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{

    public SO_Item item;
    public int amount;

    private SlotManager currentSlot;
    private Vector3 currentPosition;
    InventoryManager inventoryManager;

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
        GetComponent<Image>().sprite = item.ItemIcon;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("Started dragging");
        currentSlot = this.gameObject.transform.parent.GetComponent<SlotManager>();
        currentPosition = this.gameObject.transform.position;
        this.transform.SetParent(InventoryManager.Instance.inventoryDragParent.transform);
        
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
        }

    }

    private void HandleNewSlot(SlotManager newSlot)
    {
        //if this returns true, it was possible to move all items to the new slot
        //if it returns false, it either couldnt move anything at all or only a part
        /*        if (newSlot.TryToAddItem(item, currentSlot.currentAmount))
                {
                    currentSlot.RemoveItem(item, currentSlot.currentAmount);
                    currentSlot = newSlot;
                    this.transform.SetParent(newSlot.gameObject.transform);
                    this.gameObject.transform.position = currentPosition;
                }*/

        if (newSlot.item == null)
        {
            currentSlot.RemoveItem();
            newSlot.SetItem(item, amount);
        }
        else if (newSlot.item == item)
        {
            int maxMoveCount = newSlot.item.maxStackSize - newSlot.currentAmount;
            int leftover = maxMoveCount - amount;
            if (leftover < 0)
            {
                //we have to many we wanna move
                currentSlot.RemoveItemAmount(amount - maxMoveCount);
                newSlot.AddItem(item, maxMoveCount);
            }
            else
            {
                //we can move everything
                newSlot.AddItem(item, amount);
                currentSlot.RemoveItem();
                item = null;
            }

        }

        //always put image back to original slot
        this.transform.SetParent(currentSlot.gameObject.transform);
        this.gameObject.transform.position = currentPosition;
    }

}
