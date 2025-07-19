using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BaseEnemyHP))]
public class IfritFervorDebuff : MonoBehaviour
{
    private float _timeLeft;
    private float _hpThreshold;
    private GameObject _vfxPrefab;
    private bool _raged;
    private bool _cleanedUp = false;
    private BaseEnemyHP _hp;
    private BaseEnemyAttack _attack;
    private BaseEnemyMove _move;
    private static readonly HashSet<BaseEnemyHP> _infected = new();
    
    public void Init(float duration, float hpThreshold, GameObject vfx)
    {
        _timeLeft = duration;
        _hpThreshold = Mathf.Clamp01(hpThreshold);
        _vfxPrefab = vfx;

        if (_hp == null)
        {
            _hp = GetComponent<BaseEnemyHP>();
            _attack = GetComponent<BaseEnemyAttack>();
            _move = GetComponent<BaseEnemyMove>();

            if (_vfxPrefab)
                Instantiate(_vfxPrefab, transform).transform.localPosition = Vector3.zero;
        }

        _raged = false;
        _cleanedUp = false;

        if (!_infected.Contains(_hp))
        {
            Debug.Log($"<color=orange>[Fervor]</color> applied to {_hp.name}");
            _infected.Add(_hp);
        }
    }

    private void Update()
    {
        if (_timeLeft > 0f)
        {
            _timeLeft -= Time.deltaTime;
        }
        else
        {
            Cleanup();
            return;
        }

        if (!_raged && _hp.CurrentHP / _hp.MaxHP <= _hpThreshold)
            EnterRage();
    }

    private void EnterRage()
    {
        _raged = true;
        Debug.Log($"<color=orange>[Fervor]</color> {_hp.name} RAGED!");

        Transform friend = FindNearestAlly();
        if (friend != null)
        {
            _move.ReceiveTargetEnemy(friend);
            _attack.SetTarget(friend);
        }

        _attack.OnAttackStarted += SpreadOnHit;
    }

    private void SpreadOnHit(float _)
    {
        Transform next = FindNearestAlly();
        if (next && next.TryGetComponent(out BaseEnemyHP hp) && !_infected.Contains(hp))
        {
            hp.gameObject.AddComponent<IfritFervorDebuff>()
              .Init(_timeLeft, _hpThreshold, _vfxPrefab);

            Debug.Log($"<color=orange>[Fervor]</color> spread to {hp.name}");
        }
    }

    private Transform FindNearestAlly()
    {
        float best = float.MaxValue; Transform bestT = null;
        foreach (var cand in FindObjectsOfType<BaseEnemyHP>())
        {
            if (cand == _hp) continue;
            float sqr = (cand.transform.position - transform.position).sqrMagnitude;
            if (sqr < best) { best = sqr; bestT = cand.transform; }
        }
        return bestT;
    }

    private void Cleanup()
    {
        if (_cleanedUp) return;
        _cleanedUp = true;

        _infected.Remove(_hp);

        if (_attack)
        {
            _attack.OnAttackStarted -= SpreadOnHit;

            PlayerHP player = FindObjectOfType<PlayerHP>();
            if (player != null)
            {
                _attack.SetTarget(player.transform);
                _move.ReceiveTargetEnemy(player.transform);
                Debug.Log($"<color=green>[Fervor]</color> {_hp.name} REVERTED to attacking PLAYER");
            }
        }

        Destroy(this);
    }

    private void OnDisable() => Cleanup();
}
