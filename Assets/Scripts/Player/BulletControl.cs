using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletControl : MonoBehaviour
{
    public Transform targetPosition;
    public float bulletSpeed = 1f;

    private void Update()
    {
        if (targetPosition)
        {
            Vector3 direction = Vector3.Normalize((targetPosition.position - transform.position));
            transform.rotation = Quaternion.LookRotation(direction);
            transform.position += transform.forward * bulletSpeed * Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer.Equals(LayerMask.NameToLayer("Enemy")))
        {
            Destroy(other.gameObject);
        }
            Destroy(this.gameObject, 1);
            GetComponent<MeshRenderer>().enabled = false;
    }


}
