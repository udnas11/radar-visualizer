using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;

public class RadarDisplayController : Singleton<RadarDisplayController>
{

    public enum ERadarType
    {
        RWS,
        TWS,
        STT
    }

    public event Action<Vector3, float> OnCursorPositionUpdate; // worldspace, angle left/right
    public event Action<Vector2> OnRadarConeAngleChange;
    public event Action<Vector2> OnRadarConeRotationChange;
    public event Action<ERadarType> OnRadarTypeChange;

    #region public serialised vars
    [SerializeField, Header("Cursor")]
    RectTransform _cursor;
    [SerializeField]
    float _cursorMoveSpeed = 1f;
    [SerializeField]
    float _elevationRotateSpeed = 1f;

    [SerializeField, Header("Indicators")]
    Text _zoomText;
    [SerializeField]
    Text _altitudeMaxText;
    [SerializeField]
    Text _altitudeMinText;
    [SerializeField]
    Text _modeText;
    [SerializeField]
    RectTransform _circleBL;
    [SerializeField]
    RectTransform _circleBR;

    [SerializeField, Header("Unit management")]
    UnitDisplay _prefabUnitDisplay;
    [SerializeField]
    RectTransform _enemyArea;
    #endregion


    #region private protected vars
    int _zoomFactor = 80;
    float _cursorDistance = 40f;
    Vector2 _coneAngles;
    ERadarType _type = ERadarType.RWS;
    Vector2 _coneRotation = Vector2.zero;
    Vector2 _altitudeLimits;
    UnitEnemy _enemySTT;
    List<UnitEnemy> _enemiesTWS = new List<UnitEnemy>();

    Vector2 _cursorAxis = Vector2.zero;
    float _elevationAxis = 0;
    #endregion


    #region pub methods
    public float CursorDistance { get { return _cursorDistance; } }
    public Vector2 ConeAngles { get { return _coneAngles; } }
    public Vector2 ConeRotation { get { return _coneRotation; } }
    public Vector2 AltitudeLimits { get { return _altitudeLimits; } }
    public ERadarType ScanType { get { return _type; } }
    public UnitEnemy EnemySTT { get { return _enemySTT; } }
    public List<UnitEnemy> EnemiesTWS { get { return _enemiesTWS; } }

    public Vector3 TransformDisplayToWorld(Vector2 position, out float angle)
    {
        float distance = Mathf.InverseLerp(0, Constants.DisplayAreaSize, position.y) * _zoomFactor;
        
        angle = position.x / (Constants.DisplayAreaSize / 2);
        angle *= Constants.RadarConfig.LRSRadarConeAngles.x*0.5f;

        Vector3 result = Quaternion.Euler(0f, angle, 0f) * new Vector3(0f, 0f, distance);
        return result;
    }

    public Vector2 TransformWorldToDisplay(Vector3 worldPos, out float angle, out float distance)
    {
        Vector3 worldGround = worldPos;
        worldGround.y = 0f;

        distance = worldGround.magnitude;
        angle = Mathf.Atan(worldGround.x / worldGround.z) * Mathf.Rad2Deg;

        return new Vector2((angle/(Constants.RadarConfig.LRSRadarConeAngles.x)) * Constants.DisplayAreaSize, (distance/_zoomFactor)* Constants.DisplayAreaSize);
    }

    public Vector2 GetAltitudeLimitsAtDistance(float distance)
    {
        float a = _coneAngles.y;
        float aRad = a * Mathf.Deg2Rad;
        float b = _coneRotation.y;
        float bRad = b * Mathf.Deg2Rad;

        float AD = distance;
        float CD = Mathf.Sin(aRad / 2f + bRad) * AD;
        float BD = Mathf.Sin(bRad - aRad / 2f) * AD;

        float altitudeDelta = RadarConeController.Instance.GetPivotAltitude();
        return new Vector2(altitudeDelta + BD, altitudeDelta + CD);
    }

    public UnitDisplay CreateUnitDisplay(UnitEnemy unitWorld)
    {
        UnitDisplay newInst = Instantiate(_prefabUnitDisplay, _enemyArea) as UnitDisplay;
        return newInst;
    }

    //user interaction
    public void SetRadarElevation(float degrees)
    {
        //float newAngle = Mathf.Clamp(_coneRotation.y + _elevationAxis * Time.deltaTime * _elevationRotateSpeed, -30f, 30f);
        _coneRotation.y = degrees;

        UpdateAltitudeText();

        if (OnRadarConeRotationChange != null)
            OnRadarConeRotationChange(_coneRotation);
    }
    #endregion


    #region private protected methods
    void UpdateCursorPosition(Vector2 newPos)
    {
        _cursor.anchoredPosition = newPos;

        float angle;
        Vector3 pos = TransformDisplayToWorld(_cursor.anchoredPosition, out angle);
        _cursorDistance = pos.magnitude;

        UpdateAltitudeText();

        UpdateHorizontalZone();

        if (OnCursorPositionUpdate != null)
            OnCursorPositionUpdate(pos, angle);
    }

    void UpdateAltitudeText()
    {
        _altitudeLimits = GetAltitudeLimitsAtDistance(_cursorDistance);
        Vector2 altitudeLimitsAngels = new Vector2(Mathf.Clamp(Math.NMtoAngels(_altitudeLimits.x), 0f, 60f), Mathf.Clamp(Math.NMtoAngels(_altitudeLimits.y), 0f, 60f));
        _altitudeMaxText.text = ((int)altitudeLimitsAngels.y).ToString();
        _altitudeMinText.text = ((int)altitudeLimitsAngels.x).ToString();

        //pos up
        RectTransform rt = _altitudeMaxText.GetComponent<RectTransform>();
        float t = Mathf.InverseLerp(0f, 60f, altitudeLimitsAngels.y);
        rt.anchoredPosition = new Vector2(0f, Mathf.Lerp(0, Constants.DisplayAreaSize / 2f, t));

        //pos down
        rt = _altitudeMinText.GetComponent<RectTransform>();
        t = Mathf.InverseLerp(0f, 60f, altitudeLimitsAngels.x);
        rt.anchoredPosition = new Vector2(0f, Mathf.Lerp(0, Constants.DisplayAreaSize / 2f, t));
    }
    
    void UpdateHorizontalZone()
    {
        float lastConeRotationX = _coneRotation.x;
        float areaSize = Constants.DisplayAreaSize;
        if (_type == ERadarType.RWS)
        {
            _circleBL.pivot = new Vector2(0f, 0f);
            _circleBL.anchoredPosition = new Vector2(-areaSize / 2, 0f);
            _circleBR.pivot = new Vector2(1f, 0f);
            _circleBR.anchoredPosition = new Vector2(areaSize / 2, 0f);
            _coneRotation.x = 0f;
        }
        else if (_type == ERadarType.TWS)
        {
            float cursorPos = Mathf.Clamp(_cursor.anchoredPosition.x, -areaSize / 4f, areaSize / 4f);
            float angleHorizontal;
            TransformDisplayToWorld(new Vector2(cursorPos, 10f), out angleHorizontal);
            _coneRotation.x = angleHorizontal;

            _circleBL.pivot = new Vector2(0.5f, 0f);
            _circleBL.anchoredPosition = new Vector2(cursorPos - areaSize / 4f, 0f);

            _circleBR.pivot = new Vector2(0.5f, 0f);
            _circleBR.anchoredPosition = new Vector2(cursorPos + areaSize / 4f, 0f);
        }
        else //if (_type == ERadarType.STT)
        {
            Debug.LogAssertion("Not implemented");
        }

        if (_coneRotation.x != lastConeRotationX)
            if (OnRadarConeRotationChange != null)
                OnRadarConeRotationChange(_coneRotation);
    }
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

        UpdateCursorPosition(_cursor.anchoredPosition);
    }

    void OnRadarElevationInputChange(float axis)
    {
        _elevationAxis = axis;
    }

    void OnCursorInputChange(Vector2 axis)
    {
        _cursorAxis = axis;
    }

    void OnToggleTWS()
    {
        switch (_type)
        {
            case ERadarType.RWS:
                _type = ERadarType.TWS;
                break;
            case ERadarType.TWS:
                _type = _enemiesTWS.Count > 0 ? ERadarType.STT : ERadarType.RWS;
                break;
            // nothing happens during STT
        }

        _modeText.text = _type.ToString();

        UpdateHorizontalZone();

        switch (_type)
        {
            case ERadarType.RWS:
                _coneAngles = Constants.RadarConfig.LRSRadarConeAngles;
                break;
            case ERadarType.TWS:
                _coneAngles = Constants.RadarConfig.TWSRadarConeAngles;
                break;
            case ERadarType.STT:
                _coneAngles = Constants.RadarConfig.STTRadarConeAngles;
                break;
        }

        if (OnRadarConeAngleChange != null)
            OnRadarConeAngleChange(_coneAngles);

        if (OnRadarTypeChange != null)
            OnRadarTypeChange(_type);
    }
    #endregion


    #region mono events
    private void Awake()
    {
        RegisterSingleton(this);

        _coneAngles = Constants.RadarConfig.LRSRadarConeAngles;
        _coneRotation = Vector2.zero;
    }

    private void Start()
    {
        GlobalInputHandler.Instance.OnCursorAxisChange += OnCursorInputChange;
        GlobalInputHandler.Instance.OnDisplayZoomAxisChange += OnZoomInputChange;
        GlobalInputHandler.Instance.OnToggleTWS += OnToggleTWS;
        GlobalInputHandler.Instance.OnRadarElevationAxisChange += OnRadarElevationInputChange;

        RadarConeController.Instance.SetConeAngles(_coneAngles, _coneRotation);
        UpdateAltitudeText();
    }

    private void Update()
    {
        if (_cursorAxis != Vector2.zero)
        {
            float areaSize = Constants.DisplayAreaSize;
            Vector2 move = _cursorAxis * _cursorMoveSpeed * Time.deltaTime;
            move = _cursor.anchoredPosition + move;
            move.x = Mathf.Clamp(move.x, -areaSize / 2, areaSize / 2);
            move.y = Mathf.Clamp(move.y, 0, areaSize);

            UpdateCursorPosition(move);
        }

        if (_elevationAxis != 0f)
        {            
            float newAngle = Mathf.Clamp(_coneRotation.y + _elevationAxis * Time.deltaTime * _elevationRotateSpeed, -30f, 30f);
            SetRadarElevation(newAngle);
            /*UpdateAltitudeText();

            if (OnRadarConeRotationChange != null)
                OnRadarConeRotationChange(_coneRotation);
                */
        }
    }

    private void OnGUI()
    {
        GUILayout.BeginVertical("Box");
        GUILayout.Label("Zoom: " + _zoomFactor);
        GUILayout.Label("Cone angles: " + _coneAngles);
        GUILayout.Label("Type: " + _type.ToString());
        GUILayout.Label("Cone rotation: " + _coneRotation);
        GUILayout.Label("Cursor distance: " + _cursorDistance);
        GUILayout.EndVertical();
    }
    #endregion
}
