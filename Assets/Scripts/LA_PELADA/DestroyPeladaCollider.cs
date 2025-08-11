using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyPeladaCollider : MonoBehaviour
{
    [SerializeField] private GameObject pelada;

    [SerializeField] float disappearsTime;
    private bool paso = false;
    private bool activeAccount = true;

    void Update()
    {
        if (!activeAccount) return;

        disappearsTime -= Time.deltaTime;
        if (disappearsTime <= 0)
        {
            Destroy(pelada);
            Destroy(gameObject);
            activeAccount = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!paso && other.CompareTag("Player"))
        {
            paso = true;
            Destroy(pelada);
            Destroy(gameObject);
            AudioManager02.Instance.PlayOneShot("event:/destroyPelada");
        }
    }

    private void OnDrawGizmos()
    {
        BoxCollider box = GetComponent<BoxCollider>();
        if (box == null) return;

        Gizmos.color = Color.green;

        // Aplicar la matriz del transform actual
        Gizmos.matrix = transform.localToWorldMatrix;

        // Dibujar el cubo en el espacio local del objeto
        Gizmos.DrawWireCube(box.center, box.size);
    }
}
