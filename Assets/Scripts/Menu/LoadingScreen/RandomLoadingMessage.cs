using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RandomLoadingMessage : MonoBehaviour
{
    public TextMeshProUGUI messageText;

    [Header("Mensajes de ayuda / gameplay (en SO)")]
    public List<LocalizedString> tips;

    [Header("Mensajes inquietantes / creepy (en SO)")]
    public List<LocalizedString> creepyMessages;

    [Range(0, 1)]
    public float creepyChance = 0.3f;

    private void OnEnable()
    {
        LocalizationManager.OnLanguageChanged += RefreshText;
    }

    private void OnDisable()
    {
        LocalizationManager.OnLanguageChanged -= RefreshText;
    }

    void Start()
    {
        ShowRandomMessage();
    }

    /// <summary>
    /// Se llama cuando cambia el idioma
    /// </summary>
    void RefreshText()
    {
        ShowRandomMessage();
    }

    void ShowRandomMessage()
    {
        string selectedMessage;

        // Elegís aleatoriamente de nuevo (queda bien porque cada vez que se cambia idioma re-ruleta)
        if (Random.value < creepyChance && creepyMessages.Count > 0)
        {
            selectedMessage = creepyMessages[Random.Range(0, creepyMessages.Count)].GetValue();
        }
        else if (tips.Count > 0)
        {
            selectedMessage = tips[Random.Range(0, tips.Count)].GetValue();
        }
        else
        {
            selectedMessage = "";
        }

        if (messageText != null)
            messageText.text = selectedMessage;
    }
}

