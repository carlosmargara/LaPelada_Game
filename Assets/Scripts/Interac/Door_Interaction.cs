using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class Door_Interaction : Interactable
{
    [SerializeField] private Door ref_ScriptableObjets;

    public Door Descripcion => ref_ScriptableObjets; 
    
    //private bool isOpen = false;

    public override void Interact()
    {
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.ShowDoorDescription(this); // Le pasás el script, no el ScriptableObject
            AudioManager.Instance.PlaySoundFX(ref_ScriptableObjets.effectDoorSound, 0.5f);
        }
    }
}
