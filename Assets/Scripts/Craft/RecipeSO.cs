using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RecipeSO", menuName = "Crafting/RecipeSO")]
public class RecipeSO : ScriptableObject
{
    public ItemSO RequiredItem;
    public List<CharmCost> Charms;
    public ItemSO ResultItem;
    [NonSerialized] public bool IsDiscovered;
}
