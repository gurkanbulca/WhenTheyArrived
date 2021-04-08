using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public float speed = 1f;
    public float rotationSpeed;
    public float crouchSpeedMultiplier = 0.7f;
    public float gravityMultiplier = 1f;
    public bool isCrouching = false;
    public Joystick joystick;

    private CharacterController controller;
    private float gravity = -9.81f;
    private Animator animator;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            ToggleCrouching();
        }
        // Player Movement
        if (controller.enabled)
        {
            float horizontalAxis = Input.GetAxis("Horizontal");
            float verticalAxis = Input.GetAxis("Vertical");

            // MobileJoystick
            if(joystick.Horizontal != 0 || joystick.Vertical != 0)
            {
                horizontalAxis = joystick.Horizontal;
                verticalAxis = joystick.Vertical;
            }
            

            Vector3 direction = Vector3.ClampMagnitude(new Vector3(horizontalAxis, 0, verticalAxis), 1);
            if (!GetComponent<PlayerCollect>().isCollecting && !GetComponent<PlayerCollect>().isAutoGathering && !GetComponent<PlayerAttack>().isAttacking)
            {
                MoveToDirection(direction);
            }


            // Gravity
            controller.Move(Vector3.up * gravity * gravityMultiplier * Time.deltaTime);
        }
        


    }

    public void MoveToDirection(Vector3 direction)
    {
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
        controller.Move(direction * speed * Time.deltaTime);

        float movement = Mathf.Abs(direction.x) + Mathf.Abs(direction.z);
        animator.SetFloat("Movement", Mathf.Clamp(movement,0,1));
    }
    public void RotateTowards(Transform target)
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x,0,direction.z),Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }


    public void ToggleCrouching()
    {
        if (isCrouching)
        {
            speed /= crouchSpeedMultiplier;
            animator.SetFloat("Crouching", 0);

        }
        else
        {
            speed *= crouchSpeedMultiplier;
            animator.SetFloat("Crouching", 1);
        }
        isCrouching = !isCrouching;
    }

}
