using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class HoverTextColorChanger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Color originalColor;
    [SerializeField] private Color hoverColor;

    private bool isHighlighted = false; // para evitar sonido repetido

    void Start()
    {
        if (text != null)
            originalColor = text.color;
    }

    // --- Mouse ---
    public void OnPointerEnter(PointerEventData eventData)
    {
        Highlight();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Unhighlight();
    }

    // --- Teclado / Joystick ---
    public void OnSelect(BaseEventData eventData)
    {
        Highlight();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        Unhighlight();
    }

    // --- Métodos comunes ---
    public void Highlight()
    {
        if (text != null)
            text.color = hoverColor;

        if (!isHighlighted)
        {
            AudioManager02.Instance.PlayOneShot("event:/UI/Selection_Sound (Main_Menu)");
            isHighlighted = true;
        }
    }

    public void Unhighlight()
    {
        if (text != null)
            text.color = originalColor;

        isHighlighted = false;
    }
}

