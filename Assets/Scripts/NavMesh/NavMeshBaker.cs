using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshBaker : MonoBehaviour
{
    #region Singleton
    public static NavMeshBaker instance;

    private void Awake()
    {
        if(instance != null)
        {
            Debug.LogError("Instance already casted!");
            return;
        }
        instance = this;
    }
    #endregion

    public NavMeshSurface ground;
    public GameObject player;

    [SerializeField]
    List<NavMeshSurface> navMeshSurfaces;

    

    public void PushToSurfaces(NavMeshSurface surface)
    {
        navMeshSurfaces.Add(surface);
        BakeArea();
    }

    public void RemoveFromSurfaces(NavMeshSurface surface)
    {
        navMeshSurfaces.Remove(surface);
        StartCoroutine(DelayedBakeArea(0.1f));
    }

    public void BakeArea()
    {

        foreach (NavMeshSurface surface in navMeshSurfaces)
        {
            surface.BuildNavMesh();
        }


    }

    IEnumerator DelayedBakeArea(float second)
    {
        yield return new WaitForSeconds(second);
        BakeArea();
    }



}
