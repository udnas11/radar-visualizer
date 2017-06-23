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
    Transform _cursor2D, _cursor3D;
    #endregion


    #region private protected vars
    #endregion


    #region pub methods
    public void SetConeAngles(Vector2 angles)
    {
        _coneRenderer.GenerateCone(angles);
    }
    #endregion


    #region private protected methods
    #endregion


    #region events
    private void OnCursorPositionUpdate(Vector3 pos, float angle)
    {
        _cursor2D.localPosition = pos;
        _cursor2D.localRotation = Quaternion.Euler(90f, angle, 0f);
        _cursor3D.localScale = new Vector3(pos.magnitude, pos.magnitude, pos.magnitude);
    }

    private void OnRadarConeAngleChanged(Vector2 newAngles)
    {
        SetConeAngles(newAngles);
    }

    private void OnRadarConeRotationChanged(Vector2 newRot)
    {
        transform.localRotation = Quaternion.Euler(-newRot.y, 0f, 0f);
        _coneRenderer.transform.localRotation = Quaternion.Euler(0f, newRot.x, 0f);
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
