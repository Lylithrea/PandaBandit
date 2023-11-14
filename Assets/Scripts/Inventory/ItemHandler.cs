using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{

    public SO_Item item;

    public void Start()
    {
        GetComponent<Image>().sprite = item.ItemIcon;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("Started dragging");
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("Dragging");
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("Ended dragging");
    }

}
