using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LocalizedStringTable", menuName = "Localization/String Table")]
public class LocalizedStringTable : ScriptableObject
{
    [System.Serializable]
    public class Entry
    {
        public string key;      // Ej: "ui_play"
        public string value;    // Ej: "Jugar"
    }

    public List<Entry> entries = new List<Entry>();
}
