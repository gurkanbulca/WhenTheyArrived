using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using CoffeeRush.Utils;


public class BuildingManager : MonoBehaviour
{
    #region Singleton

    public static BuildingManager instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Instance already casted!");
            return;
        }
        instance = this;
    }

    #endregion


    GridManager gridManager;
    GameObject pickedBuilding, originalSample, highlightedBuilding, replacingObject;
    Transform replacingObjectParent;
    Vector3 oldPosition;
    MyGrid grid;
    Material mainMat;

    public LayerMask foundationLayer;
    public Transform playerBuildings;
    public GameObject placementUI;
    public BuildingUI buildingUI;
    public List<CraftingRecipe> buildingRecipes;
    public CraftingRecipe recipe;

    private void Start()
    {
        gridManager = GridManager.instace;
        placementUI = Instantiate(placementUI);
        placementUI.SetActive(false);
        buildingUI.CreateBuildingSlotsByRecipeList(buildingRecipes);
    }




    private void Update()
    {
        if (pickedBuilding != null)
        {
            Vector3 mousePosition = CoffeeUtils.GetMousePositionOnWorld(LayerMask.GetMask("Ground"));
            switch (pickedBuilding.GetComponent<Building>().buildingType)
            {
                case BuildingType.Foundation:
                    Vector3 cellCenterPosition = gridManager.GetGrid().GetCellCenterPosition(mousePosition);
                    pickedBuilding.transform.position = cellCenterPosition;
                    gridManager.UpdateCellValues();
                    if (grid.GetValue(pickedBuilding.transform.position) == 1)
                    {
                        SetColor(pickedBuilding, Color.green);
                        // Foundation placing
                        if (Input.GetMouseButtonDown(0))
                        {
                            if (!EventSystem.current.IsPointerOverGameObject())
                            {
                                PlaceBuilding(pickedBuilding);
                                break;
                            }

                        }
                    }
                    else
                    {
                        SetColor(pickedBuilding, Color.red);
                    }

                    break;
                case BuildingType.Wall:
                case BuildingType.Doorway:
                    Collider[] colliders = Physics.OverlapSphere(mousePosition, 0.5f, foundationLayer);
                    if (colliders.Length > 0) // is there any foundation on mouse position ?
                    {
                        // picking closest snap point on foundation by mouse position.
                        Transform foundation = colliders[0].transform;
                        Transform closestSnapPoint = FindClosestSnapPointOnFoundation(mousePosition, foundation);

                        // pickedBuiding set position under closest snap point.
                        pickedBuilding.transform.parent = closestSnapPoint;
                        pickedBuilding.transform.localPosition = Vector3.zero;
                        pickedBuilding.transform.localRotation = Quaternion.Euler(Vector3.zero);

                        // is foundation have another building part on snap point?
                        if (pickedBuilding.transform.parent.childCount == 1)
                        {
                            // is there any building part on neighbor snap point?
                            Transform neighborFoundation = null;
                            colliders = Physics.OverlapSphere(pickedBuilding.transform.position, 1f, foundationLayer);
                            foreach (Collider collider in colliders)
                            {
                                if (Vector3.Distance(collider.transform.position, foundation.position) != 0)
                                {
                                    neighborFoundation = collider.transform;
                                    break;
                                }
                            }
                            closestSnapPoint = neighborFoundation != null ? FindClosestSnapPointOnFoundation(pickedBuilding.transform.position, neighborFoundation) : null;
                            float childCount = closestSnapPoint != null ? closestSnapPoint.childCount : 0;
                            if (childCount == 0)
                            {
                                SetColor(pickedBuilding, Color.green);
                                // Wall placing
                                if (Input.GetMouseButtonDown(0))
                                {
                                    PlaceBuilding(pickedBuilding);
                                }

                            }
                            else // no building part on neighbor snap point.
                            {
                                SetColor(pickedBuilding, Color.red);
                            }

                        }
                        else // picked building is not only building part on snap point.
                        {
                            SetColor(pickedBuilding, Color.red);

                        }
                    }
                    else // there is no foundation on mouse position.
                    {
                        pickedBuilding.transform.position = mousePosition;
                        SetColor(pickedBuilding, Color.red);
                    }
                    break;
                case BuildingType.Door:
                    pickedBuilding.GetComponent<DoorController>().enabled = false;
                    colliders = Physics.OverlapSphere(mousePosition, 0.5f, foundationLayer);
                    if (colliders.Length > 0) // is there any foundation on mouse position ?
                    {
                        Transform foundation = colliders[0].transform;
                        Transform closestSnapPoint = FindClosestSnapPointOnFoundation(mousePosition, foundation);
                        if (closestSnapPoint.childCount > 0)
                        {
                            if (closestSnapPoint.GetChild(0).GetComponent<Building>().buildingType == BuildingType.Doorway)
                            {
                                Transform doorway = closestSnapPoint.GetChild(0);
                                for (int i = 0; i < doorway.childCount; i++)
                                {
                                    if (doorway.GetChild(i).name.Equals("HingePoint"))
                                    {
                                        Transform hingePoint = doorway.GetChild(i);
                                        pickedBuilding.transform.parent = hingePoint;
                                        pickedBuilding.transform.localPosition = Vector3.zero;
                                        pickedBuilding.transform.localRotation = Quaternion.Euler(Vector3.zero);
                                        if (hingePoint.childCount == 1)
                                        {
                                            SetColor(pickedBuilding, Color.green);
                                            if (Input.GetMouseButtonDown(0))
                                            {
                                                pickedBuilding.GetComponent<DoorController>().enabled = true;
                                                PlaceBuilding(pickedBuilding);
                                            }
                                        }
                                        else
                                        {
                                            SetColor(pickedBuilding, Color.red);
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            pickedBuilding.transform.position = mousePosition;
                            SetColor(pickedBuilding, Color.red);
                        }

                    }
                    else
                    {
                        pickedBuilding.transform.position = mousePosition;
                        SetColor(pickedBuilding, Color.red);
                    }
                    break;
                default:
                    break;
            }
        }
        else
        {
            if (buildingUI.buildingUI.activeSelf)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    if (!EventSystem.current.IsPointerOverGameObject())
                    {
                        Vector3 mousePosition = CoffeeUtils.GetMousePositionOnWorld();
                        RaycastHit hit = new RaycastHit();
                        if (Physics.Raycast(Camera.main.transform.position, (mousePosition - Camera.main.transform.position).normalized, out hit))
                        {
                            InteractWithBuilding(hit.transform);
                        }
                    }

                }
            }

        }
    }

    public bool isBuildingAlreadyPicked()
    {
        return pickedBuilding != null;
    }

    public void ReplaceProtocol(GameObject replacingObject, Transform parent, Vector3 oldPosition)
    {
        this.replacingObject = replacingObject;
        this.replacingObjectParent = parent;
        this.oldPosition = oldPosition;
    }

    public void ReturnReplacement()
    {
        if (replacingObject != null)
        {
            replacingObject.transform.parent = replacingObjectParent;
            replacingObject.transform.position = oldPosition;
            replacingObject.transform.GetComponent<Renderer>().enabled = true;
            replacingObject = null;

        }
    }

    public void PickBuilding(GameObject building, BuildingType type)
    {
        destroyHighlight();
        if (pickedBuilding == null)
        {
            if (grid == null)
            {
                grid = gridManager.GetGrid();
            }
            originalSample = building;
            pickedBuilding = Instantiate(building, new Vector3(0, -100, 0), Quaternion.identity);
            pickedBuilding.GetComponent<Building>().buildingType = type;
            Renderer renderer = pickedBuilding.GetComponent<Renderer>();
            mainMat = renderer.material;
            //Material newMat = Instantiate(renderer.material);
            //renderer.material = newMat;
            mainMat.EnableKeyword("_EMISSION");
            SetColor(pickedBuilding, Color.green);

            pickedBuilding.GetComponent<MeshCollider>().convex = true;
            pickedBuilding.GetComponent<Collider>().isTrigger = true;

        }
        else
        {
            CancelPicking();
        }
    }
    public void CancelPicking()
    {
        if (pickedBuilding != null)
        {
            Destroy(pickedBuilding);
            pickedBuilding = null;
            originalSample = null;
            recipe = null;
        }
    }

    private GameObject PlaceBuilding(GameObject building)
    {
        if (recipe != null)
        {
            if (recipe.CanCraft(Inventory.instance))
            {
                recipe.CraftWithoutOutputs(Inventory.instance);
            }
            else
            {
                Debug.LogError("Not enough material to build!");
                return null;
            }
        }
        originalSample = null;
        building.GetComponent<Collider>().isTrigger = false;
        building.GetComponent<MeshCollider>().convex = false;
        InteractWithBuilding(building.transform);
        pickedBuilding = null;
        replacingObject = null;
        recipe = null;
        NavMeshBaker.instance.PushToSurfaces(building.GetComponent<NavMeshSurface>());
        NavMeshBaker.instance.BakeArea();
        return building;
    }

    private Transform FindClosestSnapPointOnFoundation(Vector3 position, Transform foundation)
    {
        float closestDistance = Mathf.Infinity;
        Transform closestSnapPoint = null;
        for (int i = 0; i < foundation.childCount; i++)
        {
            Transform child = foundation.GetChild(i);
            if (child.CompareTag("SnapPoint"))
            {
                float distance = Vector3.Distance(child.position, position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestSnapPoint = child;
                }
            }
        }
        return closestSnapPoint;
    }

    private void InteractWithBuilding(Transform building)
    {
        if (building.tag.Equals("BuildingPart"))
        {


            placementUI.transform.SetParent(building);
            if (building.GetComponent<Building>().buildingType == BuildingType.Door)
            {
                placementUI.transform.localPosition = new Vector3(-1.15f, 6, 0);
            }
            else
            {
                placementUI.transform.localPosition = new Vector3(0, 9, 0);
            }
            placementUI.SetActive(true);
            if (highlightedBuilding != null)
            {
                destroyHighlight();
            }

            CreateHighlight(building.gameObject);
            highlightedBuilding = building.gameObject;


        }
    }

    private void CreateHighlight(GameObject obj)
    {
        mainMat = obj.GetComponent<Renderer>().material;
        //Material newMat = Instantiate(mainMat);
        //obj.GetComponent<Renderer>().material = newMat;
        SetColor(obj, Color.green);
        placementUI.SetActive(true);
    }

    public void destroyHighlight()
    {

        if (highlightedBuilding != null)
        {
            //highlightedBuilding.GetComponent<Renderer>().material = mainMat;
            SetColor(highlightedBuilding, Color.black);
            highlightedBuilding = null;
            placementUI.SetActive(false);
        }

    }

    public GameObject GetHighlight()
    {
        return highlightedBuilding;
    }

    private void SetColor(GameObject building, Color color)
    {
        Material material = building.GetComponent<Renderer>().material;

        material.EnableKeyword("_EMISSION");
        material.SetColor("_EmissionColor", color);


    }


}

public enum BuildingType
{
    Foundation, Wall, Doorway, Door
}
