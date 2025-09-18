using UnityEngine;
using FMODUnity;

public class Trigger_PatioInteno_Oficinas_ : MonoBehaviour
{
    [Header("Lighting")]
    [SerializeField] private Light steadyLight;
    [SerializeField] private GameObject blueLight;
    [SerializeField] private BlinkingLight_con_patrón_irregular flickeringLight;

    [Space]

    [Header("Pelada")]
    [SerializeField] private GameObject pelada;

    [Space]

    [SerializeField] private LookAtPlayer lookAtPlayer;


    [SerializeField] private StudioEventEmitter studioEventEmitter;

    public float time;
    private bool inSide;

    void Start()
    {
        time = 4f;
        pelada.SetActive(false);
        blueLight.SetActive(false);
    }

    void Update()
    {
        if (pelada.activeSelf == true)
        {
            lookAtPlayer.Approach();
            time -= Time.deltaTime;
            if (time <= 0)
            {
                time = 0;
                pelada.SetActive(false);
                blueLight.SetActive(false);
                studioEventEmitter.Stop();
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!inSide)
            {
                // Apago la luz fija
                if (steadyLight != null) steadyLight.enabled = false;

                // Apago el parpadeo + sonido
                if (flickeringLight != null) flickeringLight.enabled = false;

                pelada.SetActive(true);
                blueLight.SetActive(true);
                inSide = true;

                AudioManager02.Instance.PlaySound_MeetingWithPELADA();
            }
        }
    }

    private void OnDrawGizmos()
    {
        BoxCollider box = GetComponent<BoxCollider>();
        if (box == null) return;

        Gizmos.color = Color.white;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(box.center, box.size);
    }
}

