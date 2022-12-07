using UnityEngine;

public class Drag : MonoBehaviour
{
    private Vector3 screenSpace;
    private Vector3 offset;

    void OnMouseDown()
    {
        screenSpace = Camera.main.WorldToScreenPoint(transform.position);
        offset = transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenSpace.z));
    }

    void OnMouseDrag()
    {
        Vector3 currentScreenSpace = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenSpace.z);
        Vector3 currentPosition = Camera.main.ScreenToWorldPoint(currentScreenSpace) + offset;
        transform.position = currentPosition;
    }
}
