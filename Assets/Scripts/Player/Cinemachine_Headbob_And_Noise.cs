using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Cinemachine_Headbob_And_Noise : MonoBehaviour
{
    [Header("Camera References")]
    public CinemachineVirtualCamera virtualCam;
    private Transform camTransform;

    [Header("Headbob Settings")]
    public float walkBobSpeed = 8f;
    public float walkBobAmount = 0.05f;
    public float runBobSpeed = 14f;
    public float runBobAmount = 0.1f;

    [Header("Noise Profiles")]
    public NoiseSettings walkNoise;
    public NoiseSettings runNoise;

    private Vector3 originalCamPos;
    private CinemachineBasicMultiChannelPerlin noiseComponent;

    [Header("Player Movement State")]
    public bool isMoving = false;
    public bool isRunning = false;

    void Start()
    {
        if (virtualCam == null)
        {
            Debug.LogError("Falta asignar la cámara virtual de Cinemachine.");
            enabled = false;
            return;
        }

        camTransform = virtualCam.VirtualCameraGameObject.transform;
        originalCamPos = camTransform.localPosition;

        noiseComponent = virtualCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        if (noiseComponent == null)
        {
            Debug.LogWarning("No hay componente de ruido, se va a agregar uno.");
            noiseComponent = virtualCam.AddCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        }
    }

    void Update()
    {
        if (GameStateManager.Instance.IsPlayerLocked()) //Desactiva el headBob si el PlayerControler del jugador esta desactivado+
        {
            camTransform.localPosition = Vector3.Lerp(camTransform.localPosition, originalCamPos, Time.deltaTime * 5f);
            return;
        }

        // HEADBOB
        if (isMoving)
        {
            float speed = isRunning ? runBobSpeed : walkBobSpeed;
            float amount = isRunning ? runBobAmount : walkBobAmount;

            float bobX = Mathf.Sin(Time.time * speed) * amount;
            float bobY = Mathf.Cos(Time.time * speed * 2f) * amount;

            camTransform.localPosition = originalCamPos + new Vector3(bobX, bobY, 0);
        }
        else
        {
            camTransform.localPosition = Vector3.Lerp(camTransform.localPosition, originalCamPos, Time.deltaTime * 5f);
        }

        // CAMBIO DE NOISE PROFILE
        if (noiseComponent != null)
        {
            noiseComponent.m_NoiseProfile = isRunning ? runNoise : walkNoise;
        }
    }
}
