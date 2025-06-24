using UnityEngine;

public class BaseSkill : MonoBehaviour
{
    [SerializeField] protected SkillUIData _skillUIData;
    [SerializeField] protected SkillType _skillType;
    [SerializeField] protected SkillDamageType _skillDamageType;
    [SerializeField] protected float _coolDown;
    [SerializeField] protected float _damage;
    [SerializeField] protected float _duration;
    [SerializeField] protected float _radius;
    protected float _cooldownTimer = 0f;
    public bool IsReady => _cooldownTimer <= 0f;
    public virtual void Init()
    {
        
    }
    
    public virtual void Init(GameObject player)
    {
        
    }
    
    public virtual void PerformSkill()
    {
        _cooldownTimer = _coolDown;
    }

    public virtual void PerformSkill(GameObject player)
    {
        if (!IsReady) return;
        _cooldownTimer = _coolDown;
    }

    public virtual void UpdateSkill()
    {
        
    }
    
    protected virtual void Update()
    {
        if (_cooldownTimer > 0f)
            _cooldownTimer -= Time.deltaTime;
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