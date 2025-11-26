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
        public LocalizedString text;

        public string GetText()
        {
            return text != null ? text.GetValue() : "";
        }
    }

    // -----------------------
    //       HELPERS
    // -----------------------

    public string GetThought(int index)
    {
        if (thoughts == null || index < 0 || index >= thoughts.Length)
            return "";

        return thoughts[index].GetText();
    }

    public int GetThoughtCount()
    {
        return thoughts != null ? thoughts.Length : 0;
    }
}

