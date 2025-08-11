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

    public NPC_Intetaction NPC_Interaction { get; set; }
    public Door_Interaction CurrentDoor { get; set; }

    private Queue<string> dialogueSequence;
    private Queue<string> doorSequence;
    private Queue<string> thoughtSequence;
    private bool isThinking = false;

    private float inputCooldown = 0f;
    private bool dialogueAmin;
    private bool farewellShown; //despedida mostrada
    private bool isTalking;
    private bool isDoorDescription = false;
    private bool isWorldMessage = false;

    private string currentText; //variable que guarda que texto se esta mostrando en el momento

    public bool IsTalking => isTalking; //propiedad public que devuelve el resultado del bool isTalking (siempre son publicas estas propertys
                                        //porque necesito usarla desde otro scritp)

    private DiffetentTypes_footSteps_with_FmodEvent footSteps_Player;

    private void Start()
    {
        footSteps_Player = FindObjectOfType<DiffetentTypes_footSteps_with_FmodEvent>();

        dialogueSequence = new Queue<string>();
        doorSequence = new Queue<string>();
        thoughtSequence = new Queue<string>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && isTalking)
        {
            CloseDialogueCompletely();
            Cursor.visible = false;
            return;
        }

        if (inputCooldown > 0f)
        {
            inputCooldown -= Time.deltaTime;
            return; // Espera a que pase el cooldown
        }

        if (!isTalking) return;

        GameStateManager.Instance.LockPlayer();

        //Acelerar el texto si se está escribiendo
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) && !dialogueAmin)
        {
            SkipTextAnimation();
            return; // para que no entre al resto
        }

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

    private void SkipTextAnimation()
    {
        StopAllCoroutines();
        npcChatTMP.text = currentText; // siempre muestra el texto actual completo
        dialogueAmin = true;
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
        currentText = text; //aca le digo que el texto que esta animando es el texto alcual 
        StartCoroutine(AminText(text));
    }

    private IEnumerator AminText(string text) //Animar texto como maquina de escribir!
    {
        dialogueAmin = false;
        npcChatTMP.text = ""; // Limpia el texto actual (empieza desde cero)
        char[] chars = text.ToCharArray(); // Convierte el string que recibi� (por ejemplo "Hola") en un array de caracteres: ['H', 'o', 'l', 'a']

        for (int i = 0; i < chars.Length; i++)
        {
            npcChatTMP.text += chars[i];
            yield return new WaitForSeconds(0.03f);
        }

        dialogueAmin = true;
    }

    private void ContinueDialogue()
    {
        // Pensamientos internos del jugador
        if (isThinking)
        {
            if (thoughtSequence.Count > 0)
            {
                string nextText = thoughtSequence.Dequeue();
                ShowTextAmin(nextText);
            }
            else
            {
                OpenCloseDialoguePanel(false);
                isTalking = false;
                isThinking = false;
            }
            return;
        }

        // Descripciones de puertas
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

        // Di�logo con NPCs
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
        inputCooldown = 0.1f; // Pequeño delay de 0.1 segundos
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
        inputCooldown = 0.1f; // Pequeño delay de 0.1 segundos
    }
    #endregion

    /*Funcion que reinicia toda la logica del dialogo
    para que cuando apretas escape y salir del la interacion
    al volver a hablar con el npc empiece todo del principio
    */
    private void CloseDialogueCompletely()
    {
        StopAllCoroutines();
        OpenCloseDialoguePanel(false);
        isTalking = false;
        farewellShown = false;
        isDoorDescription = false;
        isWorldMessage = false;
        isThinking = false;

        dialogueSequence.Clear();
        doorSequence.Clear();
        thoughtSequence.Clear();

        GameStateManager.Instance.UnlockPlayer();
    }

    #region Logica para mostrar mensaje de "Do not Pass" Limite del mapa 
    public void ShowWorldMessage(string mensaje) //Mensaje de limite del mapa
    {
        OpenCloseDialoguePanel(true);
        npcNameTMP.text = "";
        isTalking = true;
        isWorldMessage = true;

        inputCooldown = 0.1f; // Pequeño delay de 0.1 segundos
        ShowTextAmin(mensaje);
        //StartCoroutine(CloseWorldMessageAfterDelay(3f)); // ajust� el tiempo que quer�s
    }

    #endregion

    public void ShowThoughts(PlayerThoughts thoughtAsset)
    {
        footSteps_Player.StopAllFootsteps(); //Detiene el sonido de pasos

        thoughtSequence.Clear();
        foreach (var thought in thoughtAsset.thoughts)
        {
            thoughtSequence.Enqueue(thought.text);
        }

        if (thoughtSequence.Count == 0) return;

        OpenCloseDialoguePanel(true);
        npcNameTMP.text = ""; // Sin nombre
        isTalking = true;
        isThinking = true;

        string currentText = thoughtSequence.Dequeue();
        ShowTextAmin(currentText);
        inputCooldown = 0.1f;
    }
}
