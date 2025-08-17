using System.Collections;
using UnityEngine;

public class DeepfrostWatersSkill : ActiveSkillBehaviour
{
    public override void TryCast()
    {
        if (!IsReady) return;
        base.TryCast();
        if (!Context.DeepfrostWaterMode)
        {
            Context.DeepfrostWaterMode = true;
            StartCoroutine(ModeRoutine());
        }
    }

    private IEnumerator ModeRoutine()
    {
        yield return new WaitForSeconds(Definition.Duration);
        Context.DeepfrostWaterMode = false;
        StartCooldown();
    }
}
