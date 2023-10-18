using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float movementSpeed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private GameObject playerModel;
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

        // Apply the movement direction to the character.
        if (newPos.magnitude > 0)
        {
            moveDirection = newPos.normalized * movementSpeed;
        }
        else
        {
            moveDirection = Vector3.zero;
        }

        playerModel.transform.rotation = GetMouseDirection();

        // Move the character.
        controller.Move(moveDirection * Time.deltaTime);
    }

    private Quaternion GetMouseDirection()
    {
        //to which direction to we need to shoot the projectile?
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitData;
        Vector3 worldPosition = new Vector3(0, 0, 0);
        if (Physics.Raycast(ray, out hitData))
        {
            worldPosition = hitData.point;
        }
        else
        {
            Debug.LogWarning("Mouse click was not on a valid target.");
            return Quaternion.Euler(new Vector3(0, 0, 0));
        }

        //set the rotation
        Vector3 mousePos = worldPosition;
        mousePos.y = 0;
        Vector3 playerPos = this.transform.position;
        playerPos.y = 0;
        Quaternion lookRot = Quaternion.LookRotation(mousePos - playerPos);

        return lookRot;
    }
}
