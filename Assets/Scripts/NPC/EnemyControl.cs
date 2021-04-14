using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEditor;

public class EnemyControl : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator animator;
    private Vector3 startPosition;
    private Collider[] playerColliderArray;
    private float timer, awaitTimer;
    private FieldOfView fow;
    private Vector3 lastPosition;
    private float currentHealth;

    public Vector2 damageRange;
    public float maxHealth;
    public float chaseRange;
    public float attackRange;
    public float attackSpeed;
    public float rotationSpeed;
    public LayerMask playerLayer;
    public int experience;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        fow = GetComponent<FieldOfView>();
        startPosition = transform.position;
        lastPosition = transform.position;
        currentHealth = maxHealth;
    }

    private void Update()
    {
        if(agent.velocity.magnitude > 0.1f)
        {
            animator.SetBool("isRunning", true);
            animator.SetInteger("Attack", 0);

        }
        else 
        {
            animator.SetBool("isRunning", false);

        }
        timer += Time.deltaTime;
        foreach (Transform target in fow.visibleTargets)
        {
            awaitTimer = timer;
            float distance = Vector3.Distance(transform.position , target.position);
            if(distance > attackRange)
            {
                if (animator.GetInteger("Attack") == 0)
                {
                    agent.SetDestination(target.position);
                }
            }
            else
            {
                agent.ResetPath();
                Vector3 direction = (target.position - transform.position).normalized;
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                float angle = Quaternion.Angle(lookRotation, transform.rotation);
                if(angle > 15)
                {
                    RotateTowards(target);
                }
                else if (timer > attackSpeed &&(!agent.hasPath && agent.velocity.sqrMagnitude == 0f))
                {
                    animator.SetInteger("Attack", Random.Range(1, 3));
                    timer = 0;
                }
            }

        }
        if(fow.visibleTargets.Count == 0 && timer - awaitTimer > 5)
        {
            animator.SetInteger("Attack", 0);
            agent.SetDestination(startPosition);
        }
        lastPosition = transform.position;
        
    }

    private void RotateTowards(Transform target)
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }

    public void Attack()
    {
        animator.SetInteger("Attack", 0);
        StatManagement.instance.ReceiveDamage(Random.Range(damageRange.x, damageRange.y));
        EquipmentManager equipmentManager = EquipmentManager.instance;
        equipmentManager.ArmorDurabilityDamage();
        if (fow.visibleTargets[0].GetComponent<PlayerMovement>().isCrouching)
        {
            fow.visibleTargets[0].GetComponent<PlayerMovement>().ToggleCrouching();
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if(currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
    }

    void Die()
    {
        StatManagement.instance.GainExperience(experience);
        Destroy(this.gameObject);
    }





}


