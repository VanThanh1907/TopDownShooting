using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Singleton
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    public static T Instance => instance ??= FindObjectOfType<T>();

    protected virtual void Awake()
    {
        //Neu null thi gan bang thang hien tai.
        if (instance == null)
        {
            instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
