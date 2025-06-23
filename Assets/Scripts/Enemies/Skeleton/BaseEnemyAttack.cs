using UnityEngine;

public class BaseEnemyAttack : MonoBehaviour
{
    [SerializeField] private float _hitDistance = 2f;
    [SerializeField] private int _damage = 10;
    private Transform _target;

    public void PrepareAttack(Transform target)
    {
        _target = target;
    }

    // animation event
    public void TryApplyDamage()
    {
        if (_target == null) return;

        float distance = Vector3.Distance(transform.position, _target.position);
        if (distance <= _hitDistance)
        {
            Debug.Log("Попал!");
            _target.GetComponent<PlayerHP>().ReceiveDamage(_damage);
        }
        else
        {
            Debug.Log("Мимо!");
        }
    }
}
