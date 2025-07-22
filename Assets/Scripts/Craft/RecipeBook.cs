using System;
using System.Collections.Generic;
using UnityEngine;
public class RecipeBook : MonoBehaviour
{
    [SerializeField] private RecipeSO[] _allRecipes;
    public IEnumerable<RecipeSO> All => _allRecipes;
    public event Action<RecipeSO> OnRecipeDiscovered;
    
    public void MarkDiscovered(RecipeSO recipe)
    {
        if (recipe.IsDiscovered)                     // уже открыт
            return;

        recipe.IsDiscovered = true;
        OnRecipeDiscovered?.Invoke(recipe);          // уведомляем UI
    }
}