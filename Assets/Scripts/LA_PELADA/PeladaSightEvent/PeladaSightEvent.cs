using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

public class PeladaSightEvent : MonoBehaviour
{
    [Header("Cinemachine")]
    public CinemachineVirtualCamera peladaCam;
    public float duration = 3f;

    [Header("Barras negras")]
    public RectTransform topBar;
    public RectTransform bottomBar;
    public float barHeight = 100f;
    public float barSpeed = 300f;

    [SerializeField] private PlayerThoughts firstSight_PELADA;

    public GameObject pelada;

    private Vector2 topTarget;
    private Vector2 bottomTarget;
    public bool triggered = false;
    public bool restoring = false;

    private PlayerController playerController;

    private void Start()
    {
        pelada.SetActive(false);

        topBar.sizeDelta = new Vector2(topBar.sizeDelta.x, 0);
        bottomBar.sizeDelta = new Vector2(bottomBar.sizeDelta.x, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggered || !other.CompareTag("Player")) return;

        AudioManager02.Instance.PlayOneShot("event:/PeladaSightEvent");

        pelada.SetActive(true);
        triggered = true;
        peladaCam.Priority = 20;

        // Mostrar barras
        topTarget = new Vector2(topBar.sizeDelta.x, barHeight);
        bottomTarget = new Vector2(bottomBar.sizeDelta.x, barHeight);

        GameStateManager.Instance.LockPlayer(10);

        Invoke(nameof(EndEvent), duration);
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            triggered = false;
        restoring = false;
    }

    private void Update()
    {
        if (!triggered) return;

        if (!restoring)
        {
            // Animar aparición
            topBar.sizeDelta = Vector2.MoveTowards(topBar.sizeDelta, topTarget, Time.deltaTime * barSpeed);
            bottomBar.sizeDelta = Vector2.MoveTowards(bottomBar.sizeDelta, bottomTarget, Time.deltaTime * barSpeed);
        }
        else
        {
            // Restaurar barras
            topBar.sizeDelta = Vector2.MoveTowards(topBar.sizeDelta, new Vector2(topTarget.x, 0), Time.deltaTime * barSpeed);
            bottomBar.sizeDelta = Vector2.MoveTowards(bottomBar.sizeDelta, new Vector2(bottomTarget.x, 0), Time.deltaTime * barSpeed);

            // Si ambas barras ya están cerradas por completo, apagamos a la pelada
            if (topBar.sizeDelta.y <= 0.1f && bottomBar.sizeDelta.y <= 0.1f)
            {
                if (pelada.activeSelf)
                {
                    pelada.SetActive(false);
                }
            }
        }
    }

    private void EndEvent()
    {
        peladaCam.Priority = 5;
        restoring = true;

        GameStateManager.Instance.UnlockPlayer(10);

        DialogueManager.Instance.ShowThoughts(firstSight_PELADA);
    }

}

