using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private float rotatingSpeed;
    [SerializeField] private GameObject cameraObject;
    [SerializeField] private float range;
    [SerializeField] private bool rotatePlayerCharacter = true;

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

        if (isAllowedToRotate(mouseX, mouseY))
        {
            Debug.Log("Is allowed to rotate");
            newRot.y = mouseX * Time.deltaTime * rotatingSpeed;
            newRot.x = mouseY * Time.deltaTime * rotatingSpeed;

            Vector3 newQuat1 = (this.transform.rotation).eulerAngles + newRot;

            if (rotatePlayerCharacter)
            {
                Vector3 newQuat2 = new Vector3(0, (this.transform.parent.transform.rotation).eulerAngles.y + newRot.y, 0);
                this.transform.parent.transform.rotation = Quaternion.Euler(newQuat2);
                this.transform.rotation = Quaternion.Euler(newQuat1);
            }
            else
            {
                this.transform.rotation = Quaternion.Euler(newQuat1);
            }

        }


    }


    private bool isAllowedToRotate(float x = 0, float y = 0)
    {

        Vector3 dir1 = this.transform.up * y;
        Vector3 dir2 = -this.transform.right * x;
        Vector3 finaldir = dir1 + dir2;


        if (Physics.Raycast(cameraObject.transform.position, finaldir, range))
        {
            return false;
        }

        return true;
    }


}
