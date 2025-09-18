using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStateManager : Singleton<GameStateManager>
{
    [Header("Player Prefab")]
    [SerializeField] private PlayerController playerPrefab; // prefab completo del Player

    private PlayerController playerController; // referencia al Player instanciado
    private int _currentPriority = 0;

    protected override void Awake()
    {
        base.Awake();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        /*Operador de propagación nula (null-conditional operator)
        Ese "?.transform" Significa:
        “Si lo que está a la izquierda no es null, accede a .transform.”
        “Si lo que está a la izquierda es null, devolveme null en vez de romper el programa con una excepción.”
        */

        // Buscamos el spawn point de la escena
        Transform spawnPoint = GameObject.Find("PlayerSpawnPoint")?.transform;

        // Si no tenemos PlayerController, instanciamos el prefab
        if (playerController == null)
        {
            GameObject playerObj = Instantiate(playerPrefab.gameObject);

            // Lo posicionamos en el spawn point si existe
            if (spawnPoint != null)
            {
                playerObj.transform.position = spawnPoint.position;
                playerObj.transform.rotation = spawnPoint.rotation;
            }

            playerController = playerObj.GetComponent<PlayerController>();
            DontDestroyOnLoad(playerObj); // lo hacemos persistente
        }
        else
        {
            // Si ya existe el Player (persistente), lo movemos al spawn point de la escena
            if (spawnPoint != null)
            {
                playerController.transform.position = spawnPoint.position;
                playerController.transform.rotation = spawnPoint.rotation;
            }
        }

        Debug.Log($"[GameStateManager] PlayerController asignado: {playerController.name} en escena {scene.name}");
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
}

