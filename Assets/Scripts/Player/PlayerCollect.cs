using HighlightPlus;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class PlayerCollect : MonoBehaviour
{
    public LayerMask collectableLayer;
    public float detectionRange = 20f;
    public float collectRange = 2f, gatherRange = 3f;
    public bool isCollecting = false;
    public bool isAutoGathering = false;
    public Button interactButton;

    private Animator animator;
    private GameObject closestCollect;
    private float distance, curSpeed, agentSpeed;
    private Vector3 previousPosition;
    private NavMeshAgent agent;
    private PlayerMovement movement;
    private CharacterController controller;
    private Transform collectingObject;
    private Tool tool;
    private Equipment previousEquipment;
    private bool equipmentSwapped;
    private bool targetHighlighted;

    private void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        movement = GetComponent<PlayerMovement>();
        controller = GetComponent<CharacterController>();
        agentSpeed = agent.speed;
    }

    private void Update()
    {
        if (agent.enabled)
        {
            if (movement.isCrouching)
            {
                agent.speed = agentSpeed * movement.crouchSpeedMultiplier;
            }
            else
            {
                agent.speed = agentSpeed;
            }
        }


        FindClosestCollect();

        if (closestCollect == null)
        {
            interactButton.interactable = false;
        }
        else
        {
            interactButton.interactable = true;

        }

        if (Input.GetKey(KeyCode.F) && !isCollecting)
        {
            SetDestination();
        }
        else if (Input.GetKeyDown(KeyCode.Z))
        {
            isAutoGathering = !isAutoGathering;
            if (isAutoGathering)
            {
                FindClosestCollect();
                SetDestination();
            }

        }



        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0 || Joystick.instance.Horizontal != 0 || Joystick.instance.Vertical != 0)
        {
            CancelCollect();
        }



        if (isCollecting)
        {
            if ((!agent.hasPath || agent.velocity.sqrMagnitude <= 0.01f))
            {
                float distance = Vector3.Distance(transform.position, agent.destination);
                if (distance < 3)
                {
                    float angle = CalculateAngle(closestCollect.transform.position);
                    if (angle > 10)
                    {
                        movement.RotateTowards(closestCollect.transform);
                    }
                    else
                    {
                        agent.enabled = false;
                        controller.enabled = true;
                        if (movement.isCrouching)
                        {
                            GetComponent<PlayerMovement>().ToggleCrouching();
                        }
                        if (LayerMask.LayerToName(closestCollect.layer).Equals("Collectable") && closestCollect.transform != collectingObject)
                        {
                            movement.MoveToDirection(Vector3.zero);
                            collectingObject = closestCollect.transform;

                            animator.SetTrigger("Collect");
                        }
                        else if (LayerMask.LayerToName(closestCollect.layer).Equals("Gatherable"))
                        {

                            previousEquipment = EquipmentManager.instance.CurrentEquipmentByEquipmentType(EquipmentTypes.Weapon);
                            if (previousEquipment != tool)
                            {
                                EquipmentManager.instance.VisualEquip(tool);

                                equipmentSwapped = true;
                            }
                            animator.SetTrigger("Gather");
                        }else if (LayerMask.LayerToName(closestCollect.layer).Equals("Interactable"))
                        {
                            if (closestCollect.CompareTag("ProductionStation"))
                            {
                                closestCollect.GetComponent<StationController>().Interact();
                                isCollecting = false;
                            }else if (closestCollect.CompareTag("Smelter"))
                            {
                                closestCollect.GetComponent<SmelterController>().Interact();
                                isCollecting = false;

                            }else if (closestCollect.CompareTag("Storage"))
                            {
                                closestCollect.GetComponent<StorageController>().Interact();

                                isCollecting = false;
                            }

                        }

                    }
                }
            }
            else
            {

                Vector3 curMove = transform.position - previousPosition;
                curSpeed = curMove.magnitude / Time.deltaTime / agent.speed;
                previousPosition = transform.position;
                animator.SetFloat("Movement", agent.velocity.magnitude / agent.speed);
            }
        }


    }

    public void CancelCollect()
    {
        isCollecting = false;
        isAutoGathering = false;
        agent.enabled = false;
        controller.enabled = true;
        collectingObject = null;
    }

    private void FindClosestCollect()
    {
        float sphereRadius = isAutoGathering ? Mathf.Infinity : detectionRange;
        Collider[] collectColliders = Physics.OverlapSphere(transform.position, sphereRadius, collectableLayer);
        distance = Mathf.Infinity;
        closestCollect = null;

        foreach (Collider col in collectColliders)
        {
            if (LayerMask.LayerToName(col.gameObject.layer).Equals("Interactable") && isAutoGathering) continue;
            float newDistance = Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(col.gameObject.transform.position.x, col.gameObject.transform.position.z));
            if (newDistance < distance)
            {
                if (Inventory.instance.hasEnoughSpaceForItem(col.GetComponent<ItemPickup>()?.item, 1))
                {
                    if (LayerMask.LayerToName(col.gameObject.layer).Equals("Gatherable"))
                    {
                        ToolType requiredTool = col.GetComponent<GatherableManager>().gatherTool;
                        tool = Inventory.instance.FindUsefulTool(requiredTool);
                        if (tool != null)
                        {
                            if (col.GetComponent<GatherableManager>().CanGatherable(tool))
                            {
                                distance = newDistance;
                                closestCollect = col.gameObject;
                            }

                        }
                    }
                    else
                    {
                        distance = newDistance;
                        closestCollect = col.gameObject;
                    }
                }


            }
        }
        if (closestCollect != null)
        {
            if (closestCollect.GetComponent<HighlightEffect>() != null)
            {
                if (!closestCollect.GetComponent<HighlightEffect>().highlighted)
                {
                    HighlightTarget(closestCollect.transform);
                    targetHighlighted = true;
                }
            }
            
        }
        else
        {
            if (targetHighlighted)
            {
                ClearHighlights();
                targetHighlighted = false;
            }
        }
    }

    private float CalculateAngle(Vector3 target)
    {
        target = new Vector3(target.x, 0, target.z);
        Vector3 playerDirection = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 direction = (target - playerDirection).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        float angle = Quaternion.Angle(lookRotation, Quaternion.Euler(new Vector3(0, transform.rotation.eulerAngles.y, 0)));
        return angle;
    }

    public void OnCollectButtonDown()
    {
        if (!isCollecting)
        {
            SetDestination();
            GetComponent<PlayerAttack>().CancelAttack();
        }

    }

    private void SetDestination()
    {
        FindClosestCollect();
        if (closestCollect != null)
        {
            isCollecting = true;
            agent.enabled = true;
            controller.enabled = false;
            if (closestCollect.transform.CompareTag("Stone"))
            {
                agent.stoppingDistance = 2;
            }
            else if (closestCollect.transform.CompareTag("Tree"))
            {
                agent.stoppingDistance = 2;

            }
            else
            {
                agent.stoppingDistance = 1.5f;
            }
            Vector3 direction = (closestCollect.transform.position - transform.position).normalized;
            agent.SetDestination(closestCollect.transform.position - (direction));
        }
        else
        {
            if (Inventory.instance.IsFull())
            {
                Debug.Log("Inventory is full!");
            }
            else
            {
                print("Toplanacak bir şey kalmadı.");
            }
        }
    }

    void HighlightTarget(Transform target)
    {
        ClearHighlights();
        target.GetComponent<HighlightEffect>().SetHighlighted(true);
    }

    void ClearHighlights()
    {
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Stone"))
        {
            obj.GetComponent<HighlightEffect>().SetHighlighted(false);
        }
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Tree"))
        {
            obj.GetComponent<HighlightEffect>().SetHighlighted(false);
        }
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Collectable"))
        {
            obj.GetComponent<HighlightEffect>()?.SetHighlighted(false);
        }
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("ProductionStation"))
        {
            obj.GetComponent<HighlightEffect>().SetHighlighted(false);
        }
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Smelter"))
        {
            obj.GetComponent<HighlightEffect>().SetHighlighted(false);
        }
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Storage"))
        {
            obj.GetComponent<HighlightEffect>().SetHighlighted(false);
        }
    }



    public void Collect()
    {
        closestCollect.GetComponent<ItemPickup>().PickUp();
    }

    public void Gather()
    {
        closestCollect.GetComponent<GatherableManager>().Gather(tool);

    }

    public void CollectEnd()
    {
        isCollecting = false;
        if (equipmentSwapped)
        {
            EquipmentManager.instance.VisualUnequip((int)EquipmentTypes.Weapon);
            equipmentSwapped = false;
        }
        if (isAutoGathering)
        {
            SetDestination();
        }
    }


}
