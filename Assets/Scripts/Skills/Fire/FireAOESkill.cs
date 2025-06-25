using ModestTree;
using UnityEngine;

public class FireAOESkill : BaseSkill
{
    [SerializeField] private ParticleSystem _aoeParticles;
    public override void PerformSkill(GameObject player)
    {
        Vector3 position = player.transform.position;
        
        _aoeParticles.transform.position = position;
        _aoeParticles.Play();
        
        Collider[] hits = Physics.OverlapSphere(position, _radius);

        foreach (Collider col in hits)
        {
            if (col.TryGetComponent(out IDamageable iDamageable))
            {
                iDamageable.ReceiveDamage(_damage, _skillDamageType);
            }
        }
    }
}
