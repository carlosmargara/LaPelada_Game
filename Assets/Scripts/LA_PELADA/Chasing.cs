
using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class Chasing : MonoBehaviour
{
    public static Action eventPlayChasinSound;
    
    [SerializeField] private float speed = 25f;
    [SerializeField] private float rotationSpeed = 5f;
    private float lifeTime; //tiempo de vida, cuanto tiempo de va a perseguir

    private Transform target;
    private PeladaSpawner spawner;
    private float lifeTimer;

    [Header("Tiempo de vida aleatorio")]
    [SerializeField] private float minLifeTime = 2f;
    [SerializeField] private float maxLifeTime = 10f;

    private void Start()
    {
        eventPlayChasinSound?.Invoke();
    }

    public void SetTarget(Transform playerTransform)
    {
        target = playerTransform;
        lifeTime = UnityEngine.Random.Range(minLifeTime, maxLifeTime); //Randomiza el tiempo de vide del bicho, es decir cuanto tiempo te va a perseguir 
        lifeTimer = lifeTime;
    }

    public void SetSpawner(PeladaSpawner spawnerRef)
    {
        spawner = spawnerRef;
    }

    void Update()
    {
        if (target == null) return;

        lifeTimer -= Time.deltaTime;
        if (lifeTimer <= 0f)
        {
            spawner?.NotifyPeladaMissing();
            Destroy(gameObject);
            return;
        }
        
        // Mirar al jugador
        Vector3 dir = (target.position - transform.position).normalized;
        dir.y = 0f; // No inclinar verticalmente
        
        if (dir != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        }
        
        // Moverse hacia el jugador
        transform.position += transform.forward * speed * Time.deltaTime;
            
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
            Debug.Log("La pelada de Atrapo. GAME OVER");

            SceneManager.LoadScene(2);
            AudioManager.Instance.ambSound.Stop();
            
        }
    }
}

