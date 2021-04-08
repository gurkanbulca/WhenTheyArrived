using HighlightPlus;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAttack : MonoBehaviour
{
    public LayerMask enemyLayer;
    public float enemyDetectionRange = 20f;
    public GameObject circle;
    public float attackRange = 0.5f;
    public bool hasRifle,hasSword;
    public Transform gunTip;
    public GameObject bullet;
    public bool isAttacking = false;
    public Button attackButton;

    private Animator animator;
    private GameObject closestEnemy;
    private float distance;
    private float timer = 0,rifleTimer = 0;
    private PlayerMovement movement;
    private StatManagement stats;
    AvoidObstacles avoid;
    private bool targetHighlighted;


    private void Start()
    {
        animator = GetComponent<Animator>();
        movement = GetComponent<PlayerMovement>();
        stats = StatManagement.instance;
        avoid = GetComponent<AvoidObstacles>();
    }


    void Update()
    {
        rifleTimer -= Time.deltaTime;
        Collider[] enemyColliders = Physics.OverlapSphere(transform.position, enemyDetectionRange, enemyLayer);
        distance = Mathf.Infinity;
        closestEnemy = null;
        foreach (Collider col in enemyColliders)
        {
            float newDistance = Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(col.gameObject.transform.position.x, col.gameObject.transform.position.z));
            if (newDistance < distance)
            {
                distance = newDistance;
                closestEnemy = col.gameObject;
            }
        }
        if (closestEnemy != null)
        {
            // Circle Operations
            //circle.SetActive(true);
            //circle.transform.position = closestEnemy.transform.position;

            // Highlight Target
            if (!closestEnemy.GetComponent<HighlightEffect>().highlighted)
            {
                targetHighlighted = true;
                HighlightTarget(closestEnemy.transform);
            }

            attackButton.interactable = true;
        }
        else
        {
            // Circle Operations
            //circle.SetActive(false);

            // Highlight Target
            if (targetHighlighted)
            {
                ClearHighlights();
                targetHighlighted = false;
            }

            attackButton.interactable = false;
            
        }

        if (Input.GetKey(KeyCode.Space))
        {
            Attack();
        }
        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0 || Joystick.instance.Horizontal != 0 || Joystick.instance.Vertical != 0)
        {
            CancelAttack();
        }

        if (isAttacking)
        {
            Targeting();
        }
    }

    public void CancelAttack()
    {
        isAttacking = false;
    }

    public void Attack()
    {
        if (!isAttacking)
        {
            if (hasRifle && rifleTimer > 0) return;
            GetComponent<PlayerCollect>().CancelCollect();
            isAttacking = true;
            timer = 0;
            rifleTimer = 0.5f;
        }
    }

    private void Targeting()
    {
        //Targeting
        if (closestEnemy != null)
        {
            if (hasSword)
            {
                if (distance > attackRange)
                {
                    Vector3 direction = closestEnemy.transform.position - transform.position;
                    direction.y = 0;
                    direction = Vector3.Normalize(direction);
                    direction = Vector3.ClampMagnitude(direction, 1);
                    direction = Vector3.Lerp(Vector3.zero, direction, timer * 2);
                    timer += Time.deltaTime ;
                    if (animator.GetCurrentAnimatorStateInfo(0).IsName("Movement") || animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
                    {
                        direction = avoid.Avoid(direction,closestEnemy.transform);
                        movement.MoveToDirection(direction);
                    }
                }
                else
                {
                    Vector3 direction = (closestEnemy.transform.position - transform.position).normalized;
                    Quaternion lookRotation = Quaternion.LookRotation(direction);
                    float angle = Quaternion.Angle(lookRotation, transform.rotation);

                    if (angle > 5)
                    {
                        movement.RotateTowards(closestEnemy.transform);
                    }
                    else
                    {
                        movement.MoveToDirection(Vector3.zero);
                        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Movement") || animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
                        {
                            animator.SetTrigger("Attack");
                        }
                    }

                }
            }else if (hasRifle)
            {
                movement.MoveToDirection(Vector3.zero);
                if (animator.GetCurrentAnimatorStateInfo(0).IsName("Movement"))
                {
                    Vector3 direction = closestEnemy.transform.position - transform.position;
                    direction.y = 0;
                    direction = Vector3.Normalize(direction);
                    transform.rotation = Quaternion.LookRotation(direction);
                    animator.SetTrigger("RifleFire");
                }
            }



        }
        else
        {
            isAttacking = false;
            movement.MoveToDirection(Vector3.zero);

        }

    }

    void HighlightTarget(Transform target)
    {
        ClearHighlights();
        target.GetComponent<HighlightEffect>().SetHighlighted(true);
    }

    void ClearHighlights()
    {
        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            enemy.GetComponent<HighlightEffect>().SetHighlighted(false);
        }
        foreach (GameObject animal in GameObject.FindGameObjectsWithTag("Animal"))
        {
            animal.GetComponent<HighlightEffect>().SetHighlighted(false);
        }
    }

    

    public void Strike()
    {
        if (closestEnemy != null)
        {
            if (closestEnemy.CompareTag("Enemy"))
            {
                closestEnemy.GetComponent<EnemyControl>().TakeDamage(stats.damageModifier);
            }
            else if (closestEnemy.CompareTag("Animal"))
            {
                closestEnemy.GetComponent<AnimalAI>().TakeDamage(stats.damageModifier, transform);
            }
            isAttacking = false;
        }
        
    }

    public void RifleFire()
    {
        GameObject newBullet = Instantiate(bullet, gunTip.position, Quaternion.identity);
        newBullet.GetComponent<BulletControl>().targetPosition = closestEnemy.transform;
        isAttacking = false;
        if (animator.GetFloat("Crouching") == 1)
            movement.ToggleCrouching();
    }

    private void OnDrawGizmos()
    {
        // Targeting Sphere
        //Gizmos.color = Color.yellow;
        //Gizmos.DrawWireSphere(transform.position,enemyDetectionRange);
    }
}
