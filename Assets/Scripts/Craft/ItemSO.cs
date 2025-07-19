using UnityEngine;

[CreateAssetMenu(fileName = "ItemSO", menuName = "Crafting/Item")]
public class ItemSO : ScriptableObject
{
    [Header("Visuals")]
    public string DisplayName;
    public Sprite Icon;
}
