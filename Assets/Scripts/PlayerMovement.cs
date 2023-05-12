using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float movementSpeed;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //TODO: Normalize movement, so that diagonal has same speed
        this.transform.position += this.transform.forward * Input.GetAxis("Vertical") * movementSpeed * Time.deltaTime;
        this.transform.position += this.transform.right * Input.GetAxis("Horizontal") * movementSpeed * Time.deltaTime;
         
    }
}
