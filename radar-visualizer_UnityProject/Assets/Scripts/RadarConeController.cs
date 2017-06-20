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
    Transform _cursor;
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
        _cursor.localPosition = pos;
        _cursor.localRotation = Quaternion.Euler(90f, angle, 0f);
    }

    private void OnRadarConeAngleChanged(Vector2 newAngles)
    {
        SetConeAngles(newAngles);
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
    }

    private void Update()
    {
        this.transform.position = _player.transform.position;
    }
    #endregion
}
