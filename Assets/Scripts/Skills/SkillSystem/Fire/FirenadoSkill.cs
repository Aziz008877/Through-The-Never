using UnityEngine;
public class FirenadoSkill : ActiveSkillBehaviour
{
    [SerializeField] private FirenadoTornado _tornadoPrefab;
    [SerializeField] private float _pullForce = 30f;
    [SerializeField] private float _lifetime  = 7f;
    public override void TryCast()
    {
        if (!IsReady) return;

        var tornado = Instantiate(_tornadoPrefab, PlayerContext.transform.position, Quaternion.identity);
        tornado.Init(Definition.Damage, _pullForce, _lifetime, PlayerContext);

        StartCooldown();
    }
}
