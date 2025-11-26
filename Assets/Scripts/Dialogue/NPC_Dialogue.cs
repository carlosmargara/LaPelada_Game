using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class NPC_Dialogue : ScriptableObject
{
    [Header("Info")]
    public LocalizedString Name;

    [Header("Greeting")]
    public LocalizedString greeting;

    [Header("Chat")]
    public DialogueText[] conversation;

    [Header("Farewell/Goodbye")]
    public LocalizedString farewell;

    [Serializable]
    public class DialogueText
    {
        public LocalizedString text;

        // Helper muy útil:
        public string GetText() => text != null ? text.GetValue() : "";
    }

    // --- Helpers para el juego ---
    public string GetName() => Name != null ? Name.GetValue() : "";
    public string GetGreeting() => greeting != null ? greeting.GetValue() : "";
    public string GetFarewell() => farewell != null ? farewell.GetValue() : "";

    public int GetDialogueCount() => conversation == null ? 0 : conversation.Length;

    public string GetDialogueLine(int index)
    {
        if (conversation == null || index < 0 || index >= conversation.Length)
            return "";
        return conversation[index]?.GetText() ?? "";
    }
}

