using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public class CameraMovement3D : CameraInputHandler
{
    
    [SerializeField]
    float _rotateCoef = 1f;
    [SerializeField]
    float _zoomCoef = 1f;

    protected override void OnMouseButtonDrag(int mouseBtn, Vector2 delta)
    {
        base.OnMouseButtonDrag(mouseBtn, delta);

        if (mouseBtn == 1)
        {
            transform.Rotate(new Vector3(1f, 0f, 0f), -delta.y * _rotateCoef, Space.Self);
            transform.Rotate(new Vector3(0f, 1f, 0f), delta.x * _rotateCoef, Space.World);
        }
    }

    protected override void Update()
    {
        base.Update();

        if (_isMouseOver)
            _camera.transform.Translate(new Vector3(0, 0, -Input.mouseScrollDelta.y * _camera.transform.localPosition.z * _zoomCoef), Space.Self);
    }

}
