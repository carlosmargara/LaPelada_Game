using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note_Interaction : Interactable
{
    [SerializeField] private NoteData _data;
    public NoteData Data => _data;

    public override void Interact()
    {
        NoteManager.Instance.noteInteraction = this;
        NoteManager.Instance.ShowInteracText_First(_data);
        //NoteManager.Instance.ShowSetupNotes(_data);
        Debug.Log("Estas leyendo la nota");
    }
}
