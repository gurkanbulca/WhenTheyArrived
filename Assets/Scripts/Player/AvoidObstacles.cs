using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvoidObstacles : MonoBehaviour
{
    [SerializeField]
    int numberOfRays;
    [SerializeField]
    float rayRange;
    [SerializeField]
    float angle;
    [SerializeField]
    int avoidStrength;
    [SerializeField]
    LayerMask avoidLayer;
    [SerializeField]
    bool showGizmos;


    public Vector3 Avoid(Vector3 targetDirection, Transform target)
    {
        for (int j = 0; j < avoidStrength; j++)
        {
            for (int i = 0; i < numberOfRays; i++)
            {
                var rotation = this.transform.rotation;
                var rotationModifier = Quaternion.AngleAxis(((i / ((float)numberOfRays - 1)) * angle * 2) - angle, this.transform.up);
                var direction = rotation * rotationModifier * Vector3.forward;
                var ray = new Ray(this.transform.position + Vector3.up * 0.1f, direction);
                RaycastHit hitInfo;
                if (Physics.Raycast(ray, out hitInfo, rayRange,avoidLayer))
                {
                    if(target != hitInfo.transform)
                        targetDirection -= (1.0f / numberOfRays) * direction;
                }
                else
                {
                        targetDirection += (1.0f / numberOfRays) * direction;
                }

            }
        }
        return Vector3.ClampMagnitude(targetDirection, 1);
    }

    private void OnDrawGizmos()
    {
        if (showGizmos)
        {
            for (int i = 0; i < numberOfRays; i++)
            {
                var rotation = this.transform.rotation;
                var rotationModifier = Quaternion.AngleAxis(((i / ((float)numberOfRays - 1)) * angle * 2) - angle, this.transform.up);
                var direction = rotation * rotationModifier * Vector3.forward;
                Gizmos.DrawRay(this.transform.position + Vector3.up * 0.1f, direction * rayRange);
            }
        }
        
    }
}
