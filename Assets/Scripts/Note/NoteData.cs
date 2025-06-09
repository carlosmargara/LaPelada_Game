using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "NewNote", menuName = "Note")]
public class NoteData : ScriptableObject
{
    public string noteTitle;
    public Sprite foregroundImage;
    public bool activaIA;

    [Header("Texto Interac")]
    [TextArea] public string interacText01;
    [TextArea] public string interacText02;
    
    [Space]        
    
    [Header("Texto Notas")]
    [TextArea(5,8)] public List<string> pages;
}
