using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public class UnitBase : MonoBehaviour
{

    private LineRenderer _lineRenderer;

    public bool IsVisible()
    {
        float distance = transform.position.magnitude;
        float alt = transform.position.y;
        Vector2 altitudeLimits = RadarDisplayController.Instance.GetAltitudeLimitsAtDistance(distance);
        bool altitudeVisible = altitudeLimits.x <= alt && alt <= altitudeLimits.y;

        float angle = Constants.GetPointHorizontalAngle(transform.position);
        Vector2 coneAngles = RadarDisplayController.Instance.ConeAngles;
        Vector2 coneRotation = RadarDisplayController.Instance.ConeRotation;
        bool horizontalAngleVisible = coneRotation.x - coneAngles.x / 2f <= angle && angle <= coneRotation.x + coneAngles.x / 2f;
        return altitudeVisible && horizontalAngleVisible;
    }

    public void SetAltitude(float alt)
    {
        Vector3 newPos = transform.position;
        newPos.y = alt;
        transform.position = newPos;

        //UpdateVisual();
    }

    public virtual void UpdateVisual()
    {
        _lineRenderer.SetPosition(1, new Vector3(0, -transform.position.y, 0f));
    }

    protected virtual void Start()
    {
        UpdateVisual();
    }

    protected virtual void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
    }
}
