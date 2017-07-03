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
    public event Action<float> OnRadarElevationAxisChange;
    public event Action<Vector3> OnClickAreaTrigger;

    #region public serialised vars
    [SerializeField]
    KeyCode _toggleTWS;
    #endregion


    #region private protected vars
    CameraInputHandler _activeCameraHandler = null;
    #endregion


    #region pub methods
    public CameraInputHandler ActiveCamera { get { return _activeCameraHandler; } }

    public void SetActiveCamera(CameraInputHandler cameraHandler)
    {
        if (_activeCameraHandler != null)
            _activeCameraHandler.OnMouseButtonDownAction -= OnMouseInput;

        _activeCameraHandler = cameraHandler;

        cameraHandler.OnMouseButtonDownAction += OnMouseInput;
    }
    #endregion


    #region private protected methods
    Vector2 _cursorAxis = Vector2.zero;
    float _zoomAxis = 0;
    float _radarElevationAxis = 0;
    #endregion


    #region events
    private void OnMouseInput(int mouseBtn)
    {
        if (mouseBtn != 0)
            return;

        Camera cam = _activeCameraHandler.Camera;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        bool success = Physics.Raycast(ray, out hit, Mathf.Infinity, _activeCameraHandler.LayerMask);
        if (success)
        {
            if (OnClickAreaTrigger != null)
                OnClickAreaTrigger(hit.point);
        }
    }
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

        float radarElevationAxis = Input.GetAxisRaw("RadarElevation");
        if (radarElevationAxis != _radarElevationAxis)
        {
            _radarElevationAxis = radarElevationAxis;
            OnRadarElevationAxisChange(_radarElevationAxis);
        }
    }
    #endregion
}
