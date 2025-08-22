using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using System;

[CreateAssetMenu]
public class Door : ScriptableObject
{
    [Header("Info")]
    public string ID; // ID único de la puerta
    public string requiredKeyID; // ID de la llave necesaria (si está vacío, no necesita llave)

    [Space]

    [Header("Descripton")]
    [TextArea] public string lockedText;
    [TextArea] public string unlockedText;
    public DoorText[] descriptonInterac;

    [Serializable]
    public class DoorText
    {
        [TextArea] public string text;
    }
}
