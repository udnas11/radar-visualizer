using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Player : Singleton<Player>
{
    #region public serialised vars
    #endregion


    #region private protected vars
    #endregion


    #region pub methods
    public void SetAltitude(float alt)
    {
        Vector3 newPos = transform.position;
        newPos.y = alt;
        transform.position = newPos;
    }
    #endregion


    #region private protected methods
    #endregion


    #region events
    #endregion


    #region mono events
    private void Awake()
    {
        RegisterSingleton(this);
    }
    #endregion
}
