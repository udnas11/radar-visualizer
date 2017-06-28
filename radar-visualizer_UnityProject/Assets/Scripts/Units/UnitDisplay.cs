using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class UnitDisplay : MonoBehaviour
{
    #region public serialised vars
    [HideInInspector]
    public UnitEnemy WorldUnit;
    #endregion


    #region private protected vars
    RectTransform _rt;
    bool _isVisible;

    Image _image;
    #endregion


    #region pub methods
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

        _image.color = _isVisible ? Color.green : Color.red;
    }
    #endregion


    #region private protected methods
    void UpdateToMirrorWorld()
    {
        float angle, distance;
        Vector2 displayPos = RadarDisplayController.Instance.TransformWorldToDisplay(WorldUnit.transform.position, out angle, out distance);

        _rt.anchoredPosition = displayPos;

        _image.enabled = DoesFitDisplay();
    }
    #endregion


    #region events
    #endregion


    #region mono events
    private void Awake()
    {
        _rt = GetComponent<RectTransform>();
        _image = GetComponent<Image>();
    }

    private void Update()
    {
        UpdateToMirrorWorld();
    }
    #endregion
}
