using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHandler : Singleton<EnemyHandler>
{
    #region public serialised vars
    [SerializeField]
    UnitBase _enemyPrefabWorld;
    #endregion


    #region private protected vars
    List<UnitBase> _enemies = new List<UnitBase>();
    #endregion


    #region pub methods
    public UnitBase SpawnEnemy(Vector3 position)
    {
        UnitBase newInst = Instantiate(_enemyPrefabWorld, position, Quaternion.identity, this.transform);
        if (newInst != null)
            _enemies.Add(newInst);
        return newInst;
    }

    public bool IsVisible()
    {
        return true;
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
