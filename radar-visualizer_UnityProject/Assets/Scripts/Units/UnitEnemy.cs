using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class UnitEnemy : MonoBehaviour
{
    #region public serialised vars
    [HideInInspector]
    public UnitDisplay UnitDisplay;

    [SerializeField]
    LineRenderer _lineRenderer;
    [SerializeField]
    MeshRenderer _meshRenderer;
    [SerializeField]
    MeshRenderer _selectionSphere;
    #endregion


    #region private protected vars
    bool _isVisible;
    float _onClickDistanceFromCamera;
    #endregion


    #region pub methods
    public void SetAltitude(float alt)
    {
        Vector3 newPos = transform.position;
        newPos.y = alt;
        transform.position = newPos;
    }
    #endregion


    #region private protected methods
    public void UpdateVisual(bool forceUpdate = false)
    {
        _lineRenderer.SetPosition(1, new Vector3(0, -transform.position.y, 0f));

        bool visible = IsVisible();

        if (visible != _isVisible || forceUpdate)
        {
            _isVisible = visible;

            _meshRenderer.material.color = visible ? Color.yellow : Color.red;

            if (UnitDisplay != null)
                UnitDisplay.SetVisible(visible);
        }
    }

    public bool IsVisible()
    {
        Vector3 posOnGround = transform.position;
        posOnGround.y = 0f;
        float distance = posOnGround.magnitude;
        float alt = transform.position.y;
        Vector2 altitudeLimits = RadarDisplayController.Instance.GetAltitudeLimitsAtDistance(distance);
        bool altitudeVisible = altitudeLimits.x <= alt && alt <= altitudeLimits.y;

        float angle = Math.GetAngleHorizontalForPoint(transform.position);
        Vector2 coneAngles = RadarDisplayController.Instance.ConeAngles;
        Vector2 coneRotation = RadarDisplayController.Instance.ConeRotation;
        bool horizontalAngleVisible = coneRotation.x - coneAngles.x / 2f <= angle && angle <= coneRotation.x + coneAngles.x / 2f;
        return altitudeVisible && horizontalAngleVisible;
    }
    #endregion


    #region events
    #endregion


    #region mono events
    private void OnMouseEnter()
    {
        _selectionSphere.enabled = true;
    }

    private void OnMouseExit()
    {
        _selectionSphere.enabled = false;
    }

    private void OnMouseDrag()
    {
        if (EditModesHandler.Instance.ActiveMode == EEditModeType.Move)
        {
            CameraInputHandler camHandler = GlobalInputHandler.Instance.ActiveCamera;
            Vector3 mousePos = Input.mousePosition;
            //mousePos.z = Vector3.Distance(camHandler.Camera.transform.position, this.transform.position);
            mousePos.z = _onClickDistanceFromCamera;
            Vector3 mouseWorldPos = camHandler.Camera.ScreenToWorldPoint(mousePos);

            this.transform.position = mouseWorldPos;
        }
    }

    private void OnMouseDown()
    {
        if (EditModesHandler.Instance.ActiveMode == EEditModeType.Delete)
            EnemyHandler.Instance.DeleteEnemy(this);
        else if (EditModesHandler.Instance.ActiveMode == EEditModeType.Move)
            //_onClickDistanceFromCamera = Vector3.Distance(GlobalInputHandler.Instance.ActiveCamera.Camera.transform.position, this.transform.position);
            _onClickDistanceFromCamera = GlobalInputHandler.Instance.ActiveCamera.Camera.WorldToScreenPoint(this.transform.position).z;
    }

    void Update()
    {
        UpdateVisual();
    }

    void Start()
    {
        EnemyHandler.Instance.RegisterEnemy(this);

        UpdateVisual(true);
    }
    #endregion
}
