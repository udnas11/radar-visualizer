using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class EditModesPanel : MonoBehaviour
{
    public event Action<EEditModeType> OnToggleSelected;

    #region public serialised vars
    [SerializeField]
    Toggle _toggleCreate;
    [SerializeField]
    Toggle _toggleMove;
    [SerializeField]
    Toggle _toggleDelete;
    [SerializeField]
    Text _tipText;
    [SerializeField]
    ToggleGroup _toggleGroup;
    #endregion


    #region private protected vars
    #endregion


    #region pub methods
    #endregion


    #region private protected methods    
    void ChangeHintText(EEditModeType type)
    {
        switch (type)
        {
            case EEditModeType.None:
                _tipText.text = "";
                break;
            case EEditModeType.Create:
                _tipText.text = "Left click on Top View to create new enemy";
                break;
            case EEditModeType.Move:
                _tipText.text = "Left click on an enemy to move it";
                break;
            case EEditModeType.Delete:
                _tipText.text = "Left click on an enemy to delete it";
                break;
        }
    }
    #endregion


    #region events
    void OnCreateToggled(bool val)
    {
        EEditModeType newType = val ? EEditModeType.Create : EEditModeType.None;
        ChangeHintText(newType);
        if (OnToggleSelected != null)
            OnToggleSelected(newType);
    }

    void OnMoveToggled(bool val)
    {
        EEditModeType newType = val ? EEditModeType.Move : EEditModeType.None;
        ChangeHintText(newType);
        if (OnToggleSelected != null)
            OnToggleSelected(newType);
    }

    void OnDeleteToggled(bool val)
    {
        EEditModeType newType = val ? EEditModeType.Delete : EEditModeType.None;
        ChangeHintText(newType);
        if (OnToggleSelected != null)
            OnToggleSelected(newType);
    }
    #endregion


    #region mono events
    private void Awake()
    {
        _toggleCreate.onValueChanged.AddListener(OnCreateToggled);
        _toggleMove.onValueChanged.AddListener(OnMoveToggled);
        _toggleDelete.onValueChanged.AddListener(OnDeleteToggled);
    }
    #endregion
}
