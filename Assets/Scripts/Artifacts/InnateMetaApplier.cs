using System.Reflection;
using UnityEngine;

public class InnateMetaApplier : MonoBehaviour
{
    [SerializeField] private FireInnateSkill _fireInnate; // опционально: можно не заполнять, если нет на персонаже
    [SerializeField] private IceInnateSkill  _iceInnate;

    private static readonly BindingFlags BF = BindingFlags.Instance | BindingFlags.NonPublic;

    private void Start()
    {
        var meta = MetaProgressionService.Instance;
        if (!meta) return;

        // Разрешение школ + включение иннейтов
        bool fireOn = meta.IsSchoolUnlocked(MagicSchool.Fire);
        bool iceOn  = meta.IsSchoolUnlocked(MagicSchool.Ice);

        if (_fireInnate) _fireInnate.enabled = fireOn;
        if (_iceInnate)  _iceInnate.enabled  = iceOn;

        // Подмена значений под уровни
        if (_fireInnate && fireOn)
        {
            SetPrivate(_fireInnate, "_dotPercent",  meta.PhoenixDotPercent);
            SetPrivate(_fireInnate, "_dotDuration", meta.PhoenixDotDuration);
        }

        if (_iceInnate && iceOn)
        {
            SetPrivate(_iceInnate, "_slowPerStack", meta.MoonSlowMovePerStack);
            SetPrivate(_iceInnate, "_dmgRedPerStack", meta.MoonDmgRedPerStack);
            SetPrivate(_iceInnate, "_duration", 3f); // оставляем как в твоём дефолте или вынеси в конфиг
            SetPrivate(_iceInnate, "_maxStacks", meta.MoonMaxStacks);

            // Если у тебя где-то есть поддержка замедления скорости атаки — достань компонент/дебафф и используй meta.MoonAttackSpeedSlowPerStack
            // Здесь мы не меняем сигнатуры IFrostbiteReceivable.
        }
    }

    private static void SetPrivate(object obj, string fieldName, object value)
    {
        var f = obj.GetType().GetField(fieldName, BF);
        if (f != null) f.SetValue(obj, value);
    }
}