using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Component
{
    private static T _instance; //Es donde se guarda la referencia interna del Singleton. No es accesible desde fuera de la clase.
    public static T Instance // Es la propiedad p�blica que permite a otros scripts acceder a la �nica instancia del Singleton. Si no existe, se crea autom�ticamente.
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<T>();
                if (_instance == null)
                {
                    GameObject gameObject = new GameObject(typeof(T).Name + " (Singleton)");
                    _instance = gameObject.AddComponent<T>();
                }
            }
            return _instance;
        }
    }

    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
            DontDestroyOnLoad(gameObject); // Hace persistente al singleton
        }
        else if (_instance != this)
        {
            Destroy(gameObject); // Evita duplicados
        }
    }
}