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
    #endregion


    #region private protected vars
    LineRenderer _lineRenderer;
    MeshRenderer _meshRenderer;
    bool _isVisible;
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
        float distance = transform.position.magnitude;
        float alt = transform.position.y;
        Vector2 altitudeLimits = RadarDisplayController.Instance.GetAltitudeLimitsAtDistance(distance);
        bool altitudeVisible = altitudeLimits.x <= alt && alt <= altitudeLimits.y;

        float angle = Math.GetPointHorizontalAngle(transform.position);
        Vector2 coneAngles = RadarDisplayController.Instance.ConeAngles;
        Vector2 coneRotation = RadarDisplayController.Instance.ConeRotation;
        bool horizontalAngleVisible = coneRotation.x - coneAngles.x / 2f <= angle && angle <= coneRotation.x + coneAngles.x / 2f;
        return altitudeVisible && horizontalAngleVisible;
    }
    #endregion


    #region events
    #endregion


    #region mono events
    void Update()
    {
        UpdateVisual();
    }

    void Start()
    {
        EnemyHandler.Instance.RegisterEnemy(this);

        UpdateVisual(true);
    }

    void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _meshRenderer = GetComponent<MeshRenderer>();
    }
    #endregion
}
