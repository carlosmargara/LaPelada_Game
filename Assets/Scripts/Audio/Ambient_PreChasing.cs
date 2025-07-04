using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ambient_PreChasing : MonoBehaviour
{
    public FMOD.Studio.EventInstance ambiente;
    float timer;
    float checkInterval = 6f; // cada 6 segundos decidimos si cambia algo

    void Start()
    {
        ambiente = FMODUnity.RuntimeManager.CreateInstance("event:/Chase/PreChasing");
        //ambiente.start();
        ambiente.setParameterByName("Wooo Izq", 0);
        ambiente.setParameterByName("Wooo Der", 0);
        ambiente.setParameterByName("Voces", 0);
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= checkInterval)
        {
            timer = 0f;
            ActivarCapasAleatorias();
        }
    }

    void ActivarCapasAleatorias()
    {
        ambiente.setParameterByName("Wooo Izq", Random.value < 0.4f ? 1 : 0);
        ambiente.setParameterByName("Wooo Der", Random.value < 0.6f ? 1 : 0);
        ambiente.setParameterByName("Voces", Random.value < 0.3f ? 1 : 0);
    }

    void OnDestroy()
    {
        ambiente.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        ambiente.release();
    }
}
