using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class PlayerContext : MonoBehaviour
{
    [SerializeField] private Transform _playerPosition, _playerCastPosition;
    public Transform PlayerPosition => _playerPosition;
    public Transform PlayerCastPosition => _playerCastPosition;

    [Inject] private PlayerMove _playerMove;
    public PlayerMove PlayerMove => _playerMove;
    
    [Inject] private PlayerInput _playerInput;
    public PlayerInput PlayerInput => _playerInput;
    
    [Inject] private PlayerHP _playerHp;
    public PlayerHP PlayerHp => _playerHp;
    
    [Inject] private PlayerAnimator _playerAnimator;
    public PlayerAnimator PlayerAnimator => _playerAnimator;
    [Inject] private PlayerSkillManager _playerSkillManager;
    [Inject] private DamageTextPool _damageTextPool;
    public bool SolarFlareCharge { get; set; }
    private readonly List<IDamageModifier> _damageModifiers = new();
    public void RegisterModifier(IDamageModifier m)   => _damageModifiers.Add(m);
    public void UnregisterModifier(IDamageModifier m) => _damageModifiers.Remove(m);

    public void ApplyDamageModifiers(ref float dmg, ref SkillDamageType type)
    {
        foreach (var mod in _damageModifiers)
            mod.Apply(ref dmg, ref type);
    }
    
}
