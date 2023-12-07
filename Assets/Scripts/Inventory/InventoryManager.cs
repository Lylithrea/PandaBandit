using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{

    [SerializeField] public GameObject inventoryDragParent;
    private static InventoryManager instance;
    public GraphicRaycaster raycaster;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        raycaster = this.GetComponent<GraphicRaycaster>();
    }

    public static InventoryManager Instance
    {
        get
        {
            return instance;
        }
    }

}
