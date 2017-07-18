using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class EditModesHandler : Singleton<EditModesHandler>
{
    #region public serialised vars
    [SerializeField]
    GameObject[] _clickAreas;
    #endregion


    #region private protected vars
    EEditModeType _activeMode = EEditModeType.None;

    EditModesPanel _panel;
    #endregion


    #region pub methods
    public EEditModeType ActiveMode { get { return _activeMode; } }

    public void OnClickForSpawn(Vector3 worldPos)
    {
        if (_activeMode == EEditModeType.Create)
        {
            EnemyHandler.Instance.SpawnEnemy(worldPos);
        }
    }
    #endregion


    #region private protected methods
    #endregion


    #region events
    void OnModeChanged(EEditModeType newMode)
    {
        _activeMode = newMode;
        for (int i = 0; i < _clickAreas.Length; i++)
            _clickAreas[i].SetActive(_activeMode == EEditModeType.Create);
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
