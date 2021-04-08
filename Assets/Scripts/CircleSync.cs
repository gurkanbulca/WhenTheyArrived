using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleSync : MonoBehaviour
{
    public static int PosID = Shader.PropertyToID("_PlayerPosition");
    public static int SizeID = Shader.PropertyToID("_Size");

    Material wallMaterial;
    public float fadingSpeed;
    GameObject player;

    float timer;

    private void Start()
    {
        wallMaterial = GetComponent<Renderer>().material;
        player = GameObject.FindGameObjectWithTag("Player");
        
    }

    private void Update()
    {

        if(transform.position.z< player.transform.position.z)
        {
            if (wallMaterial.GetFloat(SizeID) == 0)
            {
                timer = 0;
            }
            timer += Time.deltaTime * fadingSpeed;

            wallMaterial.SetFloat(SizeID, Mathf.Sqrt(Mathf.Lerp(0, 1, timer)));

            
        }
        else
        {
            if (wallMaterial.GetFloat(SizeID) == 1)
            {
                timer = 0;
            }
            timer += Time.deltaTime * fadingSpeed;

            wallMaterial.SetFloat(SizeID, Mathf.Sqrt(Mathf.Lerp(1, 0, timer)));

        }

        var view = Camera.main.WorldToViewportPoint(player.transform.position);
        wallMaterial.SetVector(PosID, view);


    }

}
