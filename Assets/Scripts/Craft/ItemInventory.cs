using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemInventory : MonoBehaviour
{
    private readonly List<ItemStack> _bag = new();
    public IReadOnlyList<ItemStack> Bag => _bag;
    private readonly ItemSO[] _hotbar = new ItemSO[6];
    public event Action OnChanged;
    public void Add(ItemSO item, int count = 1)
    {
        var st = _bag.Find(s => s.Item == item);
        if (st == null) _bag.Add(new ItemStack(item, count));
        else            st.Count += count;
        OnChanged?.Invoke();
    }

    public void Remove(ItemSO item, int count = 1)
    {
        var st = _bag.Find(s => s.Item == item);
        if (st == null) return;
        st.Count -= count;
        if (st.Count <= 0) _bag.Remove(st);
        
        /*for (int i = 0; i < _hotbar.Length; i++)
            if (_hotbar[i] == item && !Has(item))
                _hotbar[i] = null;*/

        OnChanged?.Invoke();
    }

    public bool Has(ItemSO item) => _bag.Exists(s => s.Item == item);

    public bool SetHotbarSlot(int idx, ItemSO item)
    {
        if (item.Role != ItemRole.Scroll) return false;
        //if (!Has(item)) return false;
        _hotbar[idx] = item;
        OnChanged?.Invoke();
        return true;
    }

    public int GetCount(ItemSO item)
    {
        var st = _bag.Find(s => s.Item == item);
        return st != null ? st.Count : 0;
    }
    
    public ItemSO GetHotbarSlot(int idx) => _hotbar[idx];
    public void  ClearHotbarSlot(int idx) { _hotbar[idx] = null; OnChanged?.Invoke(); }
}