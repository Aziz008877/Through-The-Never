using System.Collections.Generic;
using UnityEngine;
public class SmolderRingArea : MonoBehaviour
{
    [SerializeField] private float _radius = 3f;
    [SerializeField] private float _dps = 5f;
    [SerializeField] private float _duration = 3f;
    [SerializeField] private float _projectileBoost = 1.3f;
    private readonly HashSet<IDamageable> _ticked = new();
    private PlayerContext _context;
    public void Init(PlayerContext ctx)
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
            float dmg = _dps;
            SkillDamageType type = SkillDamageType.Basic;
            _context.ApplyDamageModifiers(ref dmg, ref type);
            _context.FireOnDamageDealt(enemy, dmg, type);
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
