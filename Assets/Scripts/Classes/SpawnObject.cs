using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpawnObject
{
    public GameObject gameObject;
    public int objectCount;

    SpawnObject(GameObject gameObject, int objectCount)
    {
        this.gameObject = gameObject;
        this.objectCount = objectCount;
    }
}
