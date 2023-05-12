using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Requirement/SO_Interaction")]
public class S_Interaction : ScriptableObject, I_Requirement
{
    [field:SerializeField]public bool isAchieved { get; set; }
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
