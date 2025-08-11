using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class Door_Interaction : Interactable
{
    private DiffetentTypes_footSteps_with_FmodEvent footSteps_Player;
    [SerializeField] private Door ref_ScriptableObjets;

    public Door Descripcion => ref_ScriptableObjets;

    //private bool isOpen = false;

    void Start()
    {
        footSteps_Player = FindObjectOfType<DiffetentTypes_footSteps_with_FmodEvent>();
    }

    public override void Interact()
    {
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.ShowDoorDescription(this); // Le pas�s el script, no el ScriptableObject
            //AudioManager.Instance.PlaySoundFX(ref_ScriptableObjets.effectDoorSound, 0.5f);
            AudioManager02.Instance.PlayOneShot("event:/Fxs/Closed_Door");
            footSteps_Player.StopAllFootsteps();
        }
    }
}
