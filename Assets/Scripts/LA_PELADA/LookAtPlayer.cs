using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class LookAtPlayer : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;

    [Space]

    [SerializeField] private float speedRot;
    [SerializeField] private float speedMov;

    [Space]

    [SerializeField] private StudioEventEmitter studioEventEmitter;

    private Vector3 dir;

    void Start()
    {
        playerTransform = FindObjectOfType<PlayerController>().transform;
        studioEventEmitter.Play();
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

    public void Approach()
    {
        transform.position = Vector3.MoveTowards(

        transform.position,           // posición actual
        playerTransform.position,     // hacia dónde
        speedMov * Time.deltaTime     // paso por frame
        );
    }
}
