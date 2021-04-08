using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class SpawnManager : MonoBehaviour
{
    #region Singleton

    public static SpawnManager instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of inventory found!");
            return;
        }
        instance = this;
    }

    #endregion

    public SpawnObject[] spawnObjects;

    public float radius = 1;
    public Vector2 regionSize = Vector2.one;
    public int rejectionSamples = 30;
    public float displayRadius = 0.5f;
    public bool showGizmos;

    List<Vector2> points;

    private void OnValidate()
    {
        points = PoissonDiscSampling.GeneratePoints(radius, regionSize, rejectionSamples);
    }


    public void Spawn()
    {
        for (int i = 0; i < spawnObjects.Length; i++)
        {
            Spawner(spawnObjects[i].gameObject, spawnObjects[i].objectCount);
        }
    }

    private void Spawner(GameObject obj,int count)
    {
        for(int i = 0; i < count; i++)
        {
            if(points.Count == 0)
            {
                Debug.LogError("No spawn point left!");
                return;
            }
            Vector2 point = points[Random.Range(0, points.Count)];
            float angle = Random.value * Mathf.PI * 2;
            Quaternion rotation = Quaternion.Euler(new Vector3(0, Mathf.Rad2Deg * angle, 0));
            GameObject newObject = Instantiate(obj, new Vector3(point.x, 0, point.y), Quaternion.identity);
            newObject.transform.rotation = rotation;
            points.Remove(point);
        }
    }


    private void OnDrawGizmos()
    {
        if (showGizmos)
        {
            if (points != null)
            {
                foreach (Vector2 point in points)
                {
                    Vector3 position = new Vector3(point.x, 0, point.y);
                    Gizmos.DrawSphere(position, displayRadius);
                }
            }
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position, new Vector3(regionSize.x, 1, regionSize.y));
        }
        
    }

}
