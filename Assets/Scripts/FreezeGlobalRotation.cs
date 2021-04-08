using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeGlobalRotation : MonoBehaviour
{
    public bool x, y, z;
    public Vector3 rotation;
    public bool workOnUpdate;

    private void Start()
    {
        transform.rotation = Quaternion.Euler(x ? rotation.x : transform.rotation.eulerAngles.x, y ? rotation.y : transform.rotation.eulerAngles.y, z ? rotation.z : transform.rotation.eulerAngles.z);
    }

    private void Update()
    {
        if (workOnUpdate)
        {
            transform.rotation = Quaternion.Euler(x ? rotation.x : transform.rotation.eulerAngles.x, y ? rotation.y : transform.rotation.eulerAngles.y, z ? rotation.z : transform.rotation.eulerAngles.z);
        }
    }

}
