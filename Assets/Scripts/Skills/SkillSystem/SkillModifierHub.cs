using System.Collections.Generic;
using UnityEngine;
public class SkillModifierHub : MonoBehaviour
{
    private readonly List<ISkillModifier> _mods = new();

    public void Register  (ISkillModifier m) => _mods.Add(m);
    public void Unregister(ISkillModifier m) => _mods.Remove(m);

    public float Apply(SkillKey key, float baseValue)
    {
        float val = baseValue;
        foreach (var m in _mods) val = m.Evaluate(key, val);
        return val;
    }
}
