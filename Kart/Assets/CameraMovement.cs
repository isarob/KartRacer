using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {

public float speed = 2.0f;

void Update ()
{
	float horizMovement = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
	float vertMovement = Input.GetAxis("Vertical") * speed * Time.deltaTime;

	transform.Translate(Vector3.right * horizMovement);
	transform.Translate(Vector3.forward * vertMovement);
    if (Input.GetKey(KeyCode.Q))
	{
		transform.Translate(Vector3.down * speed * Time.deltaTime);
	}
	if (Input.GetKey(KeyCode.E))
	{
		transform.Translate(Vector3.up * speed * Time.deltaTime);
	}
}

}