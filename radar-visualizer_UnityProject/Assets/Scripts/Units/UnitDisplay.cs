using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class UnitDisplay : MonoBehaviour
{

    public enum EUnitDisplayType
    {
        RWS,
        TWS,
        STT
    }

    #region public serialised vars
    [HideInInspector]
    public UnitEnemy WorldUnit;

    [SerializeField]
    Image _imageSquare;
    [SerializeField]
    Image _imageHeadingLine;
    [SerializeField]
    Text _textAltitude;

    [SerializeField, Space]
    Sprite _spriteTWS;
    [SerializeField]
    Sprite _spriteSTT;
    #endregion


    #region private protected vars
    RectTransform _rt;
    bool _isVisible;

    Color _squareColor = Color.green;
    #endregion


    #region pub methods
    public RectTransform RectTransform { get { return _rt; } }
    public bool IsVisible { get { return _isVisible; } }

    public bool DoesFitDisplay()
    {
        Vector2 pos = _rt.anchoredPosition;
        bool horizontal = Mathf.Abs(pos.x) <= Constants.DisplayAreaSize/2f;
        bool vertical = 0f <= pos.y && pos.y <= Constants.DisplayAreaSize;
        return horizontal && vertical;
    }

    public void SetVisible(bool newState)
    {
        _isVisible = newState;

        _imageSquare.color = _isVisible ? _squareColor : Constants.Colors.EnemyDisplayInvisible;
        _textAltitude.gameObject.SetActive(_isVisible);
    }

    public void SetDisplayType(EUnitDisplayType newType)
    {
        switch (newType)
        {
            case EUnitDisplayType.RWS:
                _squareColor = Color.green;
                _imageHeadingLine.enabled = false;
                _textAltitude.enabled = false;
                break;
            case EUnitDisplayType.TWS:
                _squareColor = Constants.Colors.EnemyDisplayTWS;
                _imageHeadingLine.enabled = true;
                _imageHeadingLine.sprite = _spriteTWS;
                _textAltitude.enabled = true;
                break;
            case EUnitDisplayType.STT:
                _squareColor = Color.clear;
                _imageHeadingLine.enabled = true;
                _imageHeadingLine.sprite = _spriteSTT;
                _textAltitude.enabled = false;
                break;
        }
        SetVisible(_isVisible);
    }
    #endregion


    #region private protected methods
    void UpdateToMirrorWorld()
    {
        float angle, distance;
        Vector2 displayPos = RadarDisplayController.Instance.TransformWorldToDisplay(WorldUnit.transform.position, out angle, out distance);

        _rt.anchoredPosition = displayPos;

        //float rot = WorldUnit.transform.rotation.eulerAngles.y;
        //_imageHeadingLine.transform.localRotation = Quaternion.Euler(0f, 0f, -rot);

        bool fitsDisplay = DoesFitDisplay();
        _imageSquare.enabled = fitsDisplay;
        _imageHeadingLine.gameObject.SetActive(fitsDisplay);

        float alt = WorldUnit.transform.position.y;
        _textAltitude.text = Math.NMtoAngels(alt).ToString("0.");
    }
    #endregion


    #region events
    private void OnRadarScanModeChanged(RadarDisplayController.ERadarType newType)
    {
        switch (newType)
        {
            case RadarDisplayController.ERadarType.RWS:
                SetDisplayType(EUnitDisplayType.RWS);
                break;
            case RadarDisplayController.ERadarType.TWS:
                bool marked = RadarDisplayController.Instance.EnemiesTWS.Contains(WorldUnit);
                SetDisplayType(marked ? EUnitDisplayType.STT : EUnitDisplayType.TWS);
                break;
            case RadarDisplayController.ERadarType.STT:
                bool locked = RadarDisplayController.Instance.EnemySTT == WorldUnit;
                SetDisplayType(locked ? EUnitDisplayType.STT : EUnitDisplayType.RWS);
                break;
        }
    }
    #endregion


    #region mono events
    private void Awake()
    {
        _rt = GetComponent<RectTransform>();
    }

    private void Start()
    {
        RadarDisplayController.Instance.OnRadarTypeChange += OnRadarScanModeChanged;
        OnRadarScanModeChanged(RadarDisplayController.Instance.ScanType); // applying radar scan mode during run-time instantiation        \
    }

    private void Update()
    {
        UpdateToMirrorWorld();
    }
    #endregion
}
