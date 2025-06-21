using UnityEngine;
using Zenject;
public class FireballSkill : BaseSkill
{
    [SerializeField] private Fireball _fireball;
    [SerializeField] private Transform _spawnParent;
    [SerializeField] private FireballMode _fireballMode;
    [Inject] private PlayerAnimator _playerAnimator;
    [Inject] private CameraShake _cameraShake;
    [Inject] private PlayerInput _playerInput;
    private void Awake()
    {
        _playerInput.OnPlayerPressedBasic += PerformSkill;
    }

    public override void PerformSkill()
    {
        if (_fireballMode == FireballMode.Single)
        {
            _playerAnimator.CastBasics();
            Fireball fireball = Instantiate(_fireball, _spawnParent.position, _spawnParent.rotation);
            _cameraShake.Shake();
            fireball.Init(_damage, _duration);
        }
        else if (_fireballMode == FireballMode.Triple)
        {
            Fireball center = Instantiate(_fireball, _spawnParent.position, _spawnParent.rotation);
            center.Init(_damage, _duration);
            
            Quaternion leftRotation = Quaternion.Euler(0f, _spawnParent.eulerAngles.y - 15f, 0f);
            Fireball left = Instantiate(_fireball, _spawnParent.position, leftRotation);
            left.Init(_damage, _duration);
            
            Quaternion rightRotation = Quaternion.Euler(0f, _spawnParent.eulerAngles.y + 15f, 0f);
            Fireball right = Instantiate(_fireball, _spawnParent.position, rightRotation);
            right.Init(_damage, _duration);
        }
    }

    public override void UpdateSkill()
    {
        _fireballMode = FireballMode.Triple;
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
