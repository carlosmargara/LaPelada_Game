using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class DialogueManager : Singleton<DialogueManager>
{
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI npcNameTMP;
    [SerializeField] private TextMeshProUGUI npcChatTMP;

    public NPC_Intetaction NPC_Interaction {  get; set; }
    public Door_Interaction CurrentDoor { get; set; }

    private Queue<string> dialogueSequence;
    private Queue<string> doorSequence;

    private bool dialogueAmin;
    private bool farewellShown; //despedida mostrada
    private bool isTalking;
    private bool isDoorDescription = false;
    private bool isWorldMessage = false;

    public bool IsTalking => isTalking; //propiedad public que devuelve el resultado del bool isTalking (siempre son publicas estas propertys
                                        //porque necesito usarla desde otro scritp)

    private void Start()
    {
        dialogueSequence = new Queue<string>();
        doorSequence = new Queue<string>();
    }

    private void Update()
    {
        if (!isTalking) return;

        GameStateManager.Instance.LockPlayer();

        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) && isTalking)
        {
            if (isWorldMessage)
            {
                OpenCloseDialoguePanel(false);
                isTalking = false;
                isWorldMessage = false;
                GameStateManager.Instance.UnlockPlayer();
                return;
            }

            if (farewellShown)
            {
                OpenCloseDialoguePanel(false);
                farewellShown = false;
                isTalking = false;
                return;
            }

            if (dialogueAmin)
            {
                ContinueDialogue();
            }
        }
    }

    private void OpenCloseDialoguePanel(bool state)
    {
        dialoguePanel.SetActive(state);
    }

    private void SetUpDialoguePanel(NPC_Dialogue nPC_Dialogue)
    {
        OpenCloseDialoguePanel(true);
        LoadDialogueSequence(nPC_Dialogue); //carga la secuencia de dialogo que hay en el coso que estas interactuando

        npcNameTMP.text = nPC_Dialogue.Name;

        ShowTextAmin(nPC_Dialogue.greeting); //anima el saludo, efecto maquina de escribir 
    }

    private void LoadDialogueSequence(NPC_Dialogue nPC_Dialogue)
    {
        if (nPC_Dialogue.covertation == null || nPC_Dialogue.covertation.Length <= 0) //Verificacion
        {
            return;
        }

        for (int i = 0; i < nPC_Dialogue.covertation.Length; i++) //con esto cargas los array de chats
        {
            dialogueSequence.Enqueue(nPC_Dialogue.covertation[i].text);
        }
    }

    private void ShowTextAmin(string text)
    {
        StartCoroutine(AminText(text));
    }

    private IEnumerator AminText(string text) //Animar texto como maquina de escribir!
    {
        dialogueAmin = false;
        npcChatTMP.text = ""; // Limpia el texto actual (empieza desde cero)
        char[] chars = text.ToCharArray(); // Convierte el string que recibió (por ejemplo "Hola") en un array de caracteres: ['H', 'o', 'l', 'a']

        for (int i = 0; i < chars.Length; i++)
        {
            npcChatTMP.text += chars[i];
            yield return new WaitForSeconds(0.03f);
        }

        dialogueAmin = true;
    }

    private void ContinueDialogue()
    {
        if (isDoorDescription)
        {
            if (doorSequence.Count > 0)
            {
                string nextText = doorSequence.Dequeue();
                ShowTextAmin(nextText);
            }
            else
            {
                OpenCloseDialoguePanel(false);
                isTalking = false;
                isDoorDescription = false;
            }
            return;
        }

        // Diálogo con NPCs
        if (dialogueSequence.Count > 0)
        {
            string nextDialogue = dialogueSequence.Dequeue();
            ShowTextAmin(nextDialogue);
        }
        else
        {
            string goodbye = NPC_Interaction.Dialogue.farewell;
            ShowTextAmin(goodbye);
            farewellShown = true;
        }
    }

    public void StartDialogue(NPC_Intetaction npcInteraction)
    {
        NPC_Interaction = npcInteraction;
        isTalking = true;
        SetUpDialoguePanel(npcInteraction.Dialogue);
    }

    private void LoadDoorSequence(Door currentDoor)
    {
        if (currentDoor.descriptonInterac == null || currentDoor.descriptonInterac.Length <= 0) //Verificacion
        {
            return;
        }

        for (int i = 0; i < currentDoor.descriptonInterac.Length; i++) //con esto cargas los array de chats
        {
            doorSequence.Enqueue(currentDoor.descriptonInterac[i].text);
        }
    }

    #region Logica para mostar mensaje de la puerta (ShowDoorDescription)
    public void ShowDoorDescription(Door_Interaction door)
    {
        CurrentDoor = door;
        OpenCloseDialoguePanel(true);
        isTalking = true;
        isDoorDescription = true;

        npcNameTMP.text = "";

        doorSequence.Clear(); // Por las dudas, limpiamos lo anterior
        LoadDoorSequence(door.Descripcion);

        if (doorSequence.Count > 0)
        {
            string currentText = doorSequence.Dequeue();
            ShowTextAmin(currentText);
        }
        else
        {
            // No hay texto que mostrar
            OpenCloseDialoguePanel(false);
            isTalking = false;
            isDoorDescription = false;
        }
    }
    #endregion

    #region Logica para mostrar mensaje de "Do not Pass" Limite del mapa 
    public void ShowWorldMessage(string mensaje) //Mensaje de limite del mapa
    {
        OpenCloseDialoguePanel(true);
        npcNameTMP.text = "";
        isTalking = true;
        isWorldMessage = true;

        ShowTextAmin(mensaje);
        //StartCoroutine(CloseWorldMessageAfterDelay(3f)); // ajustá el tiempo que querés
    }

    #endregion
}
