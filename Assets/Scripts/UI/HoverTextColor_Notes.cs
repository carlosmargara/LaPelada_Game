using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class HoverTextColor_Notes : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    [Header("Button Settings")]
    [SerializeField] private TextMeshProUGUI buttonText;

    [Header("Colors")]
    private Color32 normalColor = new Color32(255, 255, 255, 132);
    private Color32 hoverColor = new Color32(85, 86, 86, 132);

    private void Awake()
    {
        if (buttonText == null)
            buttonText = GetComponentInChildren<TextMeshProUGUI>();
    }

    // --- Mouse ---
    public void OnPointerEnter(PointerEventData eventData) => SetHover(true);
    public void OnPointerExit(PointerEventData eventData) => SetHover(false);

    // --- Teclado / Gamepad ---
    public void OnSelect(BaseEventData eventData) => SetHover(true);
    public void OnDeselect(BaseEventData eventData) => SetHover(false);

    private void SetHover(bool hovering)
    {
        // Cambiar color del texto
        buttonText.color = hovering ? hoverColor : normalColor;

        // Reproducir sonido solo al entrar en hover
        if (hovering)
            AudioManager02.Instance.PlayOneShot("event:/UI/Selection_Sound (Inventary)");
    }
}



