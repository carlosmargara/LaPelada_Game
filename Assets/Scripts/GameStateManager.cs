using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStateManager : Singleton<GameStateManager>
{
    [Header("Player Prefab")]
    [SerializeField] private PlayerController playerPrefab; // prefab completo del Player
    private PlayerController playerController; // referencia al Player instanciado
    private int _currentPriority = 0;


    [Header("Pelada Spawn")]
    private string lastSceneName = "";
    // Nuevo flag global: si la Pelada ya fue activada en algún momento
    [SerializeField] public bool peladaTriggered { get; set; } = false;
    // Este es solo para el caso Bar → Plaza
    private bool peladaSpawnedOnce = false;

    [Header("Puertas")]
    private Dictionary<string, bool> doorStates = new Dictionary<string, bool>();

    [Header("Avistamientos Pelada")]
    public bool peladaSightEventTriggered = false;
    public bool camTriggerGarageTriggered = false;
    public bool camTriggerZoneTriggered = false;

    private Note_Interaction note_Interaction;

    // Guarda todos los IDs de ítems que ya fueron recogidos
    private HashSet<string> pickedItems = new HashSet<string>();
    /* 
    basicamente la property( read-only computed property -porque tiene el get- ) que tengo abajo es como poner esta linea 
    - public Transform PlayerTransform => playerController != null ? playerController.transform : null; - 
    significa: 
    “Si playerController es distinto de null, devolveme su transform.
    Si es null, devolveme null.”
    */
    public Transform PlayerTransform
    {
        get
        {
            if (playerController != null)
                return playerController.transform;
            else
                return null;
        }
    }

    protected override void Awake()
    {
        base.Awake();
        note_Interaction = FindObjectOfType<Note_Interaction>();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Evitar el manejo normal si estamos en la pantalla de GameOver
        if (scene.name == "GameOver")
        {
            Debug.Log("[GameStateManager] Escena GameOver detectada: no se instancia ni reposiciona el Player.");
            // Si existe un player persistente de antes, lo destruimos
            if (playerController != null)
            {
                Destroy(playerController.gameObject);
                playerController = null;
            }
            return;
        }

        if (scene.name == "ScreenLoading")
        {
            Debug.Log($"[GameStateManager] Ignorando escena intermedia: {scene.name}. lastSceneName sigue siendo '{lastSceneName}'");
            return;
        }

        if (scene.name == "LaPeladaTeAcosaFuerte" && playerController == null)
        {
            Debug.Log("[GameStateManager] Escena inicial detectada, instanciando Player directamente.");
            Transform spawnPointPlaza = GameObject.Find("PlayerSpawnPoint")?.transform;
            HandlePlayerSpawn(spawnPointPlaza);
            AssignPlayerToTriggers();
            lastSceneName = scene.name;
            return;
        }

        Transform spawnPoint = GameObject.Find("PlayerSpawnPoint")?.transform;

        HandlePlayerSpawn(spawnPoint);
        AssignPlayerToTriggers();
        HandlePeladaSpawn(scene);

        Debug.Log($"[GameStateManager] lastSceneName actualizado: {lastSceneName} → {scene.name}");
        lastSceneName = scene.name;
    }

    private void HandlePeladaSpawn(Scene scene)
    {
        if (!peladaSpawnedOnce && lastSceneName == "Bar" && scene.name == "LaPeladaTeAcosaFuerte")
        {
            Debug.Log("[GameStateManager] El jugador volvió del Bar a la Plaza por primera vez → Spawn de La Pelada!");

            PeladaSpawner spawner = FindObjectOfType<PeladaSpawner>(true);

            if (spawner != null)
            {
                spawner.gameObject.SetActive(true);
                peladaSpawnedOnce = true;
                peladaTriggered = true;
                Debug.Log("[GameStateManager] Spawn de la Pelada activado (solo una vez).");
            }
            else
            {
                Debug.LogWarning("[GameStateManager] No se encontró ningún objeto con el script 'PeladaSpawner' en la escena.");
            }
        }
    }

    private void AssignPlayerToTriggers()
    {
        foreach (CamTriggerGarage trigger in FindObjectsOfType<CamTriggerGarage>())
        {
            trigger.SetPlayer(playerController);
        }
    }

    private void HandlePlayerSpawn(Transform spawnPoint)
    {
        if (playerController == null)
        {
            GameObject playerObj = Instantiate(playerPrefab.gameObject);

            if (spawnPoint != null)
            {
                playerObj.transform.position = spawnPoint.position;
                playerObj.transform.rotation = spawnPoint.rotation;
            }

            playerController = playerObj.GetComponent<PlayerController>();
            DontDestroyOnLoad(playerObj);
        }
        else if (spawnPoint != null)
        {
            playerController.transform.position = spawnPoint.position;
            playerController.transform.rotation = spawnPoint.rotation;
        }

        Debug.Log($"[GameStateManager] PlayerController asignado: {playerController.name}");
    }

    public void SaveDoorState(string doorID, bool isAtA)
    {
        if (string.IsNullOrEmpty(doorID)) return;

        doorStates[doorID] = isAtA;
        Debug.Log($"[GameState] Guardado estado de {doorID}: isAtA={isAtA}");
    }

    public bool GetDoorState(string doorID, bool defaultValue = true)
    {
        if (string.IsNullOrEmpty(doorID)) return defaultValue;

        if (doorStates.TryGetValue(doorID, out bool value))
            return value;

        return defaultValue;
    }

    #region Lock / Unlock Player
    public void LockPlayer(int priority = 0)
    {
        if (playerController == null) return;

        if (priority >= _currentPriority)
        {
            _currentPriority = priority;
            playerController.enabled = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            Debug.Log($"LockPlayer (Priority: {priority}) - PlayerController.enabled: {playerController.enabled}");
        }
    }

    public void UnlockPlayer(int priority = 0)
    {
        if (playerController == null) return;

        if (priority >= _currentPriority)
        {
            _currentPriority = 0;
            playerController.enabled = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            Debug.Log($"UnlockPlayer - PlayerController.enabled: {playerController.enabled}");
        }
    }

    public bool IsPlayerLocked()
    {
        if (playerController == null) return false;
        return !playerController.enabled;
    }
    #endregion

    public void ResetState()
    {
        Debug.Log("[GameStateManager] Reiniciando estado global...");

        peladaTriggered = false;
        peladaSightEventTriggered = false;
        camTriggerGarageTriggered = false;
        camTriggerZoneTriggered = false;
        peladaSpawnedOnce = false;

        doorStates.Clear();
        _currentPriority = 0;

        playerController = null; // por si se destruyó en GameOver
        lastSceneName = "";

        // 🩹 Resetear flag de persistencia del Player
        typeof(Player_PersistentObject)
            .GetField("_exists", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)
            ?.SetValue(null, false);

        Debug.Log("[GameStateManager] Estado reseteado con éxito (Player_PersistentObject flag reseteado).");
    }

    #region Metodos que uso para saber si el Item ya fue recolectado
    //fuciona con el  HashSet<string> pickedItems que esta arriba
    public bool IsItemPicked(string itemID)
    {
        return pickedItems.Contains(itemID);
    }

    public void MarkItemPicked(string itemID)
    {
        if (!pickedItems.Contains(itemID))
            pickedItems.Add(itemID);
    }
    #endregion
}

