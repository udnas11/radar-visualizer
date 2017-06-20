using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class RadarDisplayController : Singleton<RadarDisplayController>
{
    #region public serialised vars
    [SerializeField]
    RectTransform _cursor;
    [SerializeField]
    float _cursorMoveSpeed = 1f;
    #endregion


    #region private protected vars
    const float _areaSize = 476;
    #endregion


    #region pub methods
    Vector3 TransformDisplayToWorld(Vector2 position)
    {
        return Vector3.zero;
    }
    #endregion


    #region private protected methods
    Vector2 _cursorAxis = Vector2.zero;
    #endregion


    #region events

    void OnCursorInputChange(Vector2 axis)
    {
        _cursorAxis = axis;
    }
    #endregion


    #region mono events
    private void Awake()
    {
        RegisterSingleton(this);
    }

    private void Start()
    {
        GlobalInputHandler.Instance.OnCursorAxisChange += OnCursorInputChange;
    }

    private void Update()
    {
        Vector2 move = _cursorAxis * _cursorMoveSpeed * Time.deltaTime;
        move = _cursor.anchoredPosition + move;
        move.x = Mathf.Clamp(move.x, -_areaSize / 2, _areaSize / 2);
        move.y = Mathf.Clamp(move.y, 0, _areaSize);
        _cursor.anchoredPosition = move;

        if (Input.GetKeyDown(KeyCode.G))
        {
            Debug.Log(_cursor.anchoredPosition);
        }
    }
    #endregion
}
