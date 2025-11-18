using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtPlayer_Laprida : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;

    [Space]

    [SerializeField] private float speedRot;

    private Vector3 dir;

    void Start()
    {
        playerTransform = FindObjectOfType<PlayerController>().transform;
    }

    void Update()
    {
        dir = (playerTransform.position - transform.position).normalized;
        if (dir != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * speedRot);
        }
    }
}
