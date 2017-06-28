using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class ControlsPanel : MonoBehaviour
{
    #region public serialised vars
    [SerializeField]
    Text _altitudeText;
    [SerializeField]
    Scrollbar _altitudeScroll;

    [SerializeField, Space]
    Text _radarElevationText;
    [SerializeField]
    Scrollbar _radarElevationScrollbar;
    #endregion


    #region private protected vars
    #endregion


    #region pub methods
    public void OnAltitudeChanged(float input)
    {
        input = input * 40f;
        _altitudeText.text = input.ToString();
        Player.Instance.SetAltitude(Math.AngelsToNM(input));
    }

    public void OnRadarElevationScrollChanged(float input)
    {
        RadarDisplayController.Instance.SetRadarElevation(Mathf.Lerp(-30f, 30f, input));
    }
    #endregion


    #region private protected methods
    #endregion


    #region events
    void OnRadarRotationChanged(Vector2 newRot)
    {
        _radarElevationText.text = newRot.y.ToString("##.");
        _radarElevationScrollbar.onValueChanged.RemoveListener(OnRadarElevationScrollChanged);
        _radarElevationScrollbar.value = Mathf.InverseLerp(-30f, 30f, newRot.y);
        _radarElevationScrollbar.onValueChanged.AddListener(OnRadarElevationScrollChanged);
    }
    #endregion


    #region mono events
    private void Awake()
    {
        _altitudeScroll.onValueChanged.AddListener(OnAltitudeChanged);
        _altitudeScroll.numberOfSteps = 41;

        _radarElevationScrollbar.onValueChanged.AddListener(OnRadarElevationScrollChanged);
    }

    private void Start()
    {
        RadarDisplayController.Instance.OnRadarConeRotationChange += OnRadarRotationChanged;
    }
    #endregion
}
