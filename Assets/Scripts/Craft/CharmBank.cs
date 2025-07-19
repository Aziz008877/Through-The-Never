using System;
using System.Collections.Generic;
using UnityEngine;
public class CharmBank : MonoBehaviour
{
    public event Action<CharmSO,int> OnCharmChanged;

    private readonly Dictionary<CharmSO,int> _bank = new();

    public void Add(CharmSO school, int amount)
    {
        if (!_bank.ContainsKey(school)) _bank[school] = 0;
        _bank[school] += amount;
        OnCharmChanged?.Invoke(school, _bank[school]);
    }

    public bool CanAfford(IEnumerable<CharmCost> costs)
    {
        foreach (var c in costs)
            if (!_bank.TryGetValue(c.CharmType, out var have) || have < c.Amount)
                return false;
        return true;
    }

    public void Spend(IEnumerable<CharmCost> costs)
    {
        foreach (var c in costs)
        {
            _bank[c.CharmType] -= c.Amount;
            OnCharmChanged?.Invoke(c.CharmType, _bank[c.CharmType]);
        }
    }

    public int Get(CharmSO school) =>
        _bank.TryGetValue(school, out var v) ? v : 0;
}