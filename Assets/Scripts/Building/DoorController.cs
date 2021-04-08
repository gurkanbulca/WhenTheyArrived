using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    Collider player;
    Quaternion from,to,localZero;
    float timer;

    public float speed;

    private void Start()
    {
        from = transform.rotation;
        to = transform.rotation;
    }

    private void Update()
    {
        if (to != from)
        {
            if(transform.rotation != to)
            {
                transform.rotation = Quaternion.Lerp(from, to, timer * speed);
                timer += Time.deltaTime;
            }
            else
            {
                from = to;
            }
            
        }
        else
        {
            timer = 0;
        }
    }
    
    IEnumerator CloseDoorAfterSeconds(int seconds)
    {
        yield return new WaitForSeconds(seconds);
        CloseDoor();
    }

    private bool OpenDoor()
    {
        if (transform.localRotation.eulerAngles.y < 1 || transform.localRotation.eulerAngles.y > 359 )
        {
            localZero = transform.rotation;
            Vector3 distance = player.transform.position - transform.position;
            float angle = Vector3.Angle(distance, transform.forward);
            if (angle > 90)
            {
                to = transform.rotation * Quaternion.Euler(new Vector3(0, 90, 0));
            }
            else
            {
                to = transform.rotation * Quaternion.Euler(new Vector3(0, -90, 0));
            }
            return true;
        }
        return false;
    }

    private bool CloseDoor()
    {
        if (transform.localRotation.eulerAngles.y != 0)
        {
            to = localZero;
            return true;
        }
        return false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            player = other;
            if (OpenDoor())
            {
                StartCoroutine(CloseDoorAfterSeconds(5));
            }
        }
    }
}
