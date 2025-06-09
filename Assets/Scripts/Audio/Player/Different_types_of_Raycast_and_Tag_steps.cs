using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Different_types_of_Raycast_and_Tag_steps : MonoBehaviour
{
    [SerializeField] private AudioSource footstepSource;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float rayDistance = 1.8f;

    [SerializeField] private FootstepAudioData[] audioDataArray;
    [SerializeField] private SurfaceType defaultSurface = SurfaceType.Default;

    [Header("Paso caminando")]
    [SerializeField] private float walkStepInterval = 0.5f;
    [SerializeField] private float walkVolume = 0.45f;

    [Header("Paso corriendo")]
    [SerializeField] private float runStepInterval = 0.35f;
    [SerializeField] private float runVolume = 0.8f;

    private float stepTimer = 0f;


    private void Update()
    {
        Debug.DrawRay(transform.position + Vector3.up * 0.1f, Vector3.down * rayDistance, Color.blue, 0.1f); 
    }

    public void HandleFootsteps(Rigidbody playerRb, bool isRunning)
    {
        Vector3 horizontalVelocity = new Vector3(playerRb.velocity.x, 0, playerRb.velocity.z);
        float movementThreshold = 0.1f;

        float currentInterval = isRunning ? runStepInterval : walkStepInterval;
        float currentVolume = isRunning ? runVolume : walkVolume;

        if (horizontalVelocity.magnitude > movementThreshold)
        {
            stepTimer -= Time.deltaTime;
            if (stepTimer <= 0f)
            {
                SurfaceType surface = DetectSurfaceType();
                PlayFootstep(surface, currentVolume);
                stepTimer = currentInterval;
            }
        }
        else
        {
            stepTimer = 0;
        }
    }

    private SurfaceType DetectSurfaceType()
    {
        Ray ray = new Ray(transform.position + Vector3.up * 0.1f, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, rayDistance, groundLayer))
        {
            // Detectamos por tag
            switch (hit.collider.tag)
            {
                case "Wood": return SurfaceType.Wood;
                case "Stone": return SurfaceType.Stone;
                case "Grass": return SurfaceType.Grass;
                case "Metal": return SurfaceType.Metal;
                case "Asphalt": return SurfaceType.Asphalt;
                case "Cobblestones": return SurfaceType.Cobblestones;
                case "Concrete": return SurfaceType.Concrete;
                default: return defaultSurface;
            }
        }

        return defaultSurface;
    }

    private void PlayFootstep(SurfaceType surfaceType, float volume)
    {
        AudioClip[] clips = GetClipsForSurface(surfaceType);
        if (clips != null && clips.Length > 0)
        {
            int index = Random.Range(0, clips.Length);
            footstepSource.PlayOneShot(clips[index], volume);
        }
    }

    private AudioClip[] GetClipsForSurface(SurfaceType surfaceType)
    {
        foreach (var data in audioDataArray)
        {
            if (data.surfaceType == surfaceType)
                return data.footstepClips;
        }
        return null;
    }

}
