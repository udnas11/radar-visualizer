using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;

public class RadarDisplayController : Singleton<RadarDisplayController>
{
    #region public serialised vars
    [SerializeField, Header("Cursor")]
    RectTransform _cursor;
    [SerializeField]
    float _cursorMoveSpeed = 1f;

    [SerializeField, Header("Indicators")]
    Text _zoomText;

    public Transform shit;
    #endregion


    #region private protected vars
    int _zoomFactor = 80;
    Vector2 _coneAngles;
    bool _isTWS = false;
    const float _areaSize = 476;
    #endregion


    #region pub methods
    Vector3 TransformDisplayToWorld(Vector2 position)
    {
        float distance = Mathf.InverseLerp(0, _areaSize, position.y) * _zoomFactor;
        
        float angle = position.x / (_areaSize/2);
        angle *= Constants.RadarConfig.LRSRadarConeAngles.x*0.5f;

        Vector3 result = Quaternion.Euler(0f, angle, 0f) * new Vector3(0f, 0f, distance);
        return result;
    }
    #endregion


    #region private protected methods
    Vector2 _cursorAxis = Vector2.zero;
    #endregion


    #region events
    void OnZoomInputChange(float axis)
    {
        if (axis > 0)
            _zoomFactor *= 2;
        else if (axis < 0)
            _zoomFactor /= 2;

        _zoomFactor = Mathf.Clamp(_zoomFactor, 10, 160);
        _zoomText.text = _zoomFactor.ToString();
    }

    void OnCursorInputChange(Vector2 axis)
    {
        _cursorAxis = axis;
    }

    void OnToggleTWS()
    {
        _isTWS = !_isTWS;

        _coneAngles = _isTWS ? Constants.RadarConfig.TWSRadarConeAngles : Constants.RadarConfig.LRSRadarConeAngles;
        RadarConeController.Instance.SetConeAngles(_coneAngles);
    }
    #endregion


    #region mono events
    private void Awake()
    {
        RegisterSingleton(this);

        _coneAngles = Constants.RadarConfig.LRSRadarConeAngles;
    }

    private void Start()
    {
        GlobalInputHandler.Instance.OnCursorAxisChange += OnCursorInputChange;
        GlobalInputHandler.Instance.OnDisplayZoomAxisChange += OnZoomInputChange;
        GlobalInputHandler.Instance.OnToggleTWS += OnToggleTWS;

        RadarConeController.Instance.SetConeAngles(_coneAngles);
    }

    private void Update()
    {
        Vector2 move = _cursorAxis * _cursorMoveSpeed * Time.deltaTime;
        move = _cursor.anchoredPosition + move;
        move.x = Mathf.Clamp(move.x, -_areaSize / 2, _areaSize / 2);
        move.y = Mathf.Clamp(move.y, 0, _areaSize);
        _cursor.anchoredPosition = move;

        if (Input.GetKeyDown(KeyCode.G))
        {
            Debug.Log(_cursor.anchoredPosition);
            Vector3 pos = TransformDisplayToWorld(_cursor.anchoredPosition);
            Instantiate(shit, pos, Quaternion.identity);
        }
    }
    #endregion
}
