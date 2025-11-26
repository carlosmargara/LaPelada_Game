using System.Collections;
using UnityEngine;
using TMPro;

public class LoadingDots : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI loadingText;
    [SerializeField] private float dotInterval = 0.5f;

    [SerializeField] private LocalizedString baseText; // 🔥 ahora es una key

    private int dotCount = 0;

    void Start()
    {
        StartCoroutine(AnimateDots());
    }

    IEnumerator AnimateDots()
    {
        while (true)
        {
            dotCount = (dotCount + 1) % 4;
            string dots = new string('.', dotCount);

            loadingText.text = baseText.GetValue() + dots; // 🔥 texto localizado

            yield return new WaitForSeconds(dotInterval);
        }
    }
}

