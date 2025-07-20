using System;
using System.Collections.Generic;
using UnityEngine;

public class CharmBank : MonoBehaviour
{
    public event Action<CharmSO, int> OnCharmChanged;

    private readonly Dictionary<CharmSO, int> _bank = new();

    public void Add(CharmSO school, int amount)
    {
        if (!_bank.ContainsKey(school))
            _bank[school] = 0;

        _bank[school] += amount;
        OnCharmChanged?.Invoke(school, _bank[school]);
    }

    public bool CanAfford(IEnumerable<CharmCost> costs)
    {
        foreach (var cost in costs)
        {
            if (!_bank.TryGetValue(cost.CharmType, out var have) || have < cost.Amount)
                return false;
        }
        return true;
    }

    public void Spend(IEnumerable<CharmCost> costs)
    {
        foreach (var cost in costs)
        {
            _bank[cost.CharmType] -= cost.Amount;
            OnCharmChanged?.Invoke(cost.CharmType, _bank[cost.CharmType]);
        }
    }

    public int GetAmount(CharmSO school) =>
        _bank.TryGetValue(school, out var value) ? value : 0;
    
    public Dictionary<CharmSO, int> GetAll()
    {
        return _bank;
    }

}