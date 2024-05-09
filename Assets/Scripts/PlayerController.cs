using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    private Camera _cam;

    private void Awake()
    {
        _cam = Camera.main;
        Cursor.visible = false;
    }

    void Update()
    {
        // Move this transform to wherever the mouse is
        var mousePos = _cam.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector3(mousePos.x, mousePos.y, 0);
    }
}
