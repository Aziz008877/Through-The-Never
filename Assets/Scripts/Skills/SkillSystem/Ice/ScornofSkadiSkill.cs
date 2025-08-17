using System.Collections;
using UnityEngine;

public class ScornOfSkadiPassive : PassiveSkillBehaviour
{
    [SerializeField] private float _healPercent = 0.2f;
    [SerializeField] private float _healDuration = 4f;
    private bool _usedThisBattle = false;
    private Coroutine _healRoutine;

    public override void EnablePassive()
    {
        Context.Hp.OnIncomingDamage += OnIncomingDamage;
        MockCombatStart();
    }

    public override void DisablePassive()
    {
        Context.Hp.OnIncomingDamage -= OnIncomingDamage;
        if (_healRoutine != null) { StopCoroutine(_healRoutine); _healRoutine = null; }
    }

    private void OnIncomingDamage(ref float damage, IDamageable source)
    {
        if (_usedThisBattle) return;
        float hpAfter = Context.Hp.CurrentHP - damage;
        if (hpAfter > Context.Hp.MinHP) return;

        damage = 0f;
        _usedThisBattle = true;

        if (_healRoutine != null) StopCoroutine(_healRoutine);
        _healRoutine = StartCoroutine(HealRoutine(Context.Hp.MaxHP * _healPercent, _healDuration));
    }

    private IEnumerator HealRoutine(float amount, float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            float dt = Mathf.Min(Time.deltaTime, duration - t);
            Context.Hp.ReceiveHP(amount * (dt / duration));
            t += dt;
            yield return null;
        }
        _healRoutine = null;
    }

    public void MockCombatStart()
    {
        //_usedThisBattle = false;
    }

    public void MockCombatEnd()
    {
        _usedThisBattle = false;
    }
}