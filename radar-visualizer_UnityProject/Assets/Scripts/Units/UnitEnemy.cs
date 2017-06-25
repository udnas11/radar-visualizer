using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class UnitEnemy : UnitBase
{
    #region public serialised vars
    [HideInInspector]
    public UnitDisplay UnitDisplay;
    #endregion


    #region private protected vars
    MeshRenderer _meshRenderer;
    bool _isVisible;
    #endregion


    #region pub methods
    #endregion


    #region private protected methods
    public override void UpdateVisual()
    {
        base.UpdateVisual();

        bool visible = IsVisible();

        if (visible != _isVisible)
        {
            _isVisible = visible;

            _meshRenderer.material.color = visible ? Color.yellow : Color.red;

            if (UnitDisplay != null)
                UnitDisplay.SetVisible(visible);
        }
    }
    #endregion


    #region events
    #endregion


    #region mono events
    protected virtual void Update()
    {
        UpdateVisual();
    }

    protected override void Start()
    {
        base.Start();
        EnemyHandler.Instance.RegisterEnemy(this);
    }

    protected override void Awake()
    {
        base.Awake();

        _meshRenderer = GetComponent<MeshRenderer>();
    }
    #endregion
}
