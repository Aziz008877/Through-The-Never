using UnityEngine;
using Zenject;

public class PlayerSkillHandler : MonoBehaviour
{
    [Inject] private PlayerDash _playerDash;
    [Inject] private FireballSkill _fireballSkill;
    [Inject] private SkillUIHandler _skillUIHandler;
    [Inject] private PlayerMove _playerMove;
    
    [SerializeField] private FirebeamSkill _firebeamSkill;
    public PlayerDash PlayerDash => _playerDash;
    public FireballSkill FireballSkill => _fireballSkill;
    public SkillUIHandler SkillUIHandler => _skillUIHandler;
    public FirebeamSkill FirebeamSkill => _firebeamSkill;
    public PlayerMove PlayerMove => _playerMove;
}
