using Michsky.UI.ModernUIPack;
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

    WindowManager windowManager;

    private void Awake()
    {
        windowManager = MultiuseUI.instace.windowManager;

    }

    private void OnValidate()
    {
        if (!isStackable)
        {
            stackSize = 1;
        }
    }
     
    public virtual bool Use()
    {
        // use the item
        // Something might happen
        Debug.Log("using " + name);
        if (windowManager.currentWindowIndex != 0)
        {
            IItemContainer station = windowManager.windows[windowManager.currentWindowIndex].windowObject.GetComponent<ISidePanelUI>().GetStation();
            if (Inventory.instance.HasItem(this))
            {
                if (station.Add(this, this.amount))
                {
                    Inventory.instance.DestroyItem(this);
                }
            }
            else
            {
                if (Inventory.instance.Add(this, amount))
                {
                    station.DestroyItem(this);
                }
            }
            return false;
            
        }
        return true;
    }

    public void RemoveFromInventory()
    {
        Inventory.instance.DestroyItem(this);
    }

}
