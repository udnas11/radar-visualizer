using UnityEngine;
using UnityEngine.Assertions;

public class Singleton<T> : MonoBehaviour where T : class
{
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("Singleton error! make sure you have a " + typeof(T).FullName + " in your scene");
            }
            return _instance;
        }
    }

    public void RegisterSingleton(T instance)
    {
        Assert.IsNull(_instance);
        Assert.IsNotNull(instance);
        _instance = instance;
    }

    public static void ResetInstance()
    {
        _instance = null;
    }

    private static T _instance;
}