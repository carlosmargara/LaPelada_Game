using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RandomLoadingMessage : MonoBehaviour
{
    [Header("Referencia al texto en pantalla")]
    public TextMeshProUGUI messageText;

    [Header("Mensajes de ayuda / gameplay")]
    [TextArea(2, 5)]
    public List<string> tips = new List<string>()
    {
        "El crosshair desaparece cuando estás cerca de algo interactuable.",
        "Podés correr, pero no todo el tiempo vas a querer hacerlo.",
        "Revisá bien los rincones. A veces hay cosas importantes escondidas.",
        "La linterna consume tu atención. Usala con cuidado."
    };

    [Header("Mensajes inquietantes / creepy")]
    [TextArea(2, 5)]
    public List<string> creepyMessages = new List<string>()
    {
        "¿Ya la viste?",
        "No mires atrás.",
        "No estás solo.",
        "Ya está acá.",
        "Escuchá bien... a veces avisa."
    };

    [Range(0, 1)]
    [Tooltip("Probabilidad de que aparezca un mensaje creepy (0.3 = 30%)")]
    public float creepyChance = 0.3f;

    void Start()
    {
        ShowRandomMessage();
    }

    void ShowRandomMessage()
    {
        string selectedMessage;

        if (Random.value < creepyChance && creepyMessages.Count > 0)
        {
            selectedMessage = creepyMessages[Random.Range(0, creepyMessages.Count)];
        }
        else if (tips.Count > 0)
        {
            selectedMessage = tips[Random.Range(0, tips.Count)];
        }
        else
        {
            selectedMessage = "";
        }

        if (messageText != null)
        {
            messageText.text = selectedMessage;
        }
    }
}

