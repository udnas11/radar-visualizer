
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


/// <summary>
/// UnitBase class.
/// </summary>
public class UnitBase : MonoBehaviour
{

    private LineRenderer _lineRenderer;

    public void UpdateAltitude(float alt)
    {
        Vector3 newPos = transform.position;
        newPos.y = alt;
        transform.position = newPos;

        UpdateVisual();
    }

    public void UpdateVisual()
    {
        _lineRenderer.SetPosition(1, new Vector3(0, -transform.position.y, 0f));
    }

    void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        UpdateVisual();
    }

}
