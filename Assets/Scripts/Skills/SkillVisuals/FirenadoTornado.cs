using System.Collections;
using UnityEngine;
public class FirenadoTornado : MonoBehaviour
{
    [SerializeField] private float _radius = 4f;
    [SerializeField] private float _tickInterval = 1f;
    private float _dps, _pullForce;
    private PlayerContext _context;
    public void Init(float damagePerSecond, float pullForce, float lifetime, PlayerContext context)
    {
        _dps = damagePerSecond;
        _pullForce = pullForce;
        _context = context;

        StartCoroutine(DestroySelf(lifetime));
        InvokeRepeating(nameof(ApplyTick), 0f, _tickInterval);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.attachedRigidbody != null)
        {
            Vector3 dir = (transform.position - other.transform.position).normalized;
            other.attachedRigidbody.AddForce(dir * _pullForce, ForceMode.Acceleration);
        }
    }

    private void ApplyTick()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, _radius);
        foreach (var h in hits)
        {
            if (!h.TryGetComponent(out IDamageable dmg)) continue;

            float tickDamage = _dps * _tickInterval;
            SkillDamageType type = SkillDamageType.Basic;
            _context.ApplyDamageModifiers(ref tickDamage, ref type);
            dmg.ReceiveDamage(tickDamage, type);
        }
    }

    private IEnumerator DestroySelf(float time)
    {
        yield return new WaitForSeconds(time); 
        CancelInvoke(); 
        Destroy(gameObject);
    }
}
