using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFootsteps : MonoBehaviour
{
    [Header("Components")]
    private Rigidbody playerRigidbody;
    [SerializeField] private AudioSource footSteps;

    [Header("Sound")]
    [SerializeField] private AudioClip[] footStepClips;

    [Header("Settings")]
    [SerializeField] private float movementThreshold = 0.1f;
    [SerializeField] private float walkStepInterval = 0.5f;
    [SerializeField] private float runStepInterval = 0.35f;
    [SerializeField] private float footstepVolume = 0.5f;

    private float stepTimer;
    private float currentStepInterval;

    private bool isRunning = false;

    private void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody>();
    }

    void Start()
    {
        currentStepInterval = walkStepInterval;
    }

    void Update()
    {
        HandleFootsteps();
    }

    private void HandleFootsteps()
    {
        Vector3 horizontalVelocity = new Vector3(playerRigidbody.velocity.x, 0f, playerRigidbody.velocity.z);

        if (horizontalVelocity.magnitude > movementThreshold)
        {
            Debug.Log("_El jugador se esta Moviendo");
            stepTimer -= Time.deltaTime;
            if (stepTimer <= 0f)
            {
                float currentVolume = isRunning ? 0.8f : 0.45f; // esta linea dice: si esta corriendo volumen = 0.8 sino volumen = 0.45
                PlayFootstepSound();
                stepTimer = currentStepInterval;
            }
        }
        else
        {
            stepTimer = 0f;
        }
    }

    private void PlayFootstepSound()
    {
        if (footStepClips.Length > 0)
        {
            int randomIndex = Random.Range(0, footStepClips.Length);
            footSteps.PlayOneShot(footStepClips[randomIndex], footstepVolume);
            Debug.Log("_SONIDO PASOS");
        }
    }

    public void SetRunning(bool running)
    {
        isRunning = running;
        currentStepInterval = running ? runStepInterval : walkStepInterval;
    }
}

