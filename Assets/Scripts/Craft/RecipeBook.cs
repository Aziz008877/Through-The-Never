using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class RecipeBook : MonoBehaviour
{
    [SerializeField] private RecipeSO[] _allRecipes;
    public IEnumerable<RecipeSO> All => _allRecipes;
}