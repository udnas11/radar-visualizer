// Made by Alexandru Romanciuc <sanromanciuc@gmail.com>
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;

public class ToggleGroupPlus : ToggleGroup
{
    #region public serialised vars
    #endregion


    #region private protected vars
    #endregion


    #region pub methods
    public void RegisterTogglePlus(Toggle toggle)
    {
        base.RegisterToggle(toggle);
        toggle.onValueChanged.AddListener(OnToggleChanged);
    }

    public void UnregisterTogglePlus(Toggle toggle)
    {
        base.UnregisterToggle(toggle);
        toggle.onValueChanged.RemoveListener(OnToggleChanged);
    }
    #endregion


    #region private protected methods
    #endregion


    #region events
    void OnToggleChanged(bool newState)
    {
        foreach (Toggle toggle in ActiveToggles())
        {
            
        }
    }
    #endregion


    #region mono events
    #endregion
}
