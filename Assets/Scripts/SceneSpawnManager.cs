using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneSpawnManager : MonoBehaviour
{
    void Start()
    {
        string spawnPointName = SceneLoader.nextSpawnPoint;
        if (string.IsNullOrEmpty(spawnPointName)) return;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        GameObject spawnPoint = GameObject.Find(spawnPointName);

        if (player != null && spawnPoint != null)
            player.transform.position = spawnPoint.transform.position;
        else
            Debug.LogWarning("SpawnManager: falta Player o SpawnPoint.");
    }
}
