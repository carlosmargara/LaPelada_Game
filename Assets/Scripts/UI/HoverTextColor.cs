using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;


public class HoverTextColor : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public enum ButtonType { Si, No }

    [SerializeField] private ButtonType tipoBoton;
    [SerializeField] private TextMeshProUGUI buttonText;
    [SerializeField] private GameObject flecha_uiSi;
    [SerializeField] private GameObject flecha_uiNo;

    private Color32 normalColor = new Color32(255, 255, 255, 132); // blanco con alfa a la mitad
    private Color32 hoverColor = new Color32(85, 86, 86, 132);     // gris oscuro con alfa a la mitad

    private void Awake()
    {
        if (buttonText == null)
            buttonText = GetComponentInChildren<TextMeshProUGUI>();
    }

    private void Start()
    {
        if (flecha_uiSi != null) flecha_uiSi.SetActive(false);
        if (flecha_uiNo != null) flecha_uiNo.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        buttonText.color = hoverColor;

        if (tipoBoton == ButtonType.Si && flecha_uiSi != null)
            flecha_uiSi.SetActive(true);
        else if (tipoBoton == ButtonType.No && flecha_uiNo != null)
            flecha_uiNo.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        buttonText.color = normalColor;

        if (tipoBoton == ButtonType.Si && flecha_uiSi != null)
            flecha_uiSi.SetActive(false);
        else if (tipoBoton == ButtonType.No && flecha_uiNo != null)
            flecha_uiNo.SetActive(false);
    }
}

