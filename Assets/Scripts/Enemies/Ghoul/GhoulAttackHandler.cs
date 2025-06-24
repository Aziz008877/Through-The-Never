using UnityEngine;

public class GhoulAttackHandler : MonoBehaviour
{
    [SerializeField] private ParticleSystem _leftHandSlashVFX, _rightHandSlashVFX;
    [SerializeField] private Transform _target;
    [SerializeField] private float _damage, _meleeDamageDistance;
    [SerializeField] private CameraShake _cameraShake;
    public void FirstMeleeAttack()
    {
        _cameraShake.Shake();
        _rightHandSlashVFX.Play();

        float distance = Vector3.Distance(transform.position, _target.position);

        if (distance <= _meleeDamageDistance)
        {
            _target.GetComponent<PlayerHP>().ReceiveDamage(_damage);
        }
    }
    
    public void SecondMeleeAttack()
    {
        _cameraShake.Shake();
        _leftHandSlashVFX.Play();
        
        float distance = Vector3.Distance(transform.position, _target.position);
        
        if (distance <= _meleeDamageDistance)
        {
            _target.GetComponent<PlayerHP>().ReceiveDamage(_damage);
        }
    }

    public void ThirdMeleeAttack()
    {
        _cameraShake.Shake();
        _rightHandSlashVFX.Play();
        
        float distance = Vector3.Distance(transform.position, _target.position);
        
        if (distance <= _meleeDamageDistance)
        {
            _target.GetComponent<PlayerHP>().ReceiveDamage(_damage);
        }
    }

    public void RangeAttack()
    {
        
    }
}
