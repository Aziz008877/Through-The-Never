using UnityEngine;
using Zenject;
public class FireballSkill : BaseSkill
{
    [SerializeField] private Fireball _fireball;
    [SerializeField] private Transform _spawnParent;
    [SerializeField] private FireballMode _fireballMode;
    [SerializeField] private AudioSource _fireballSound;
    [Inject] private PlayerAnimator _playerAnimator;
    [Inject] private CameraShake _cameraShake;
    [Inject] private PlayerInput _playerInput;
    private void Awake()
    {
        _playerInput.OnPlayerPressedBasic += PerformSkill;
    }
    
    public override void PerformSkill()
    {
        if (!IsReady) return;
        
        base.PerformSkill();
        
        //_playerAnimator.CastBasics();
        _cameraShake.Shake();
        _fireballSound.pitch = Random.Range(0.9f, 1.5f);
        _fireballSound.PlayOneShot(_fireballSound.clip);
        SpawnFireball(_spawnParent.rotation);

        if (_fireballMode == FireballMode.Triple)
        {
            float angleOffset = 15f;
            SpawnFireball(Quaternion.Euler(0f, _spawnParent.eulerAngles.y - angleOffset, 0f));
            SpawnFireball(Quaternion.Euler(0f, _spawnParent.eulerAngles.y + angleOffset, 0f));
        }
    }

    private void SpawnFireball(Quaternion rotation)
    {
        Fireball instance = Instantiate(_fireball, _spawnParent.position, rotation);
        instance.Init(_damage, _duration, _skillDamageType);
    }

    public override void UpdateSkill()
    {
        _fireballMode = FireballMode.Triple;
    }

    public void SetDotMode()
    {
        _skillDamageType = SkillDamageType.DOT;
    }

    public void BuffDamage(float percents)
    {
        _damage *= 1f + percents / 100f;
    }

    private void OnDestroy()
    {
        _playerInput.OnPlayerPressedBasic -= PerformSkill;
    }
}

public enum FireballMode
{
    Single,
    Triple
}
