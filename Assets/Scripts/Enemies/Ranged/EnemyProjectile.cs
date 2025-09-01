using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [SerializeField] private float speed = 16f;
    [SerializeField] private float maxDistance = 20f;
    [SerializeField] private float damage = 10f;
    private IDamageable _source;
    private Vector3 _start;
    public void Init(float spd, float maxDist, float dmg, IDamageable src)
    {
        speed = spd; maxDistance = maxDist; damage = dmg; _source = src;
    }
    private void Awake()
    {
        _start = transform.position;
    }

    private void FixedUpdate()
    {
        transform.position += transform.forward * (speed * Time.deltaTime);
        if ((transform.position - _start).sqrMagnitude >= maxDistance * maxDistance)
            Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out ActorContext context))
        {
            context.Hp.ReceiveDamage(damage, _source);
            Destroy(gameObject);
        }
    }
}
