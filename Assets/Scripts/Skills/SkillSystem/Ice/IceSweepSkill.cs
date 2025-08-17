using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class IceSweepSkill : PassiveSkillBehaviour
{
    private ISkillManager _skillManager;
    private PlayerDashSkill _dash;
    private Coroutine _sweepRoutine;
    private bool _isDashing;
    private readonly HashSet<Collider> _alreadyFrozen = new();
    private Vector3 _lastPos;
    public override void EnablePassive()
    {
        _skillManager = Context.SkillManager;
        AttachToDash(_skillManager.GetActive(SkillSlot.Dash));
        _skillManager.ActiveRegistered += OnActiveRegistered;
    }

    public override void DisablePassive()
    {
        if (_skillManager != null) _skillManager.ActiveRegistered -= OnActiveRegistered;
        Detach();
    }

    private void OnActiveRegistered(SkillSlot slot, ActiveSkillBehaviour beh)
    {
        if (slot == SkillSlot.Dash) AttachToDash(beh);
    }

    private void AttachToDash(ActiveSkillBehaviour beh)
    {
        Detach();
        if (beh && beh.TryGetComponent(out PlayerDashSkill dash))
        {
            _dash = dash;
            _dash.OnDashStarted += OnDashStarted;
            _dash.OnDashEnded   += OnDashEnded;
        }
    }

    private void Detach()
    {
        if (_dash)
        {
            _dash.OnDashStarted -= OnDashStarted;
            _dash.OnDashEnded -= OnDashEnded;
            _dash = null;
        }
        if (_sweepRoutine != null)
        {
            StopCoroutine(_sweepRoutine);
            _sweepRoutine = null;
        }
        _isDashing = false;
        _alreadyFrozen.Clear();
    }

    private void OnDashStarted(Vector3 startPos)
    {
        _isDashing = true;
        _alreadyFrozen.Clear();
        _lastPos = startPos;
        if (_sweepRoutine == null) _sweepRoutine = StartCoroutine(SweepRoutine());
    }

    private void OnDashEnded(Vector3 endPos)
    {
        FinalSweep(endPos);
        _isDashing = false;
        if (_sweepRoutine != null)
        {
            StopCoroutine(_sweepRoutine);
            _sweepRoutine = null;
        }
        _alreadyFrozen.Clear();
    }

    private IEnumerator SweepRoutine()
    {
        var wait = new WaitForFixedUpdate();
        while (_isDashing)
        {
            yield return wait;
            Vector3 curPos = Context.transform.position;
            SweepSegment(_lastPos, curPos);
            _lastPos = curPos;
        }
    }

    private void FinalSweep(Vector3 endPos)
    {
        SweepSegment(_lastPos, endPos);
    }

    private void SweepSegment(Vector3 a, Vector3 b)
    {
        Collider[] hits = Physics.OverlapCapsule(a, b, Definition.Raduis);
        for (int i = 0; i < hits.Length; i++)
        {
            var col = hits[i];
            if (_alreadyFrozen.Contains(col)) continue;
            if (!col.TryGetComponent(out IDamageable _)) continue;
            if (col.TryGetComponent(out StunDebuff stun))
            {
                stun.ApplyStun(Definition.Duration);
                _alreadyFrozen.Add(col);
            }
        }
    }
}
