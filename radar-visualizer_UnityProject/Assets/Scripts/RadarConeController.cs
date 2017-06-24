using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class RadarConeController : Singleton<RadarConeController>
{
    #region public serialised vars
    [SerializeField]
    RadarConeRenderer _coneRenderer;
    [SerializeField]
    UnitBase _player;
    [SerializeField]
    Transform _cursor2D;
    [SerializeField]
    Transform _cursor3D;
    #endregion


    #region private protected vars
    Vector2 _coneAngles;
    Vector2 _coneRotation;
    #endregion


    #region pub methods
    public void SetConeAngles(Vector2 angles, Vector2 rotation)
    {
        _coneAngles = angles;
        _coneRotation = rotation;
        _coneRenderer.GenerateCone(angles, rotation);
    }

    public float GetPivotAltitude()
    {
        return transform.position.y;
    }
    #endregion


    #region private protected methods
    private void UpdateCursor3D()
    {
        Vector2 altitudeLimits = RadarDisplayController.Instance.AltitudeLimits;
        altitudeLimits.x -= transform.position.y;
        altitudeLimits.y -= transform.position.y;
        _cursor3D.localScale = new Vector3(0.5f, 0.5f, altitudeLimits.y - altitudeLimits.x);
        _cursor3D.localPosition = new Vector3(0f, 0f, -Mathf.Lerp(altitudeLimits.x, altitudeLimits.y, 0.5f));
    }
    #endregion


    #region events
    private void OnCursorPositionUpdate(Vector3 pos, float angle)
    {
        _cursor2D.localPosition = pos;
        _cursor2D.localRotation = Quaternion.Euler(90f, angle, 0f);

        UpdateCursor3D();
    }

    private void OnRadarConeAngleChanged(Vector2 newAngles)
    {
        SetConeAngles(newAngles, _coneRotation);
    }

    private void OnRadarConeRotationChanged(Vector2 newRot)
    {
        SetConeAngles(_coneAngles, newRot);
        UpdateCursor3D();
    }
    #endregion


    #region mono events
    private void Awake()
    {
        RegisterSingleton(this);
    }

    private void Start()
    {
        RadarDisplayController.Instance.OnCursorPositionUpdate += OnCursorPositionUpdate;
        RadarDisplayController.Instance.OnRadarConeAngleChange += OnRadarConeAngleChanged;
        RadarDisplayController.Instance.OnRadarConeRotationChange += OnRadarConeRotationChanged;
    }

    private void Update()
    {
        this.transform.position = _player.transform.position;
    }
    #endregion
}
