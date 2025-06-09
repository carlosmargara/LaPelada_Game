using UnityEngine;
using UnityEngine.AI;

public class PeladaAI : MonoBehaviour
{
    [SerializeField] private Transform[] patrolPoints; //puntos de patrulla
    [SerializeField] private float detectionRadius = 10f;
    [SerializeField] private float attackDistance = 5f;
    [SerializeField] private float waitTimeAtPoint = 2f;

    private int currentPointIndex = 0;
    private float waitTimer = 0f;
    private NavMeshAgent agent;
    private Transform player;
    private bool chasing;

    public float distanceToPlayer; //esto esta aca para poder verlo en el inspector

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        GoToNextPoint();
    }

    void Update()
    {
        //float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (!chasing && distanceToPlayer <= detectionRadius)
        {
            chasing = true;
            Debug.Log("La Pelada te vio...");
        }

        if (chasing)
        {
            agent.SetDestination(player.position);

            if (distanceToPlayer <= attackDistance)
            {
                // Acá podés lanzar animación de muerte o pantalla de game over
                Debug.Log("La Pelada te agarró");
                // Ej: SceneManager.LoadScene("GameOver");
            }

        }
        else
        {
            if (!agent.pathPending && agent.remainingDistance < 0.5f)
            {
                waitTimer += Time.deltaTime;
                if (waitTimer >= waitTimeAtPoint)
                {
                    GoToNextPoint();
                    waitTimer = 0f;
                }
            }
        }
    }

    void GoToNextPoint()
    {
        if (patrolPoints.Length == 0) return;

        agent.destination = patrolPoints[currentPointIndex].position;
        currentPointIndex = (currentPointIndex + 1) % patrolPoints.Length;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
