using UnityEngine;
using Zenject;

public class PlayerSkillHandler : MonoBehaviour
{
    [Inject] private PlayerDash _playerDash;
    [Inject] private FireballSkill _fireballSkill;
    [Inject] private SkillUIHandler _skillUIHandler;
    public void ApplySelectedSkill(int skillID)
    {
        switch (skillID)
        {
            case 0:
                _playerDash.gameObject.SetActive(true);
                _skillUIHandler.ShowUISkill(skillID);
                break;
            
            case 1:
                _fireballSkill.UpdateSkill();
                break;
            
            case 2:
                _fireballSkill.BuffDamage(20);
                break;
        }
    }
}
