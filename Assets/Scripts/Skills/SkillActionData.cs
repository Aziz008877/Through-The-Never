using UnityEngine;

public abstract class SkillActionData : ScriptableObject
{
    public abstract void Activate(PlayerSkillHandler handler);
}