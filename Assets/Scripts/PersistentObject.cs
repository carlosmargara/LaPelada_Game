using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentObject : MonoBehaviour
{
    private static bool _exists = false;

    void Awake()
    {
        if (_exists)
        {
            Destroy(gameObject);
            return;
        }

        _exists = true;
        DontDestroyOnLoad(gameObject);
    }
}
