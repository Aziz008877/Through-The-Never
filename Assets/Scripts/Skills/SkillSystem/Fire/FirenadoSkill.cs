using UnityEngine;
public class FirenadoSkill : ActiveSkillBehaviour
{
    [SerializeField] private FirenadoTornado _tornadoPrefab;
    [SerializeField] private float _pullForce = 30f;
    public override void TryCast()
    {
        if (!IsReady) return;
        base.TryCast();
        var tornado = Instantiate(_tornadoPrefab, Context.transform.position, Quaternion.identity);
        tornado.Init(Damage, _pullForce, Definition.Duration, Context);

        StartCooldown();
    }
}
