using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingManager : MonoBehaviour
{
    public List<CraftingRecipe> playerRecipes;
    CraftingUI craftingUI;

    private void Start()
    {
        craftingUI = CraftingUI.instance;
        craftingUI.GenerateRecipeList(playerRecipes);
    }
}
