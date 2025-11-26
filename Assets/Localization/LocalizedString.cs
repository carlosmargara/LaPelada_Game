using UnityEngine;

[CreateAssetMenu(menuName = "Localization/Localized String")]
public class LocalizedString : ScriptableObject
{
    public string key;

    public string GetValue()
    {
        return LocalizationManager.Instance.GetText(key);
    }

    public void OnLanguageChanged()
    {
        // VACÍO — NO debe llamar al DialogueManager ni a otro manager
    }
}

