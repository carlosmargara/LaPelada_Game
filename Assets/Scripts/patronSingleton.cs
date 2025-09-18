using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class patronSingleton<T> : MonoBehaviour where T : Component
{
    private static T _instance; //Es donde se guarda la referencia interna del Singleton. No es accesible desde fuera de la clase.
    public static T Instance // Es la propiedad p?blica que permite a otros scripts acceder a la ?nica instancia del Singleton. Si no existe, se crea autom?ticamente.
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<T>();
                if (_instance == null)
                {
                    GameObject gameObject = new GameObject();
                    _instance = gameObject.AddComponent<T>();
                }
            }
            return _instance;
        }
    }

    protected virtual void Awake()
    {
        _instance = this as T;
    }
}

