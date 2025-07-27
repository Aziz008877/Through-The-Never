using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

public class PlayerContext : MonoBehaviour
{
    [SerializeField] private Transform _playerPosition, _playerCastPosition;
    [SerializeField] private SkillSelectionSaver _selectionSaver;
    public SkillSelectionSaver SkillSelectionSaver => _selectionSaver;
    public Renderer[] PlayerMeshes;
    public GameObject FireballModel;
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
    public PlayerSkillManager PlayerSkillManager => _playerSkillManager;
    
    [Inject] private PlayerEnemyHandler _playerEnemyHandler;
    public PlayerEnemyHandler PlayerEnemyHandler => _playerEnemyHandler;

    [Inject] private SkillModifierHub _skillModifierHub;
    public SkillModifierHub SkillModifierHub => _skillModifierHub;
    [Inject] private PlayerState _playerState;
    public PlayerState PlayerState => _playerState;
    [Inject] private DamageTextPool _damageTextPool;
    public bool SolarFlareCharge { get; set; }
    private readonly List<IOnDamageDealtModifier> _onDamageDealtModifiers = new();
    private readonly List<IDamageModifier> _damageModifiers = new();
    
    public float CritChance { get; private set; } = 100f;
    public float CritMultiplier { get; private set; } = 2f;
    public void RegisterModifier(IDamageModifier m) => _damageModifiers.Add(m);
    public void UnregisterModifier(IDamageModifier m) => _damageModifiers.Remove(m);

    public void ApplyDamageModifiers(ref float dmg, ref SkillDamageType type)
    {
        foreach (var mod in _damageModifiers)
            mod.Apply(ref dmg, ref type);
        
        if (CritChance > 0f && Random.value <= CritChance)
        {
            dmg *= CritMultiplier;
        }
    }

    public void RegisterOnDamageDealtModifier(IOnDamageDealtModifier ioOnDamageDealtModifier)
    {
        if (!_onDamageDealtModifiers.Contains(ioOnDamageDealtModifier))
            _onDamageDealtModifiers.Add(ioOnDamageDealtModifier);
    }

    public void UnregisterOnDamageDealtModifier(IOnDamageDealtModifier ioOnDamageDealtModifier)
    {
        _onDamageDealtModifiers.Remove(ioOnDamageDealtModifier);
    }
    
    public void FireOnDamageDealt(IDamageable target, float damage, SkillDamageType type)
    {
        foreach (var modifier in _onDamageDealtModifiers)
            modifier.OnDamageDealt(target, damage, type, this);
    }

    public void AddCritChance(float delta)
    {
        CritChance = Mathf.Max(0, CritChance + delta);
    }

    public void AddCritMultiplier(float delta)
    {
        CritMultiplier += delta;
    }
}
