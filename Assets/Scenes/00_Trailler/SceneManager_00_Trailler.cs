using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.UI;
using TMPro;
using UnityEditor.Rendering;
using System;


public class SceneManager_00_Trailler : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera CM_01;
    [SerializeField] private TextMeshProUGUI textUI; // Referencia directa al texto
    [Space]
    [SerializeField] private float currentTime;
    [SerializeField] private float currentTimePELADA;
    public float time;
    public float timePELADA;

    [Header("Parpadeo")]
    [SerializeField] private float blinkSpeed = 1.5f; // Velocidad del parpadeo

    private Color originalColor;

    Pelada_00_Trailler pelada_00_Trailler;

    void Start()
    {
        pelada_00_Trailler = FindObjectOfType<Pelada_00_Trailler>();

        time = currentTime;
        timePELADA = currentTimePELADA;

        if (textUI != null)
        {
            originalColor = textUI.color;
            textUI.enabled = false; // Arranca oculto
        }
    }

    void Update()
    {
        if (time > 0f)
        {
            time -= Time.deltaTime;
            if (time <= 0f)
            {
                time = 0f; // Evita que baje de cero
                CM_01.Priority = 20;
            }
        }

        if (CM_01.Priority == 20 && textUI != null)
        {
            textUI.enabled = true;

            StartCoroutine(BlinkText());

            /*
            // Parpadeo con alpha usando seno
            float alpha = Mathf.Abs(Mathf.Sin(Time.time * blinkSpeed));
            textUI.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            */
        }
        else if (textUI != null)
        {
            textUI.enabled = false;
        }

        if (timePELADA > 0f)
        {
            timePELADA -= Time.deltaTime;
            if (timePELADA <= 0)
            {
                timePELADA = 0;
                pelada_00_Trailler.IniciarSalto();
            }
        }
    }

    IEnumerator BlinkText()
    {
        while (true)
        {
            textUI.enabled = !textUI.enabled;
            yield return new WaitForSeconds(blinkSpeed);
        }
    }
}
