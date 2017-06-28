using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class SideControlsLayout : MonoBehaviour
{
    #region public serialised vars
    [SerializeField]
    RectTransform _otherRT;
    #endregion


    #region private protected vars
    RectTransform _rt;
    #endregion


    #region pub methods
    #endregion


    #region private protected methods
    #endregion


    #region events
    #endregion


    #region mono events
    private void Start()
    {
        _rt = GetComponent<RectTransform>();
        _rt.offsetMax = new Vector2(-_otherRT.rect.width, 0f);
    }
    #endregion
}
