using System.Collections.Generic;
using UnityEngine;
public class SmolderRingArea : MonoBehaviour
{
    [SerializeField] private float _radius = 3f;
    [SerializeField] private float _dps = 5f;
    [SerializeField] private float _duration = 3f;
    [SerializeField] private float _projectileBoost = 1.3f;
    private readonly HashSet<IDamageable> _ticked = new();
    private ActorContext _context;
    public void Init(ActorContext ctx)
    {
        _context = ctx;
        var col = GetComponent<SphereCollider>();
        col.isTrigger = true;
        col.radius = _radius;
        Destroy(gameObject, _duration);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent(out IDamageable enemy) && !_ticked.Contains(enemy))
        {
            var ctx = new DamageContext
            {
                Attacker       = _context,
                Target         = enemy,
                SkillBehaviour = null,
                SkillDef       = null,
                Slot           = SkillSlot.Undefined,
                Type           = SkillDamageType.Basic,
                Damage         = _dps,
                IsCrit         = false,
                CritMultiplier = 1f,
                HitPoint       = other.transform.position,
                HitNormal      = Vector3.up,
                SourceGO       = gameObject
            };

            _context.ApplyDamageContextModifiers(ref ctx);
            enemy.ReceiveDamage(ctx);
            _ticked.Add(enemy);
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IProjectileBoostable proj))
            proj.BoostDamage(_projectileBoost);
    }

    private void LateUpdate() => _ticked.Clear();
}
