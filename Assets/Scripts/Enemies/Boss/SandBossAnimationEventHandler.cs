using UnityEngine;

public class SandBossAnimationEventHandler : MonoBehaviour
{
    [SerializeField] private SandBossAttack _attack;
    [SerializeField] private CameraShakeEffect _cameraShakeEffect;
    public void Melee_Begin() { }

    public void Melee_Hit()
    {
        _attack.DoMeleeHit();
        _cameraShakeEffect.ShakeCamera(2);
    }
    public void Melee_End()   { _attack.EndManualCast(false); }
    public void Dash_Begin()  {}

    public void Dash_Start()
    {
        _attack.DashStart();
        _cameraShakeEffect.ShakeCamera(2);
    } 
    public void Dash_End()    { _attack.EndManualCast(false); }
    public void Shatter_Begin()  {  }

    public void Shatter_Impact()
    {
        _attack.EarthShatterImpact();
        _cameraShakeEffect.ShakeCamera(2);
    }
    public void Shatter_End()    { _attack.EndManualCast(false); }
    public void Toss_Begin()  {}
    public void Toss_Create() { _attack.PrepareBoulder(); }
    public void Toss_Throw()  { _attack.ThrowPreparedBoulder(); }
    public void Toss_End()    { _attack.EndManualCast(false); }

}