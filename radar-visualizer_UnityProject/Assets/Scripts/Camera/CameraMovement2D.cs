using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class CameraMovement2D : CameraInputHandler
{
    #region public serialised vars
    [SerializeField]
    float _translateCoef = 1f;
    [SerializeField]
    float _zoomCoef = 1f;
    [SerializeField]
    Vector3 _orderOfAxisTranslate; // 0 for x; 1 for y; 2 for z;
    #endregion


    #region private protected vars
    #endregion


    #region pub methods
    #endregion


    #region private protected methods
    protected override void OnMouseButtonDrag(int mouseBtn, Vector2 delta)
    {
        base.OnMouseButtonDrag(mouseBtn, delta);

        if (mouseBtn == 1)
        {
            Vector3 translate = OrderInputAxis(new Vector3(-delta.x, -delta.y, 0f), _orderOfAxisTranslate) * _camera.orthographicSize * _translateCoef;
            this.transform.Translate(translate, Space.World);
        }
    }

    Vector3 OrderInputAxis(Vector3 raw, Vector3 order)
    {
        Vector3 result = new Vector3();
        if (order.x == 0)
            result.x = raw.x;
        else if (order.x == 1)
            result.x = raw.y;
        else
            result.x = raw.z;

        if (order.y == 0)
            result.y = raw.x;
        else if (order.y == 1)
            result.y = raw.y;
        else
            result.y = raw.z;

        if (order.z == 0)
            result.z = raw.x;
        else if (order.z == 1)
            result.z = raw.y;
        else
            result.z = raw.z;
        return result;
    }
    #endregion


    #region events
    #endregion


    #region mono events
    protected override void Update()
    {
        base.Update();

        if (_isMouseOver)
            _camera.orthographicSize = Mathf.Clamp(_camera.orthographicSize - Input.mouseScrollDelta.y * _zoomCoef, 1f, 60f);
    }
    #endregion
}
