using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LoadingDots : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI loadingText; // Asignalo en el Inspector
    [SerializeField] private float dotInterval = 0.5f; // Tiempo entre cada cambio

    [SerializeField] private string baseText = "Cargando";
    private int dotCount = 0;

    void Start()
    {
        StartCoroutine(AnimateDots());
    }

    IEnumerator AnimateDots()
    {
        while (true)
        {
            dotCount = (dotCount + 1) % 4; // 0, 1, 2, 3
            string dots = new string('.', dotCount);
            loadingText.text = baseText + dots;
            yield return new WaitForSeconds(dotInterval);
        }
    }
}
