using System.Threading.Tasks;
using UnityEngine;
public class Fireball : MonoBehaviour
{
    [SerializeField] private ParticleSystem _castParticles;
    [SerializeField] private ParticleSystem _fireballParticles;
    [SerializeField] private ParticleSystem _fireballHit;
    [SerializeField] private float _speed;
    private SkillDamageType _skillDamageType;
    private PlayerContext _context;
    private float _damage;
    private bool _canDamage = true;
    public void Init(float damage, float duration, SkillDamageType skillDamageType, PlayerContext context)
    {
        _damage = damage;
        _skillDamageType = skillDamageType;
        _context = context;
        _castParticles.Play();
        Invoke(nameof(DestroyFireball), duration);
    }
    private void Update()
    {
        transform.position += transform.forward * _speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out IDamageable target) || !_canDamage) return;
        
        float finalDamage = _damage;
        SkillDamageType finalType = _skillDamageType;
        _context.ApplyDamageModifiers(ref finalDamage, ref finalType);

        target.ReceiveDamage(finalDamage, finalType);
        DestroyFireball();
    }

    private async void DestroyFireball()
    {
        _canDamage = false;
        _fireballHit.Play();
        _fireballParticles.gameObject.SetActive(false);
        _speed = 0;

        await Task.Delay(1500);

        if (this != null && gameObject != null)
        {
            Destroy(gameObject);
        }
    }
}
