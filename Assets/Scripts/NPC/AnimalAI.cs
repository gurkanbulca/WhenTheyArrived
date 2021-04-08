using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AnimalAI : MonoBehaviour
{

    FieldOfView fow;
    private float timer;
    private GameObject runningFrom;
    private NavMeshAgent agent;
    private Animator animator;
    NavMeshPath path;
    float currentHealth;

    private float idleTime;
    public float wanderSpeed;
    public float runSpeed;
    public float maxHealth = 50;
    public int experience;


    private void Start()
    {
        currentHealth = maxHealth;
        path = new NavMeshPath();
        fow = GetComponent<FieldOfView>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        idleTime = Random.Range(5, 10);
        agent.avoidancePriority = Random.Range(0, 99);
    }

    private void Update()
    {
        if (!agent.hasPath)
        {
            animator.SetBool("isRunning", false);
        }
        if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
        {
            timer += Time.deltaTime;
            if (timer > idleTime)
            {
                Vector3 randomDestination = new Vector3(transform.position.x + Random.Range(-50, 50), 0, transform.position.z + Random.Range(-50, 50));

                if (agent.CalculatePath(randomDestination, path))
                {
                    animator.SetBool("isRunning", true);
                    agent.SetPath(path);
                    agent.speed = wanderSpeed;
                    animator.speed = 1;
                    timer = 0;
                    idleTime = Random.Range(5, 10);
                }
            }
        }
        if (fow.visibleTargets.Count > 0)
        {
            path = FindPath(fow.visibleTargets[0]);
            RunAway(path);
        }

    }

    private NavMeshPath FindPath(Transform target)
    {
        NavMeshPath path = new NavMeshPath();
        Transform player = GameObject.FindGameObjectWithTag("Player").transform;
        Vector3 direction = (transform.position - target.position).normalized;
        Vector3 direction2 = (transform.position - target.position).normalized;
        int safetyCounter = 0;
        while (true)
        {
            safetyCounter++;
            if (safetyCounter > 100) break;
            Vector3 destination = transform.position + (direction * 20);
            if (agent.CalculatePath(destination, path))
            {
                return path;
            }
            destination = transform.position + (direction2 * 20);
            if (agent.CalculatePath(destination, path))
            {
                return path;
            }
            direction = (Quaternion.AngleAxis(10, Vector3.up) * direction).normalized;
            direction2 = (Quaternion.AngleAxis(-10, Vector3.up) * direction).normalized;
        }
        return path;
    }

    private void RunAway(NavMeshPath path)
    {
        agent.SetPath(path);
        agent.speed = runSpeed;
        animator.speed = 2;
        animator.SetBool("isRunning", true);
    }

    public void TakeDamage(float damage,Transform agressor)
    {
        currentHealth -= damage;
        if(currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
        else
        {
            NavMeshPath path = FindPath(agressor);
            RunAway(path);
        }
    }

    private void Die()
    {
        StatManagement.instance.GainExperience(experience);
        Destroy(this.gameObject);
    }

}
