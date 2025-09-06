using System.Reflection;
using UnityEngine;

public class InnateMetaApplier : MonoBehaviour
{
    [SerializeField] private FireInnateSkill _fireInnate;
    [SerializeField] private IceInnateSkill  _iceInnate;

    private static readonly BindingFlags BF = BindingFlags.Instance | BindingFlags.NonPublic;

    private void Start()
    {
        var meta = MetaProgressionService.Instance;
        if (!meta) return;
        
        bool fireOn = meta.IsSchoolUnlocked(MagicSchool.Fire);
        bool iceOn  = meta.IsSchoolUnlocked(MagicSchool.Ice);

        if (_fireInnate) _fireInnate.enabled = fireOn;
        if (_iceInnate)  _iceInnate.enabled  = iceOn;
        
        if (_fireInnate && fireOn)
        {
            SetPrivate(_fireInnate, "_dotPercent",  meta.PhoenixDotPercent);
            SetPrivate(_fireInnate, "_dotDuration", meta.PhoenixDotDuration);
        }

        if (_iceInnate && iceOn)
        {
            SetPrivate(_iceInnate, "_slowPerStack", meta.MoonSlowMovePerStack);
            SetPrivate(_iceInnate, "_dmgRedPerStack", meta.MoonDmgRedPerStack);
            SetPrivate(_iceInnate, "_duration", 3f); 
            SetPrivate(_iceInnate, "_maxStacks", meta.MoonMaxStacks);
            
        }
    }

    private static void SetPrivate(object obj, string fieldName, object value)
    {
        var f = obj.GetType().GetField(fieldName, BF);
        if (f != null) f.SetValue(obj, value);
    }
}