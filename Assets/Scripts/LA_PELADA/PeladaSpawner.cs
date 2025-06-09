using UnityEngine;
using UnityEngine.UI;

public class PeladaSpawner : MonoBehaviour
{
    [SerializeField] private Transform player;
    [Space]
    [Header("Info Bicho")]
    [SerializeField] private GameObject peladaPrefab;
    [SerializeField] private float minSpawnTime = 20f;
    [SerializeField] private float maxSpawnTime = 60f;
    private float spawnDistanceBehindPlayer;

    public float spawnTimer;
    private bool peladaActive;

    [Header("Feedback visual Chasing")]
    [SerializeField] private GameObject feedbackVisualChasing;

    [Header("Distancia de aparición")]
    [SerializeField] private float minSpawnDistance = 20f;
    [SerializeField] private float maxSpawnDistance = 50f;

    void Start()
    {
        ResetSpawnTimer();
    }

    void Update()
    {
        if (peladaActive) return;
        
        spawnTimer -= Time.deltaTime * 2;

        if (spawnTimer <= 0)
        {
            SpawnPelada();
            ResetSpawnTimer();
        }
    }

    void ResetSpawnTimer()
    {
        spawnTimer = Random.Range(minSpawnTime, maxSpawnTime);
    }

    void SpawnPelada()
    {
        spawnDistanceBehindPlayer = Random.Range(minSpawnDistance,maxSpawnDistance);//Randomiza la distancia en la que aparece el bicho
        
        Vector3 spawnPosition = player.position - player.forward * spawnDistanceBehindPlayer;// Calcula posición detrás del jugador
        spawnPosition.y = player.position.y; // Asegura que esté al mismo nivel

        // Instancia la Pelada
        GameObject pelada = Instantiate(peladaPrefab, spawnPosition, Quaternion.identity);

        // Le pasa una referencia al jugador
        Chasing perseguidor = pelada.GetComponent<Chasing>();
        if (perseguidor != null)
        {
            perseguidor.SetTarget(player);
            perseguidor.SetSpawner(this);
        }
        
        peladaActive = true;
        feedbackVisualChasing.SetActive(true);
        Debug.Log("PELADA MISSING... Activar feedback visual");

    }

    public void NotifyPeladaMissing() //Notificacion de pelada Desaparecida
    {
        Debug.Log("PELADA MISSING... desactivar feedback visual");
        feedbackVisualChasing.SetActive(false);
        peladaActive = false;
        ResetSpawnTimer(); 
    }
}

