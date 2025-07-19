using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BaseEnemyHP))]
public class CombustDebuff : MonoBehaviour
{
    public bool IsAmplifyPhase  { get; private set; }
    public bool IsActive { get; private set; }
    
    private float _explosionDmg;
    private float _explosionRad;
    private GameObject _vfxPrefab;
    private PlayerContext _ownerCtx;
    private Coroutine _lifeRoutine;
    private GameObject _vfxInst;
    private BaseEnemyHP _hp;
    
    public bool Activate(float duration, float explosionDmg, float explosionRad, GameObject vfxPrefab, PlayerContext ownerCtx)
    {
        if (IsActive) return false;
        IsActive = true;
        IsAmplifyPhase = true;

        _explosionDmg = explosionDmg;
        _explosionRad = explosionRad;
        _vfxPrefab = vfxPrefab;
        _ownerCtx = ownerCtx;

        if (_hp == null) _hp = GetComponent<BaseEnemyHP>();

        if (_vfxPrefab)
        {
            _vfxInst = Instantiate(_vfxPrefab, transform);
            _vfxInst.transform.localPosition = Vector3.zero;
        }

        _lifeRoutine = StartCoroutine(LifeTimer(duration));
        return true;
    }

    private IEnumerator LifeTimer(float duration)
    {
        yield return new WaitForSeconds(duration);
        IsAmplifyPhase = false;

        Explode();
        ResetState();
    }
    
    private void Explode()
    {
        Debug.Log($"<color=orange>[Combust]</color> {_hp.name} EXPLODES!");

        _hp.ReceiveDamage(_explosionDmg, SkillDamageType.Basic);

        Collider[] hits = Physics.OverlapSphere(transform.position, _explosionRad);
        foreach (var h in hits)
        {
            if (!h.TryGetComponent(out BaseEnemyHP enemy)) continue;
            if (enemy == _hp) continue;

            enemy.ReceiveDamage(_explosionDmg, SkillDamageType.Basic);
            _ownerCtx.FireOnDamageDealt(enemy, _explosionDmg, SkillDamageType.Basic);

            Debug.Log($"<color=orange>[Combust]</color> AoE hit {enemy.name} " +
                      $"-{_explosionDmg:F1}");
        }
    }
    
    private void ResetState()
    {
        if (_lifeRoutine != null)
        {
            StopCoroutine(_lifeRoutine);
            _lifeRoutine = null;
        }

        IsActive = false;
        IsAmplifyPhase = false;

        if (_vfxInst) Destroy(_vfxInst);
    }

    private void OnDisable() => ResetState();
}
