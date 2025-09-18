using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class DiffetentTypes_footSteps_with_FmodEvent : MonoBehaviour
{
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float rayDistance = 1.8f;

    private EventInstance walkInstance;
    private EventInstance runInstance;

    private bool isFootstepPlaying = false;
    private bool wasRunningLastFrame = false;

    private void Start()
    {
        walkInstance = RuntimeManager.CreateInstance("event:/Player/Walk");
        runInstance = RuntimeManager.CreateInstance("event:/Player/Run");
    }

    public void HandleFootsteps(Vector3 moveDirection, bool isRunning)
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        if (Mathf.Abs(x) > 0.1f || Mathf.Abs(z) > 0.1f)
        {
            //Debug.Log("_Se esta moviendo");
            SurfaceType surface = DetectSurfaceType();

            if (isRunning)
            {
                runInstance.setParameterByNameWithLabel("Surface", surface.ToString());
                if (!isFootstepPlaying || !wasRunningLastFrame)
                {
                    StopAllFootsteps();
                    runInstance.start();
                    isFootstepPlaying = true;
                    wasRunningLastFrame = true;
                }
            }
            else
            {
                walkInstance.setParameterByNameWithLabel("Surface", surface.ToString());
                if (!isFootstepPlaying || wasRunningLastFrame)
                {
                    StopAllFootsteps();
                    walkInstance.start();
                    isFootstepPlaying = true;
                    wasRunningLastFrame = false;
                }
            }
        }
        else
        {
            //Debug.Log("_No se esta moviendo ");
            if (isFootstepPlaying)
            {
                StopAllFootsteps();
                isFootstepPlaying = false;
            }
        }
    }

    public void StopAllFootsteps()
    {
        walkInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        runInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    private SurfaceType DetectSurfaceType()
    {
        Ray ray = new Ray(transform.position + Vector3.up * 0.1f, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, rayDistance, groundLayer))
        {
            switch (hit.collider.tag)
            {
                case "Wood": return SurfaceType.Wood;
                case "Stone": return SurfaceType.Stone;
                case "Grass": return SurfaceType.Grass;
                case "Metal": return SurfaceType.Metal;
                case "Asphalt": return SurfaceType.Asphalt;
                case "Cobblestones": return SurfaceType.Cobblestones;
                case "Concrete": return SurfaceType.Concrete;
            }
        }
        return SurfaceType.Default;
    }

    private void OnDestroy()
    {
        walkInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        runInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);

        walkInstance.release();
        runInstance.release();
    }
}

