using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemInventory : MonoBehaviour
{
    public event Action OnInventoryChanged;

    private readonly List<ItemSO> _items = new();

    public void Add(ItemSO item)
    {
        _items.Add(item);
        OnInventoryChanged?.Invoke();
    }
    public void Remove(ItemSO item)
    {
        _items.Remove(item);
        OnInventoryChanged?.Invoke();
    }
    public bool Has(ItemSO item) => _items.Contains(item);

    public IEnumerable<ItemSO> All => _items;
}