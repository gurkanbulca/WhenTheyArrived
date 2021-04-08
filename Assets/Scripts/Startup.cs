using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Startup : MonoBehaviour
{
    public GameObject playerSpawner;
    public bool enableSpawnProcess;
    SpawnManager spawnManager;
    void Start()
    {
        spawnManager = SpawnManager.instance;

        if(enableSpawnProcess)
        spawnManager.Spawn();
        NavMeshBaker.instance.PushToSurfaces(NavMeshBaker.instance.ground);
        if(enableSpawnProcess)
        GameObject.FindGameObjectWithTag("PlayerCameraGroup").transform.position = playerSpawner.transform.position;
    }

    
}
