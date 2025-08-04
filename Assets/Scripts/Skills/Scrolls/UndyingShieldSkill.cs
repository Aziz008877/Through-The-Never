using System.Collections;
using UnityEngine;
public class UndyingShieldSkill : ActiveSkillBehaviour
{
    [Header("VFX / SFX")]
    [SerializeField] private GameObject _shieldVfxPrefab;

    private GameObject _shieldInstance;

    public override void TryCast()
    {
        if (!IsReady) return;
        base.TryCast();

        float shieldTime = Definition.Duration > 0f ? Definition.Duration : 5f;
        StartCoroutine(ShieldRoutine(shieldTime));
        StartCooldown();
    }

    private IEnumerator ShieldRoutine(float time)
    {
        _shieldInstance = Instantiate(_shieldVfxPrefab, Context.transform);
        Context.Hp.SetCanBeDamagedState(false);
        yield return new WaitForSeconds(time);
        Context.Hp.SetCanBeDamagedState(true);
        if (_shieldInstance) Destroy(_shieldInstance);
    }
}