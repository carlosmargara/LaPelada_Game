using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public enum PeladaState
{
    Patrol,
    Stalk,
    KillChase
}

public class AI_VersionWithState : MonoBehaviour
{
    public PeladaState currentState = PeladaState.Patrol;

    public Transform[] patrolPoints;
    public float detectionRadius = 10f;
    public float stalkDuration = 3f;
    public float attackDistance = 1.5f;
    public float speedPatrol = 2f;
    public float speedChase = 6f;

    private NavMeshAgent agent;
    private Transform player;
    private int currentPatrolIndex = 0;
    public float stalkTimer = 0f; //esto es solo publico para poder verlo en el inspector

    [Space]
    [SerializeField] private GameObject feedbackChasing;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        GoToNextPatrolPoint();
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        switch (currentState)
        {
            case PeladaState.Patrol:
                agent.speed = speedPatrol;
                PatrolBehavior(distanceToPlayer);
                break;

            case PeladaState.Stalk:
                StalkBehavior(distanceToPlayer);
                break;

            case PeladaState.KillChase:
                agent.speed = speedChase;
                agent.acceleration = 500f;
                ChaseBehavior(distanceToPlayer);
                break;
        }
    }

    void PatrolBehavior(float distanceToPlayer)
    {
        if (distanceToPlayer <= detectionRadius)
        {
            AudioManager02.Instance.Read_Dario_LaVoz.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            AudioManager02.Instance.PlaySound_MeetingWithPELADA(); //sistema con Fmod.
            currentState = PeladaState.Stalk;
            stalkTimer = 0f;
            agent.ResetPath();
            Debug.Log("La Pelada te est� mirando...");
            return;
        }

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            GoToNextPatrolPoint();
        }
    }

    void StalkBehavior(float distanceToPlayer)
    {
        //AudioManager.Instance.PlayMusic(AudioManager.Instance.chasing, false); //pone en play la musica de chasing


        stalkTimer += Time.deltaTime;

        // Mirar al jugador lentamente
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0;
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 1.5f);
        }

        if (stalkTimer >= stalkDuration)
        {
            AudioManager02.Instance.StopAmbience();
            AudioManager02.Instance.PlayOneShot("event:/Chase/woman's scream");
            currentState = PeladaState.KillChase;
            Debug.Log("�La Pelada sali� corriendo!");
            feedbackChasing.SetActive(true);
        }
    }

    void ChaseBehavior(float distanceToPlayer)
    {
        // Puede usar NavMesh o movimiento directo
        agent.SetDestination(player.position);

        if (distanceToPlayer <= attackDistance)
        {
            Debug.Log("�La Pelada te hizo boleta! _GAME OVER");
            // Ac� pod�s lanzar animaci�n de muerte o cargar escena
            SceneManager.LoadScene(2);
        }
    }

    void GoToNextPatrolPoint()
    {
        if (patrolPoints.Length == 0) return;

        // Si ya estamos en el �ltimo punto, destruimos el objeto
        if (currentPatrolIndex >= patrolPoints.Length)
        {
            Debug.Log("La Pelada termin� de patrullar. Se destruye.");
            Destroy(gameObject);
            return;
        }

        agent.destination = patrolPoints[currentPatrolIndex].position;
        currentPatrolIndex++;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}

