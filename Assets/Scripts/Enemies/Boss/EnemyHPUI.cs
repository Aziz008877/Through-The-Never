using System;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHPUI : MonoBehaviour
{
    [SerializeField] private Image _hpValue;
    private BaseEnemyHP _baseEnemyHp;
    private void Awake()
    {
        _baseEnemyHp = GetComponent<BaseEnemyHP>();
        _baseEnemyHp.OnDamaged += EnemyDamaged;
    }

    private void EnemyDamaged(DamageContext damageContext)
    {
        _hpValue.fillAmount = _baseEnemyHp.CurrentHP / _baseEnemyHp.MaxHP;
    }

    private void OnDestroy()
    {
        _baseEnemyHp.OnDamaged -= EnemyDamaged;
    }
}
