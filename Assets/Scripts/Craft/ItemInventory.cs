using System.Collections.Generic;
using UnityEngine;

public class ItemInventory : MonoBehaviour
{
    private readonly List<ItemSO> _items = new();

    public void Add(ItemSO item) => _items.Add(item);

    public void Remove(ItemSO item) => _items.Remove(item);

    public bool Has(ItemSO item) => _items.Contains(item);

    public IEnumerable<ItemSO> All => _items;
}