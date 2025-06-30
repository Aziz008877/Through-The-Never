using UnityEngine;
using System.Collections.Generic;
using TMPro;
using Zenject;

public class GhoulAttackHandler : BaseEnemyAttack
{
    [Header("Ghoul Specific")]
    [SerializeField] private ParticleSystem _leftHandSlashVFX, _rightHandSlashVFX;
    [SerializeField] private GhoulFireball _ghoulFireball;
    [SerializeField] private CameraShake _cameraShake;
    [SerializeField] private Transform _fireballSpawnPoint;
    [SerializeField] private GameObject _skeletonPrefab;
    [SerializeField] private Transform[] _summonPoints;
    [SerializeField] private TextMeshProUGUI _currentAttackText;
    [Inject] private DamageTextPool _damageTextPool;
    public float SummonCooldown = 10f;
    public float StopDurationAfterSummon = 2f;

    private List<BaseEnemyHP> _spawnedSkeletons = new List<BaseEnemyHP>();
    private bool _canSummon = true;

    private void Update()
    {
        CheckSkeletonsAlive();
    }

    private void CheckSkeletonsAlive()
    {
        _spawnedSkeletons.RemoveAll(s => s == null);

        if (_spawnedSkeletons.Count == 0 && !_canSummon)
        {
            Invoke(nameof(ResetSummonCooldown), SummonCooldown);
            _canSummon = true;
        }
    }

    private void ResetSummonCooldown()
    {
        _canSummon = true;
    }

    public bool CanSummonSkeletons()
    {
        return _canSummon && _spawnedSkeletons.Count == 0;
    }

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
        _currentAttackText.text = "Melee Attack";
        float distance = Vector3.Distance(transform.position, _target.position);
        if (distance <= _meleeDistance)
        {
            _target.GetComponent<PlayerHP>()?.ReceiveDamage(_damage);
        }
    }

    public void SpawnSkeletons()
    {
        if (!CanSummonSkeletons()) return;

        _currentAttackText.text = "Summoning Skeletons";
        foreach (var point in _summonPoints)
        {
            GameObject skeleton = Instantiate(_skeletonPrefab, point.position, point.rotation);
            if (skeleton.TryGetComponent(out BaseEnemyHP skeletonHP))
            {
                skeletonHP.Init(_damageTextPool);
                skeleton.GetComponent<BaseEnemyAttack>().ReceiveTargetEnemy(_target);
                skeleton.GetComponent<BaseEnemyMove>().ReceiveTargetEnemy(_target);
                _spawnedSkeletons.Add(skeletonHP);
            }
        }

        _cameraShake.Shake();
        _canSummon = false;
    }

    public override void PerformRangeAttack()
    {
        _currentAttackText.text = "Range Attack";
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
