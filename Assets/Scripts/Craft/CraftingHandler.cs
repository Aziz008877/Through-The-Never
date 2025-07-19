using System.Linq;
using UnityEngine;
public class CraftingHandler : MonoBehaviour
{
    [SerializeField] private CharmBank _charms;
    [SerializeField] private ItemInventory _inv;
    [SerializeField] private RecipeBook _book;

    public bool TryCraft(ItemSO item, CharmCost[] providedCosts)
    {
        // Ищем точное совпадение рецепта
        var recipes = Resources.LoadAll<RecipeSO>("Recipes");
        var target = recipes.FirstOrDefault(r => r.RequiredItem == item && MatchCosts(r.Charms, providedCosts));

        if (target == null) return false;
        if (!_charms.CanAfford(target.Charms)) return false;
        if (!_inv.Has(item)) return false;

        // списываем
        _charms.Spend(target.Charms);
        _inv.Remove(item);

        // выдаём результат
        _inv.Add(target.ResultItem);
        _book.Unlock(target);

        Debug.Log($"Crafted: {target.ResultItem.DisplayName}");
        return true;
    }

    private static bool MatchCosts(System.Collections.Generic.List<CharmCost> need, CharmCost[] given)
    {
        if (need.Count != given.Length) return false;
        foreach (var n in need)
        {
            var g = System.Array.Find(given, c => c.CharmType == n.CharmType);
            if (g.CharmType == null || g.Amount < n.Amount) return false;
        }
        return true;
    }
}
