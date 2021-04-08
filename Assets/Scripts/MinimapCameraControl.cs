using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapCameraControl : MonoBehaviour
{
    Camera cam;
    public float orthographicSize;
    public Transform target;
    private void Start()
    {
        cam = GetComponent<Camera>();   
    }

    private void FixedUpdate()
    {
        cam.orthographicSize = orthographicSize;

    }

    private void LateUpdate()
    {
        cam.transform.position = new Vector3(target.position.x, cam.transform.position.y, target.position.z);
    }



}
