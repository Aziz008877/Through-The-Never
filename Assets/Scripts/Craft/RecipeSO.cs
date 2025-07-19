using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RecipeSO", menuName = "Crafting/RecipeSO")]
public class RecipeSO : ScriptableObject
{
    [Header("Input")]
    public ItemSO RequiredItem;
    public List<CharmCost> Charms;

    [Header("Output")]
    public ItemSO ResultItem;
    public Sprite PreviewIcon;
}
