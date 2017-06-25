using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class EnemyHandler : Singleton<EnemyHandler>
{
    #region public serialised vars
    [SerializeField]
    UnitEnemy _enemyPrefabWorld;
    #endregion


    #region private protected vars
    List<UnitBase> _enemies = new List<UnitBase>();
    //Dictionary<UnitEnemy, UnitDisplay> _enemies = new Dictionary<UnitEnemy, UnitDisplay>();
    #endregion


    #region pub methods
    public UnitEnemy SpawnEnemy(Vector3 position)
    {
        UnitEnemy newInst = Instantiate(_enemyPrefabWorld, position, Quaternion.identity, this.transform) as UnitEnemy;
        return newInst;
    }

    public void RegisterEnemy(UnitEnemy unit)
    {
        UnitDisplay unitDisplay = RadarDisplayController.Instance.CreateUnitDisplay(unit);
        _enemies.Add(unit);
        unit.UnitDisplay = unitDisplay;
        unitDisplay.WorldUnit = unit;
    }

    public void DeleteEnemy(UnitEnemy unit)
    {
        Assert.IsTrue(_enemies.Contains(unit));

        UnitDisplay unitDisplay = unit.UnitDisplay;
        _enemies.Remove(unit);
        Destroy(unitDisplay);
        Destroy(unit);
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
