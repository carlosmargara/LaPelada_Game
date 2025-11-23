using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Door")]
public class Door : ScriptableObject
{
    [Header("Info")]
    public string ID;
    public string requiredKeyID;

    [Space]

    [Header("Localization")]
    public LocalizedString lockedText;
    public LocalizedString unlockedText;
    public LocalizedString[] descriptionInterac;

    [Serializable]
    public class LocalizedStringArrayElement
    {
        public LocalizedString text;
    }

    // Métodos (igual que en Item)
    public string GetLockedText() => lockedText.GetValue();
    public string GetUnlockedText() => unlockedText.GetValue();
    public int GetDescriptionCount() => descriptionInterac.Length;

    public string GetDescriptionInterac(int index)
    {
        if (index < 0 || index >= descriptionInterac.Length)
            return "";

        return descriptionInterac[index].GetValue();
    }
}
