using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour {

public float sensitivityX = 5F;
public float sensitivityY = 5F;
public float minimumX = -360F;
public float maximumX = 360F;
public float minimumY = -60F;
public float maximumY = 60F;
float rotationY = 0F;

void Update ()
{

	if (!Input.GetMouseButton(0))
	{
		float rotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivityX;
		rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
		rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);
		transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);
	}
    if(Input.GetMouseButton(0)) 
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true; 
    }
    else {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = true; 
    }

}

}