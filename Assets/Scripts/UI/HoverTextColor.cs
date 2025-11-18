using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class HoverTextColor : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    [SerializeField] private TextMeshProUGUI buttonText;

    private Color32 normalColor = new Color32(255, 255, 255, 132);
    private Color32 hoverColor = new Color32(85, 86, 86, 132);

    private void Awake()
    {
        if (buttonText == null)
            buttonText = GetComponentInChildren<TextMeshProUGUI>();
    }

    // 🖱️ Mouse
    public void OnPointerEnter(PointerEventData eventData) => SetHover(true);
    public void OnPointerExit(PointerEventData eventData) => SetHover(false);

    // 🎮 Teclado / Gamepad
    public void OnSelect(BaseEventData eventData) => SetHover(true);
    public void OnDeselect(BaseEventData eventData) => SetHover(false);

    private void SetHover(bool hovering)
    {
        if (buttonText == null) return;

        buttonText.color = hovering ? hoverColor : normalColor;

        if (hovering)
            AudioManager02.Instance.PlayOneShot("event:/UI/Selection_Sound (Inventary)");
    }
}



