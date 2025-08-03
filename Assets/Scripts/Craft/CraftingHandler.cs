using System.Collections.Generic;
using UnityEngine;
public class CraftingHandler : MonoBehaviour
{
    [SerializeField] private CharmBank _charmBank;
    [SerializeField] private ItemInventory _itemInventory;
    [SerializeField] private RecipeBook _recipeBook;
    public bool TryCraft(ItemSO inputItem, List<CharmCost> inputCharms)
    {
        foreach (var recipe in _recipeBook.All)
        {
            if (recipe.RequiredItem != inputItem)
                continue;

            if (!MatchCosts(recipe.Charms, inputCharms))
                continue;

            if (!_charmBank.CanAfford(recipe.Charms))
                return false;

            _charmBank.Spend(recipe.Charms);
            _itemInventory.Remove(inputItem);
            _itemInventory.Add(recipe.ResultItem);
            _recipeBook.MarkDiscovered(recipe);
            Debug.Log($"Crafted: {recipe.ResultItem.DisplayName}");
            return true;
        }

        Debug.Log("No matching recipe.");
        return false;
    }

    private bool MatchCosts(List<CharmCost> recipe, List<CharmCost> input)
    {
        if (recipe.Count != input.Count)
            return false;

        foreach (var r in recipe)
        {
            var match = input.Find(c => c.CharmType == r.CharmType);
            if (match.CharmType == null || match.Amount < r.Amount)
                return false;
        }
        
        return true;
    }
}