using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Startup : MonoBehaviour
{
    [SerializeField]
    Transform spawnPoints;
    [SerializeField]
    bool enableSpawnProcess;
    SpawnManager spawnManager;
    void Start()
    {
        spawnManager = SpawnManager.instance;

        if(enableSpawnProcess)
        spawnManager.Spawn();
        NavMeshBaker.instance.PushToSurfaces(NavMeshBaker.instance.ground);
        if (enableSpawnProcess)
        {
            int randomIndex = Random.Range(0, spawnPoints.childCount);
            GameObject.FindGameObjectWithTag("PlayerCameraGroup").transform.position = spawnPoints.GetChild(randomIndex).position;
        }
        
    }

    
}
