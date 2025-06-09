using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class NPC_Dialogue : ScriptableObject
{
    [Header("Info")]
    public string Name;

    [Header("Greeting")]
    [TextArea] public string greeting; //Saludo

    [Header("Chat")]
    public DialogueText[] covertation; //Conversacion

    [Header("Farewell/Goodbye")]
    [TextArea] public string farewell; //Despedida

    [Serializable]
    public class DialogueText
    {
        [TextArea] public string text;
    }
}
