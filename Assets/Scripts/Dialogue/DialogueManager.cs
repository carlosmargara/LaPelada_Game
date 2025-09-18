using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueManager : Singleton<DialogueManager>
{
    [Header("UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI npcNameTMP;
    [SerializeField] private TextMeshProUGUI npcChatTMP;

    [Header("Settings")]
    [SerializeField] private float typingSpeed = 0.03f;
    [SerializeField] private float inputCooldownDuration = 0.1f;

    public NPC_Intetaction NPC_Interaction { get; private set; }
    public Door_Interaction CurrentDoor { get; private set; }

    private enum DialogueKind { None, NPC, Door, Thought, WorldMessage }
    private DialogueKind currentKind = DialogueKind.None;

    private readonly Queue<string> dialogueQueue = new Queue<string>();
    private readonly Queue<string> doorQueue = new Queue<string>();
    private readonly Queue<string> thoughtQueue = new Queue<string>();

    private Coroutine typingCoroutine;
    private bool isTextAnimating = false;
    private float inputCooldown = 0f;
    private bool farewellShown = false;
    private string currentText = "";

    private DiffetentTypes_footSteps_with_FmodEvent footSteps_Player;

    public bool IsTalking => currentKind != DialogueKind.None;

    private int dialoguePriority = 2; // prioridad para bloquear al jugador

    private void Start()
    {
        footSteps_Player = FindObjectOfType<DiffetentTypes_footSteps_with_FmodEvent>();
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
    }

    private void Update()
    {
        if (!IsTalking) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseDialogueCompletely();
            return;
        }

        if (inputCooldown > 0f)
        {
            inputCooldown -= Time.deltaTime;
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            if (isTextAnimating)
            {
                SkipTextAnimation();
                return;
            }

            if (farewellShown)
            {
                EndDialogue();
                return;
            }

            ShowNext();
        }
    }

    #region Mostrar texto

    private void ShowNext()
    {
        string next = null;

        switch (currentKind)
        {
            case DialogueKind.NPC:
                if (dialogueQueue.Count > 0) next = dialogueQueue.Dequeue();
                else
                {
                    if (NPC_Interaction != null && NPC_Interaction.Dialogue != null && !string.IsNullOrEmpty(NPC_Interaction.Dialogue.farewell))
                    {
                        next = NPC_Interaction.Dialogue.farewell;
                        farewellShown = true;
                    }
                    else { EndDialogue(); return; }
                }
                break;

            case DialogueKind.Door:
                if (doorQueue.Count > 0) next = doorQueue.Dequeue();
                else { EndDialogue(); return; }
                break;

            case DialogueKind.Thought:
                if (thoughtQueue.Count > 0) next = thoughtQueue.Dequeue();
                else { EndDialogue(); return; }
                break;

            case DialogueKind.WorldMessage:
                EndDialogue();
                return;

            default: return;
        }

        if (next != null) ShowTextAnim(next);
    }

    private void ShowTextAnim(string text)
    {
        currentText = text;
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypewriterRoutine(text));
    }

    private IEnumerator TypewriterRoutine(string text)
    {
        isTextAnimating = true;
        npcChatTMP.text = "";

        foreach (char c in text)
        {
            npcChatTMP.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTextAnimating = false;
        typingCoroutine = null;
    }

    private void SkipTextAnimation()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        npcChatTMP.text = currentText;
        isTextAnimating = false;
    }

    #endregion

    #region API pública para iniciar diálogos

    public void StartDialogue(NPC_Intetaction npcInteraction)
    {
        if (npcInteraction == null || npcInteraction.Dialogue == null) return;

        NPC_Interaction = npcInteraction;
        currentKind = DialogueKind.NPC;
        dialogueQueue.Clear();

        foreach (var c in npcInteraction.Dialogue.covertation)
            if (c != null) dialogueQueue.Enqueue(c.text);

        npcNameTMP.text = npcInteraction.Dialogue.Name;
        farewellShown = false;

        OpenCloseDialoguePanel(true);
        GameStateManager.Instance.LockPlayer(priority: dialoguePriority);
        inputCooldown = inputCooldownDuration;

        if (!string.IsNullOrEmpty(npcInteraction.Dialogue.greeting))
            ShowTextAnim(npcInteraction.Dialogue.greeting);
        else
            ShowNext();
    }

    public void ShowDoorDescription(Door_Interaction door)
    {
        if (door == null || door.Descripcion == null) return;

        CurrentDoor = door;
        currentKind = DialogueKind.Door;
        doorQueue.Clear();

        foreach (var d in door.Descripcion.descriptonInterac)
            if (d != null) doorQueue.Enqueue(d.text);

        npcNameTMP.text = "";
        farewellShown = false;

        OpenCloseDialoguePanel(true);
        GameStateManager.Instance.LockPlayer(priority: dialoguePriority);
        inputCooldown = inputCooldownDuration;

        ShowNext();
    }

    public void ShowWorldMessage(string mensaje)
    {
        if (string.IsNullOrEmpty(mensaje)) return;

        currentKind = DialogueKind.WorldMessage;
        npcNameTMP.text = "";
        farewellShown = false;

        OpenCloseDialoguePanel(true);
        GameStateManager.Instance.LockPlayer(priority: dialoguePriority);
        inputCooldown = inputCooldownDuration;

        ShowTextAnim(mensaje);
    }

    public void ShowThoughts(PlayerThoughts thoughtAsset)
    {
        if (thoughtAsset == null || thoughtAsset.thoughts == null) return;

        footSteps_Player?.StopAllFootsteps();

        thoughtQueue.Clear();
        foreach (var t in thoughtAsset.thoughts)
            if (t != null) thoughtQueue.Enqueue(t.text);

        if (thoughtQueue.Count == 0) return;

        npcNameTMP.text = "";
        currentKind = DialogueKind.Thought;
        farewellShown = false;

        OpenCloseDialoguePanel(true);
        GameStateManager.Instance.LockPlayer(priority: dialoguePriority);
        inputCooldown = inputCooldownDuration;

        ShowNext();
    }

    #endregion

    #region End / Close

    private void EndDialogue()
    {
        StopAllCoroutines();
        typingCoroutine = null;
        isTextAnimating = false;

        OpenCloseDialoguePanel(false);

        currentKind = DialogueKind.None;
        NPC_Interaction = null;
        CurrentDoor = null;
        farewellShown = false;

        dialogueQueue.Clear();
        doorQueue.Clear();
        thoughtQueue.Clear();

        GameStateManager.Instance?.UnlockPlayer(priority: dialoguePriority);
    }

    private void CloseDialogueCompletely() => EndDialogue();

    #endregion

    #region Utils

    private void OpenCloseDialoguePanel(bool state)
    {
        if (dialoguePanel != null) dialoguePanel.SetActive(state);
    }

    #endregion
}


