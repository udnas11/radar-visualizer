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
    public event Action<bool> OnShowHiddenEnemiesChange;

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
    [SerializeField]
    RectTransform _vVertical;
    [SerializeField]
    Text _vVerticalText;
    [SerializeField]
    RectTransform _vHorizontal;
    [SerializeField]
    Text _distanceToTargetText;

    [SerializeField, Header("Unit management")]
    UnitDisplay _prefabUnitDisplay;
    [SerializeField]
    RectTransform _enemyArea;


    private bool _showHiddenEnemies = true;
    public bool ShowHiddenEnemies
    {
        get
        {
            return _showHiddenEnemies;
        }
        set
        {
            _showHiddenEnemies = value;
            Debug.Log("hidden show: " + value);
            if (OnShowHiddenEnemiesChange != null)
                OnShowHiddenEnemiesChange(value);
        }
    }
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

    RectTransform _altitudeMaxTextRT;
    RectTransform _altitudeMinTextRT;
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
        /*
        float a = _coneAngles.y;
        float aRad = a * Mathf.Deg2Rad;
        float b = _coneRotation.y;
        float bRad = b * Mathf.Deg2Rad;

        float AD = distance;
        float CD = Mathf.Sin(aRad / 2f + bRad) * AD;
        float BD = Mathf.Sin(bRad - aRad / 2f) * AD;

        float altitudeDelta = RadarConeController.Instance.GetPivotAltitude();
        return new Vector2(altitudeDelta + BD, altitudeDelta + CD)
        */
        float minimum = Math.GetPointForVerticalAngleAtGroundDistance(distance, _coneRotation.y - _coneAngles.y * 0.5f).y;
        float maximum = Math.GetPointForVerticalAngleAtGroundDistance(distance, _coneRotation.y + _coneAngles.y * 0.5f).y;
        float alt = RadarConeController.Instance.GetPivotAltitude();
        return new Vector2(minimum + alt, maximum + alt);
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

        UpdateVerticalZone();

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

        UpdateVerticalZone();

        UpdateHorizontalZone();

        if (OnCursorPositionUpdate != null)
            OnCursorPositionUpdate(pos, angle);
    }

    void UpdateVerticalZone()
    {
        if (_type != ERadarType.STT) //stt handled in update
        {
            _altitudeLimits = GetAltitudeLimitsAtDistance(_cursorDistance);
            Vector2 altitudeLimitsAngels = new Vector2(Mathf.Clamp(Math.NMtoAngels(_altitudeLimits.x), 0f, 60f), Mathf.Clamp(Math.NMtoAngels(_altitudeLimits.y), 0f, 60f));
            _altitudeMaxText.text = ((int)altitudeLimitsAngels.y).ToString();
            _altitudeMinText.text = ((int)altitudeLimitsAngels.x).ToString();

            //pos up
            float t = Mathf.InverseLerp(0f, 60f, altitudeLimitsAngels.y);
            _altitudeMaxTextRT.anchoredPosition = new Vector2(0f, Mathf.Lerp(0, Constants.DisplayAreaSize / 2f, t));

            //pos down
            t = Mathf.InverseLerp(0f, 60f, altitudeLimitsAngels.x);
            _altitudeMinTextRT.anchoredPosition = new Vector2(0f, Mathf.Lerp(0, Constants.DisplayAreaSize / 2f, t));
        }
    }
    
    void UpdateHorizontalZone()
    {
        float lastConeRotationX = _coneRotation.x;
        float areaSize = Constants.DisplayAreaSize;
        if (_type == ERadarType.RWS)
        {
            _circleBL.anchoredPosition = new Vector2(-areaSize / 2, 0f);
            _circleBR.anchoredPosition = new Vector2(areaSize / 2, 0f);
            _coneRotation.x = 0f;
        }
        else if (_type == ERadarType.TWS)
        {
            float cursorPos = Mathf.Clamp(_cursor.anchoredPosition.x, -areaSize / 4f, areaSize / 4f);
            float centerRotation;
            if (_enemiesTWS.Count == 0)
                centerRotation = cursorPos;
            else
            {
                float limLeft = 1000f;
                float limRight = -1000f;
                for (int i = 0; i < _enemiesTWS.Count; i++)
                {
                    float pos = _enemiesTWS[i].UnitDisplay.RectTransform.anchoredPosition.x;
                    if (pos < limLeft)
                        limLeft = pos;
                    if (pos > limRight)
                        limRight = pos;
                }

                limLeft += areaSize / 4f - 10f;
                limRight -= areaSize / 4f - 10f;

                centerRotation = Mathf.Clamp(cursorPos, limRight, limLeft);
            }
            float angleHorizontal;
            TransformDisplayToWorld(new Vector2(centerRotation, 10f), out angleHorizontal);
            _coneRotation.x = angleHorizontal;
            
            _circleBL.anchoredPosition = new Vector2(centerRotation - areaSize / 4f, 0f);
            _circleBR.anchoredPosition = new Vector2(centerRotation + areaSize / 4f, 0f);
        }
        else //if (_type == ERadarType.STT)
        {
            // handled in Update
        }

        if (_coneRotation.x != lastConeRotationX)
            if (OnRadarConeRotationChange != null)
                OnRadarConeRotationChange(_coneRotation);
    }

    void SetShowPrimaryTarget(bool show)
    {
        _vVertical.gameObject.SetActive(show);
        _vHorizontal.gameObject.SetActive(show);
        _distanceToTargetText.gameObject.SetActive(show);
    }

    void UpdateVisualsPrimaryTarget(UnitEnemy enemy)
    {
        if (enemy == null)
        {
            Debug.LogError("Attempting update visuals primary target for null");
            return;
        }

        //horizontal
        float banditPos = enemy.UnitDisplay.RectTransform.anchoredPosition.x;
        _vHorizontal.anchoredPosition = new Vector2(banditPos, 0f);

        //vertical
        float altitude = Math.NMtoAngels(enemy.transform.position.y);
        _vVerticalText.text = altitude.ToString("#0.0").Replace(".", " - ");
        float t = Mathf.InverseLerp(0f, 60f, altitude);
        _vVertical.anchoredPosition = new Vector2(0f, Mathf.Lerp(0, Constants.DisplayAreaSize / 2f, t));

        //distance
        float dist = enemy.transform.position.magnitude;
        _distanceToTargetText.text = dist.ToString("0.");
    }

    void UpdateWhenTWS()
    {
        //checking for deleted enemies
        while (_enemiesTWS.Contains(null))
        {
            _enemiesTWS.Remove(null);

            for (int i = 0; i < _enemiesTWS.Count; i++)
                _enemiesTWS[i].UnitDisplay.SetDisplayType(i == 0 ? UnitDisplay.EUnitDisplayType.STT : UnitDisplay.EUnitDisplayType.TWSMarked);
            SetShowPrimaryTarget(_enemiesTWS.Count > 0);
        }
        
        //checking enemies who got out of frustrum
        for (int i = _enemiesTWS.Count - 1 ; i >= 0; i--)
        {
            if (_enemiesTWS[i].IsVisible() == false)
                RemoveEnemyFromTWS(_enemiesTWS[i]);
        }
    }

    void UpdateWhenSTT()
    {
        if (_enemySTT == null)
        {
            Debug.Log("tracked target deleted");
            SetScanType(ERadarType.RWS);
            return;
        }

        Vector2 lastConeRotation = _coneRotation;

        //radar cone
        _coneRotation.x = Math.GetAngleHorizontalForPoint(_enemySTT.transform.position);
        _coneRotation.y = Math.GetAngleVerticalForPoint(_enemySTT.transform.position - Player.Instance.transform.position);        
        
        //check for fail
        if (_coneRotation != lastConeRotation)
        {
            if (Mathf.Abs(_coneRotation.x) <= Constants.RadarConfig.GimbalLimitsSTT.x &&
                Mathf.Abs(_coneRotation.y) <= Constants.RadarConfig.GimbalLimitsSTT.y)
            {
                if (OnRadarConeRotationChange != null)
                    OnRadarConeRotationChange(_coneRotation);
            }
            else
                SetScanType(ERadarType.RWS);
        }
    }

    void SetScanType(ERadarType newType)
    {
        _type = newType;
        switch (_type)
        {
            case ERadarType.RWS:
                _coneAngles = Constants.RadarConfig.LRSRadarConeAngles;
                _altitudeMinText.gameObject.SetActive(true);
                _altitudeMaxText.gameObject.SetActive(true);
                _circleBL.gameObject.SetActive(true);
                _circleBR.gameObject.SetActive(true);
                
                _circleBL.pivot = new Vector2(0f, 0f);
                _circleBR.pivot = new Vector2(1f, 0f);

                SetShowPrimaryTarget(false);

                _enemySTT = null;
                _enemiesTWS.Clear();

                if (Mathf.Abs(_coneRotation.y) > Constants.RadarConfig.GimbalLimits.y)
                {
                    _coneRotation.y = Mathf.Clamp(_coneRotation.y, -Constants.RadarConfig.GimbalLimits.y, Constants.RadarConfig.GimbalLimits.y);
                    if (OnRadarConeRotationChange != null)
                        OnRadarConeRotationChange(_coneRotation);
                }
                break;
            case ERadarType.TWS:
                _coneAngles = Constants.RadarConfig.TWSRadarConeAngles;
                _altitudeMinText.gameObject.SetActive(true);
                _altitudeMaxText.gameObject.SetActive(true);
                _circleBL.gameObject.SetActive(true);
                _circleBR.gameObject.SetActive(true);

                _circleBL.pivot = new Vector2(0.5f, 0f);
                _circleBR.pivot = new Vector2(0.5f, 0f);

                SetShowPrimaryTarget(false);

                _enemySTT = null;
                break;
            case ERadarType.STT:
                _coneAngles = Constants.RadarConfig.STTRadarConeAngles;
                _altitudeMinText.gameObject.SetActive(false);
                _altitudeMaxText.gameObject.SetActive(false);
                _circleBL.gameObject.SetActive(false);
                _circleBR.gameObject.SetActive(false);

                SetShowPrimaryTarget(true);

                _enemiesTWS.Clear();
                break;
        }

        _modeText.text = _type.ToString();
        UpdateHorizontalZone();
        UpdateVerticalZone();

        if (OnRadarConeAngleChange != null)
            OnRadarConeAngleChange(_coneAngles);

        if (OnRadarTypeChange != null)
            OnRadarTypeChange(_type);
    }

    UnitEnemy GetEnemyUnderCursor()
    {
        List<UnitEnemy> enemies = EnemyHandler.Instance.Enemies;
        float dist = Constants.DisplayLockRange;
        UnitDisplay result = null;

        for (int i = 0; i < enemies.Count; i++)
        {
            UnitDisplay ud = enemies[i].UnitDisplay;
            if (ud.IsVisible)
            {
                float distThis = Vector2.Distance(_cursor.anchoredPosition, ud.RectTransform.anchoredPosition);
                if (distThis < dist)
                {
                    dist = distThis;
                    result = ud;
                }
            }
        }

        return result != null ? result.WorldUnit : null;
    }

    void AddEnemyToTWS(UnitEnemy unit)
    {
        _enemiesTWS.Add(unit);

        for (int i = 0; i < _enemiesTWS.Count; i++)
            _enemiesTWS[i].UnitDisplay.SetDisplayType(i == 0 ? UnitDisplay.EUnitDisplayType.STT : UnitDisplay.EUnitDisplayType.TWSMarked);

        if (_enemiesTWS.Count > 0)
            SetShowPrimaryTarget(true);
    }

    void RemoveEnemyFromTWS(UnitEnemy unit)
    {
        _enemiesTWS.Remove(unit);
        unit.UnitDisplay.SetDisplayType(UnitDisplay.EUnitDisplayType.TWS);

        for (int i = 0; i < _enemiesTWS.Count; i++)
            _enemiesTWS[i].UnitDisplay.SetDisplayType(i == 0 ? UnitDisplay.EUnitDisplayType.STT : UnitDisplay.EUnitDisplayType.TWSMarked);

        if (_enemiesTWS.Count == 0)
            SetShowPrimaryTarget(false);
    }
    #endregion


    #region events
    void OnZoomInputChange(float axis)
    {
        if (axis > 0)
            _zoomFactor /= 2;
        else if (axis < 0)
            _zoomFactor *= 2;

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
                SetScanType(ERadarType.TWS);
                break;
            case ERadarType.TWS:
                if (_enemiesTWS.Count == 0)
                    SetScanType(ERadarType.RWS);
                else
                {
                    _enemySTT = _enemiesTWS[0];
                    SetScanType(ERadarType.STT);
                }
                break;
            // nothing happens during STT
        }        
    }

    private void OnLockPressed()
    {
        UnitEnemy selectedEnemy = null;
        switch (_type)
        {
            case ERadarType.RWS:
                selectedEnemy = GetEnemyUnderCursor();
                if (selectedEnemy != null)
                {
                    _enemySTT = selectedEnemy;
                    SetScanType(ERadarType.STT);
                }
                break;
            case ERadarType.TWS:
                selectedEnemy = GetEnemyUnderCursor();
                if (selectedEnemy != null)
                {
                    if (_enemiesTWS.Contains(selectedEnemy))
                    {
                        _enemySTT = selectedEnemy;
                        SetScanType(ERadarType.STT);
                    }
                    else
                    {
                        if (_enemiesTWS.Count < 4)
                            AddEnemyToTWS(selectedEnemy);
                    }
                }
                break;
            case ERadarType.STT:
                SetScanType(ERadarType.RWS);
                break;
        }
    }

    private void OnUnlockPressed()
    {
        switch (_type)
        {
            case ERadarType.RWS:
                // nothing
                break;
            case ERadarType.TWS:
                UnitEnemy selectedEnemy = GetEnemyUnderCursor();
                if (_enemiesTWS.Contains(selectedEnemy))
                    RemoveEnemyFromTWS(selectedEnemy);
                break;
            case ERadarType.STT:
                SetScanType(ERadarType.RWS);
                break;
        }
    }
    #endregion


    #region mono events
    private void Awake()
    {
        RegisterSingleton(this);

        _altitudeMaxTextRT = _altitudeMaxText.GetComponent<RectTransform>();
        _altitudeMinTextRT = _altitudeMinText.GetComponent<RectTransform>();

        _coneAngles = Constants.RadarConfig.LRSRadarConeAngles;
        _coneRotation = Vector2.zero;
    }

    private void Start()
    {
        GlobalInputHandler.Instance.OnCursorAxisChange += OnCursorInputChange;
        GlobalInputHandler.Instance.OnDisplayZoomAxisChange += OnZoomInputChange;
        GlobalInputHandler.Instance.OnToggleTWS += OnToggleTWS;
        GlobalInputHandler.Instance.OnLockPressed += OnLockPressed;
        GlobalInputHandler.Instance.OnUnlockPressed += OnUnlockPressed;
        GlobalInputHandler.Instance.OnRadarElevationAxisChange += OnRadarElevationInputChange;

        RadarConeController.Instance.SetConeAngles(_coneAngles, _coneRotation);
        UpdateVerticalZone();
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

        if (_type == ERadarType.STT)
        {
            UpdateWhenSTT();
            UpdateVisualsPrimaryTarget(_enemySTT);
        }
        else if (_type == ERadarType.TWS)
        {
            UpdateWhenTWS();
            if (_enemiesTWS.Count > 0)
                UpdateVisualsPrimaryTarget(_enemiesTWS[0]);
        }
    }
    /*
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
    */
    #endregion
}
