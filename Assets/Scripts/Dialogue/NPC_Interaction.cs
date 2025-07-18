using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Intetaction : Interactable
{
    private DiffetentTypes_footSteps_with_FmodEvent footSteps_Player;
    [SerializeField] private NPC_Dialogue npc_dialogue;

    public NPC_Dialogue Dialogue => npc_dialogue;

    void Start()
    {
        footSteps_Player = FindObjectOfType<DiffetentTypes_footSteps_with_FmodEvent>();
    }

    public override void Interact()
    {
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.StartDialogue(this);
            footSteps_Player.StopAllFootsteps();
        }
    }
}
