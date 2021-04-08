using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "New Tool", menuName = "Inventory/Tool")]
public class Tool : Equipment
{
    public ToolType toolType;
    [Range(0,2)]
    public int gatherLevel;
}

public enum ToolType
{
    Chopping,Mining,Multitool
}
