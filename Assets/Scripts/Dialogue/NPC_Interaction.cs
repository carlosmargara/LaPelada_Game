using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Intetaction : Interactable
{
    [SerializeField] private NPC_Dialogue npc_dialogue;

    public NPC_Dialogue Dialogue => npc_dialogue;

    public override void Interact()
    {
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.StartDialogue(this);
        }
    }
}
