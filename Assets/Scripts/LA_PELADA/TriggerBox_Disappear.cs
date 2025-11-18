using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerBox_Disappear : MonoBehaviour
{
    [SerializeField] private float time;
    [SerializeField] private GameObject pelada;
    [SerializeField] private GameObject box;

    // Start is called before the first frame update
    void Start()
    {
        pelada.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (pelada.activeSelf)
        {
            time -= Time.deltaTime;
            if (time <= 0)
            {
                Destroy(pelada);
                Destroy(box);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            pelada.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            AudioManager02.Instance.FadeOutAndStopMusic(10);
        }
    }

    private void OnDrawGizmos()
    {
        BoxCollider box = GetComponent<BoxCollider>();
        if (box == null) return;

        Gizmos.color = Color.blue;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(box.center, box.size);
    }
}
