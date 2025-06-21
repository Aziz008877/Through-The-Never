using UnityEngine;

public class BaseSkill : MonoBehaviour
{
    [SerializeField] protected SkillUIData _skillUIData;
    [SerializeField] protected SkillType _skillType;
    [SerializeField] protected SkillDamageType _skillDamageType;
    [SerializeField] protected float _coolDown;
    [SerializeField] protected float _damage;
    [SerializeField] protected float _duration;
    public virtual void Init()
    {
        
    }
    
    public virtual void Init(GameObject player)
    {
        
    }
    
    public virtual void PerformSkill()
    {
        
    }

    public virtual void PerformSkill(GameObject player)
    {
        
    }

    public virtual void UpdateSkill()
    {
        
    }
}

public enum SkillType
{
    Active,
    SchoolPassive,
    GlobalPassive
}

public enum SkillDamageType
{
    Basic,
    DOT,
    AOE,
    NoDamage
}