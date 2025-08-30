using UnityEngine;

public class SandBossAnimationEventHandler : MonoBehaviour
{
    [SerializeField] private SandBossAttack _attack;

    // === MELEE ===
    public void Melee_Begin() { /* просто маркер; логику уже включили BeginManualCast в мозге */ }
    public void Melee_Hit()   { _attack.DoMeleeHit(); }
    public void Melee_End()   { _attack.EndManualCast(false); }

    // === DASH ===
    public void Dash_Begin()  { /* BeginManualCast уже вызван мозгом */ }
    public void Dash_Start()  { _attack.DashStart(); }   // пуск рывка
    public void Dash_End()    { _attack.EndManualCast(false); }

    // === EARTH SHATTER ===
    public void Shatter_Begin()  { /* BeginManualCast уже вызван мозгом */ }
    public void Shatter_Impact() { _attack.EarthShatterImpact(); }
    public void Shatter_End()    { _attack.EndManualCast(false); }

    // === BOULDER TOSS ===
    public void Toss_Begin()  { /* BeginManualCast уже вызван мозгом */ }
    public void Toss_Create() { _attack.PrepareBoulder(); }
    public void Toss_Throw()  { _attack.ThrowPreparedBoulder(); }
    public void Toss_End()    { _attack.EndManualCast(false); }

}