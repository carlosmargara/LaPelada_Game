using UnityEngine;
using UnityEngine.UI;
using TMPro; // si no tenés TextMeshPro en tu proyecto, ver nota más abajo

[RequireComponent(typeof(RectTransform))]
public class LocalizedText : MonoBehaviour
{
    public string key;

    // intentamos TMP primero, si no está usamos legacy Text
    private TMP_Text tmpText;
    private Text uiText;

    void Awake()
    {
        tmpText = GetComponent<TMP_Text>();
        uiText = GetComponent<Text>();
        RefreshText();
    }

    public void RefreshText()
    {
        if (LocalizationManager.Instance == null) return;

        string localized = LocalizationManager.Instance.GetText(key);

        if (tmpText != null)
        {
            tmpText.text = localized;
        }
        else if (uiText != null)
        {
            uiText.text = localized;
        }
        else
        {
            // si no tiene componente de texto, log para que lo revises
            Debug.LogWarning($"LocalizedText en '{gameObject.name}' no encontró componente TMP_Text ni Text. Key: {key}");
        }
    }
}

