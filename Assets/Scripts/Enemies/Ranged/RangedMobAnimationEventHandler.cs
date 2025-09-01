using UnityEngine;

public class RangedMobAnimationEventHandler : MonoBehaviour
{
    [SerializeField] private RangedMobAttack _atk;
    public void Melee_Begin() {}
    public void Melee_Hit()  { _atk.Melee_Hit(); }
    public void Melee_End()  { _atk.Melee_End(); }
    
    public void Ranged_Begin() {}
    public void Ranged_Fire() { _atk.Ranged_Fire(); }
    public void Ranged_End()  { _atk.Ranged_End(); }
    
    public void Buff_Begin() {}
    public void Buff_Apply() { _atk.Buff_Apply(); }
    public void Buff_End()   { _atk.Buff_End(); }
}
