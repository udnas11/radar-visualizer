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
    //[SerializeField]
    //Vector2[] _coneAngles;
    #endregion


    #region private protected vars
    bool _isTWS;
    #endregion


    #region pub methods
    /*
    public Vector2 SetConeAngles(int preset)
    {
        _coneRenderer.GenerateCone(_coneAngles[preset]);
        return _coneAngles[preset];
    }*/

    public void SetConeAngles(Vector2 angles)
    {
        _coneRenderer.GenerateCone(angles);
    }
    #endregion


    #region private protected methods
    #endregion


    #region events
    /*
    void OnToggleTWS()
    {
        _isTWS = !_isTWS;

        SetConeAngles(_isTWS ? 1 : 0);

        Vector3 currentRotation = transform.localRotation.eulerAngles;
        currentRotation.y = 0;
        transform.localRotation = Quaternion.Euler(currentRotation);
    }
    */
    #endregion


    #region mono events
    private void Awake()
    {
        RegisterSingleton(this);
    }

    private void Start()
    {
        //GlobalInputHandler.Instance.OnToggleTWS += OnToggleTWS;
    }

    private void Update()
    {
        this.transform.position = _player.transform.position;
    }
    #endregion
}
