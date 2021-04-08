using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{

    private Vector3 distance;
    private GameObject player;
    public float cameraSpeed;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        distance =  transform.position - player.transform.position;
        transform.LookAt(player.transform);
    }

    private void LateUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, player.transform.position + distance, cameraSpeed * Time.deltaTime);
    }


}
