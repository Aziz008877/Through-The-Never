using UnityEngine;
using Zenject;

public class PlayerContext : MonoBehaviour
{
    [SerializeField] private Transform _playerPosition;
    [Inject] private PlayerMove _playerMove;
    [Inject] private PlayerHP _playerHp;
    [Inject] private PlayerAnimator _playerAnimator;
    [Inject] private PlayerSkillHandler _playerSkillHandler;
    [Inject] private DamageTextPool _damageTextPool;
    
}
