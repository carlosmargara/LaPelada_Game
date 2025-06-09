using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public class GameOver : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TextMeshProUGUI text; // Asignalo desde el inspector
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color hoverColor = Color.yellow;

    private void Reset()
    {
        // Esto se ejecuta automáticamente cuando agregás el script en el editor
        text = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (text != null)
            text.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (text != null)
            text.color = normalColor;
    }
}
