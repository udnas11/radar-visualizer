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
    [SerializeField]
    Transform _spawnTransform;
    #endregion


    #region private protected vars
    List<UnitEnemy> _enemies = new List<UnitEnemy>();
    #endregion


    #region pub methods
    public List<UnitEnemy> Enemies { get { return _enemies; } }

    public UnitEnemy SpawnEnemy(Vector3 position)
    {
        UnitEnemy newInst = Instantiate(_enemyPrefabWorld, position, Quaternion.Euler(0f, 180f, 0f), _spawnTransform) as UnitEnemy;
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
        Destroy(unitDisplay.gameObject);
        Destroy(unit.gameObject);
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
