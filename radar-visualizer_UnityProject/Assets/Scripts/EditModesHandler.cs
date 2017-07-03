using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class EditModesHandler : Singleton<EditModesHandler>
{
    #region public serialised vars
    #endregion


    #region private protected vars
    EEditModeType _activeMode = EEditModeType.None;

    EditModesPanel _panel;
    #endregion


    #region pub methods
    public EEditModeType ActiveMode { get { return _activeMode; } }

    public void OnClickForSpawn(Vector3 worldPos)
    {

    }
    #endregion


    #region private protected methods
    #endregion


    #region events
    void OnModeChanged(EEditModeType newMode)
    {
        _activeMode = newMode;
    }
    #endregion


    #region mono events
    private void Awake()
    {
        RegisterSingleton(this);

        _panel = FindObjectOfType<EditModesPanel>();
        _panel.OnToggleSelected += OnModeChanged;
    }

    private void Start()
    {
        GlobalInputHandler.Instance.OnClickAreaTrigger += OnClickForSpawn;
    }
    #endregion
}
