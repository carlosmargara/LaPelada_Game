using UnityEngine;
using UnityEngine.UI;
using TMPro;

/*Este script lo que hace es agregar un efecto de
 * tembleque a las letras de la UI, actua sobre los
 * TextMeshProUGUI de una escena */

public class TextJitterManager : MonoBehaviour
{
    public float intensity = 1f;
    public float speed = 60f;

    private RectTransform[] textRects;
    private Vector2[] originalPositions;
    private float timer;

    void Start()
    {
        TextMeshProUGUI[] tmpTexts = GetComponentsInChildren<TextMeshProUGUI>(true);
        Text[] uiTexts = GetComponentsInChildren<Text>(true);

        int total = tmpTexts.Length + uiTexts.Length;
        textRects = new RectTransform[total];
        originalPositions = new Vector2[total];

        int i = 0;
        foreach (var text in tmpTexts)
        {
            textRects[i] = text.GetComponent<RectTransform>();
            originalPositions[i] = textRects[i].anchoredPosition;
            i++;
        }

        foreach (var text in uiTexts)
        {
            textRects[i] = text.GetComponent<RectTransform>();
            originalPositions[i] = textRects[i].anchoredPosition;
            i++;
        }
    }

    void Update()
    {
        timer += Time.deltaTime * speed;

        for (int i = 0; i < textRects.Length; i++)
        {
            float offsetX = (Mathf.PerlinNoise(timer + i, 0f) - 0.5f) * intensity;
            float offsetY = (Mathf.PerlinNoise(0f, timer + i) - 0.5f) * intensity;

            textRects[i].anchoredPosition = originalPositions[i] + new Vector2(offsetX, offsetY);
        }
    }

    void OnDisable()
    {
        for (int i = 0; i < textRects.Length; i++)
        {
            if (textRects[i] != null)
                textRects[i].anchoredPosition = originalPositions[i];
        }
    }
}

