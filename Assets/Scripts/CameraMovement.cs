using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private float rotatingSpeed;
    [SerializeField] private GameObject cameraObject;
    [SerializeField] private float range;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {

        Vector3 newRot = new Vector3(0, 0, 0);
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        if (isAllowedToRotate(this.transform.right, mouseX, mouseY))
        {
            Debug.Log("Is allowed to rotate");

            return;
        }



        //this should be limited for against walls
        if (mouseX > 0)
        {
            if (isAllowedToRotate(this.transform.right))
            {
                newRot.y = mouseX * Time.deltaTime * rotatingSpeed;
            }
        }
        else if (mouseX < 0)
        {
            if (isAllowedToRotate(-this.transform.right))
            {
                newRot.y = mouseX * Time.deltaTime * rotatingSpeed;
            }
        }

        //this should be limited for against ground

        if (mouseY > 0)
        {
            if (isAllowedToRotate(this.transform.up))
            {
                newRot.x = mouseY * Time.deltaTime * rotatingSpeed;
            }
        }
        else if (mouseY < 0)
        {
            if (isAllowedToRotate(-this.transform.up))
            {
                newRot.x = mouseY * Time.deltaTime * rotatingSpeed;
            }
        }
        //Debug.Log(newRot);
        Vector3 newQuat = (this.transform.rotation).eulerAngles + newRot;
        this.transform.rotation = Quaternion.Euler(newQuat);
    }

    private bool isAllowedToRotate(Vector3 direction, float x = 0, float y = 0)
    {

        Vector3 dir1 = this.transform.up * y;
        Vector3 dir2 = this.transform.right * x;
        Vector3 finaldir = dir1 + dir2;


        if (Physics.Raycast(cameraObject.transform.position, finaldir, range))
        {
            return false;
        }


        return true;
    }


}
