using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


[CreateAssetMenu(menuName = "Dialogue/PlayerThoughts")]
public class PlayerThoughts : ScriptableObject
{
    [Header("Pensamientos internos del jugador")]
    public ThoughtText[] thoughts;

    [Serializable]
    public class ThoughtText
    {
        [TextArea] public string text;
    }
}
