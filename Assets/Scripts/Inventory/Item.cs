using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{

    new public string name = "New Item";
    public string description = "No description.";
    public Sprite icon = null;
    public bool isDefaultItem = false;

    public bool isStackable = false;
    public int stackSize=1;
    public int amount = 1;


    private void OnValidate()
    {
        if (!isStackable)
        {
            stackSize = 1;
        }
    }
     
    public virtual void Use()
    {
        // use the item
        // Something might happen
        Debug.Log("using " + name);
    }

    public void RemoveFromInventory(int amount = 1)
    {
        Inventory.instance.Remove(this,amount);
    }

}
