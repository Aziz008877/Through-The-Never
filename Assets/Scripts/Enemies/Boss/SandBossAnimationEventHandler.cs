using UnityEngine;

public class SandBossAnimationEventHandler : MonoBehaviour
{
    [SerializeField] private SandBossAttack _attack;
    public void Melee_Begin() { }
    public void Melee_Hit()   { _attack.DoMeleeHit(); }
    public void Melee_End()   { _attack.EndManualCast(false); }
    public void Dash_Begin()  {}
    public void Dash_Start()  { _attack.DashStart(); } 
    public void Dash_End()    { _attack.EndManualCast(false); }
    public void Shatter_Begin()  {  }
    public void Shatter_Impact() { _attack.EarthShatterImpact(); }
    public void Shatter_End()    { _attack.EndManualCast(false); }
    public void Toss_Begin()  {}
    public void Toss_Create() { _attack.PrepareBoulder(); }
    public void Toss_Throw()  { _attack.ThrowPreparedBoulder(); }
    public void Toss_End()    { _attack.EndManualCast(false); }

}