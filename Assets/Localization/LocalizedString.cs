using UnityEngine;

[CreateAssetMenu(menuName = "Localization/Localized String")]
public class LocalizedString : ScriptableObject
{
    public string key; // ej: "door_locked", "npc_greeting_juan"

    public string GetValue()
    {
        return LocalizationManager.Instance.GetText(key);
    }
}

