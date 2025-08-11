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
    public string ID;

    [Space]

    public AudioClip effectDoorSound;

    [Header("Descripton")]
    public DoorText[] descriptonInterac;

    [Serializable]
    public class DoorText
    {
        [TextArea] public string text; 
    }
}
