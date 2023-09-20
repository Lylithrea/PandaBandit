using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float movementSpeed;
    [SerializeField] private float rotationSpeed;
    public CharacterController controller;
    private Vector3 moveDirection = Vector3.zero;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
    }
    // Update is called once per frame
    void Update()
    {
        Vector3 newPos = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        //newPos.Normalize();

        Vector3 newRot = this.transform.position + newPos;

        // Calculate the target angle based on the input axes
        float targetAngle = Mathf.Atan2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) * Mathf.Rad2Deg;

        // Apply the rotation to the player on the y-axis only
        //transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);

        //this.transform.rotation = Quaternion.Lerp(this.transform.rotation,Quaternion.Euler(new Vector3(0,targetAngle,0)), rotationSpeed * Time.deltaTime);

        //newPos *= movementSpeed * Time.deltaTime;
        //this.transform.position += newPos;

        // Apply the movement direction to the character.
        if (newPos.magnitude > 0)
        {
            moveDirection = newPos.normalized * movementSpeed;
        }
        else
        {
            moveDirection = Vector3.zero;
        }

        // Move the character.
        controller.Move(moveDirection * Time.deltaTime);


        //this.transform.
    }
}
