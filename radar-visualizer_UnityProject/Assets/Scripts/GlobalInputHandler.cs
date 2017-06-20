using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class GlobalInputHandler : Singleton<GlobalInputHandler>
{

    public event Action OnToggleTWS;
    public event Action<Vector2> OnCursorAxisChange;
    public event Action<float> OnDisplayZoomAxisChange;

    #region public serialised vars
    [SerializeField]
    KeyCode _toggleTWS;
    #endregion


    #region private protected vars
    #endregion


    #region pub methods
    #endregion


    #region private protected methods
    Vector2 _cursorAxis = Vector2.zero;
    float _zoomAxis = 0;
    #endregion


    #region events
    #endregion


    #region mono events
    private void Awake()
    {
        RegisterSingleton(this);
    }

    private void Update()
    {
        if (Input.GetKeyDown(_toggleTWS))
            if (OnToggleTWS != null)
                OnToggleTWS();
        
        Vector2 newCurAxis = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (newCurAxis != _cursorAxis)
        {
            _cursorAxis = newCurAxis;
            OnCursorAxisChange(_cursorAxis);
        }

        float zoomAxis = Input.GetAxisRaw("DisplayZoom");
        if (zoomAxis != _zoomAxis)
        {
            _zoomAxis = zoomAxis;
            OnDisplayZoomAxisChange(_zoomAxis);
        }
    }
    #endregion
}
