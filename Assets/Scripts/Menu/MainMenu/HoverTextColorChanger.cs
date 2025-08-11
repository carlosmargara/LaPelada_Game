using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;


public class HoverTextColorChanger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Color originalColor;
    [SerializeField] private Color hoverColor;

    void Start()
    {
        if (text != null)
        {
            originalColor = text.color;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (text != null)
            text.color = hoverColor;
        AudioManager02.Instance.PlayOneShot("event:/UI/Selection_Sound (Main_Menu)");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (text != null)
            text.color = originalColor;
    }
}
