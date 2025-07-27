using UnityEngine;
public class RefractionPassive : PassiveSkillBehaviour
{
    [SerializeField, Range(0f,1f)] private float _chanceBoost = 0.15f;
    public override void EnablePassive() => PlayerContext.AddCritChance(_chanceBoost);
    public override void DisablePassive() => PlayerContext.AddCritChance(-_chanceBoost);
}