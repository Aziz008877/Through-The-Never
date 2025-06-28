using UnityEngine;

public class GhoulAttackHandler : BaseEnemyAttack
{
    [Header("Ghoul Specific")]
    [SerializeField] private ParticleSystem _leftHandSlashVFX, _rightHandSlashVFX;
    [SerializeField] private GhoulFireball _ghoulFireball;
    [SerializeField] private CameraShake _cameraShake;
    [SerializeField] private Transform _fireballSpawnPoint;
    public void FirstMeleeAttack()
    {
        _cameraShake.Shake();
        _rightHandSlashVFX.Play();
        ApplyMeleeDamage();
    }

    public void SecondMeleeAttack()
    {
        _cameraShake.Shake();
        _leftHandSlashVFX.Play();
        ApplyMeleeDamage();
    }

    public void ThirdMeleeAttack()
    {
        _cameraShake.Shake();
        _rightHandSlashVFX.Play();
        ApplyMeleeDamage();
    }

    private void ApplyMeleeDamage()
    {
        if (_target == null) return;

        float distance = Vector3.Distance(transform.position, _target.position);
        
        if (distance <= _meleeDistance)
        {
            _target.GetComponent<PlayerHP>()?.ReceiveDamage(_damage);
        }
    }

    public override void PerformRangeAttack()
    {
        _cameraShake.Shake();

        if (_fireballSpawnPoint != null)
        {
            Instantiate(_ghoulFireball, _fireballSpawnPoint.position, _fireballSpawnPoint.rotation);
        }
        else
        {
            Instantiate(_ghoulFireball, transform.position, transform.rotation);
        }
    }
}