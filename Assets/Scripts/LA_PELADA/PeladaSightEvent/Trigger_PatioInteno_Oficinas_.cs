using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger_PatioInteno_Oficinas_ : MonoBehaviour
{
    [SerializeField] private Light _light;
    [SerializeField] private GameObject pelada;

    public float time;
    private bool paso;
    // Start is called before the first frame update
    void Start()
    {
        time = 10f;
        pelada.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (pelada.activeSelf == true)
        {
            time -= Time.deltaTime;
            if (time <= 0)
            {
                time = 0;
                pelada.SetActive(false);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!paso)
            {
                _light.enabled = false;
                pelada.SetActive(true);
                paso = true;

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
