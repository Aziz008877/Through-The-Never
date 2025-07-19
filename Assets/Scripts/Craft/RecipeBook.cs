using System.Collections.Generic;
using UnityEngine;

public class RecipeBook : MonoBehaviour
{
    private readonly HashSet<RecipeSO> _known = new();
    public IEnumerable<RecipeSO> Known => _known;

    public bool IsKnown(RecipeSO r) => _known.Contains(r);

    public void Unlock(RecipeSO r)   => _known.Add(r);
}