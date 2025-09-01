using UnityEngine;

public class MeleeMobAnimationEventHandler : MonoBehaviour
{
    [SerializeField] private MeleeMobAttack _attack;
    public void Melee_Begin() {}
    public void Melee_Hit()  { _attack.Melee_Hit(); }
    public void Melee_End()  { _attack.Melee_End(); }
    
    public void Heavy_Begin() {}
    public void Heavy_Impact() { _attack.Heavy_Impact(); }
    public void Heavy_End()    { _attack.Heavy_End(); }
    
    public void Roar_Begin() {}
    public void Roar_Apply() { _attack.Roar_Apply(); }
    public void Roar_End()   { _attack.Roar_End(); }
    
    public void Toss_Begin()  {}
    public void Toss_Create() { _attack.Toss_Create(); }
    public void Toss_Throw()  { _attack.Toss_Throw(); }
    public void Toss_End()    { _attack.Toss_End(); }
}
