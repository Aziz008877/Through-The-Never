using UnityEngine;

public class NemesisTarget : MonoBehaviour
{
    [SerializeField] private string _npcId; // Уникальный, стабильный ID
    [SerializeField] private bool _autoGuid = true;
    [SerializeField] private float _baseMaxHp = 100f;
    [SerializeField] private float _baseDamage = 10f;

    private float _currentMaxHp;
    private float _currentDamage;
    private NemesisMarkUI _mark;

    public string NpcId => _npcId;
    public float CurrentMaxHp => _currentMaxHp;
    public float CurrentDamage => _currentDamage;

    private void Reset()
    {
        if (string.IsNullOrWhiteSpace(_npcId))
            _npcId = System.Guid.NewGuid().ToString("N");
    }

    private void Awake()
    {
        if (_autoGuid && string.IsNullOrWhiteSpace(_npcId))
            _npcId = System.Guid.NewGuid().ToString("N");
    }

    private void OnEnable()
    {
        ApplyNemesisStatsAndMark();
        if (NemesisRuntime.Svc != null)
            NemesisRuntime.Svc.OnLevelChanged += HandleLevelChanged;
    }

    private void OnDisable()
    {
        if (NemesisRuntime.Svc != null)
            NemesisRuntime.Svc.OnLevelChanged -= HandleLevelChanged;

        if (_mark) Destroy(_mark.gameObject);
    }

    private void HandleLevelChanged(string id, int level)
    {
        if (id != _npcId) return;
        ApplyNemesisStatsAndMark();
    }

    private void ApplyNemesisStatsAndMark()
    {
        var svc = NemesisRuntime.Svc;
        int lvl = svc != null ? svc.GetLevel(_npcId) : 0;

        float hpMul = svc != null ? svc.GetHpMultiplier(_npcId) : 1f;
        float dmgMul = svc != null ? svc.GetDamageMultiplier(_npcId) : 1f;

        _currentMaxHp = Mathf.Max(1f, _baseMaxHp * hpMul);
        _currentDamage = Mathf.Max(0f, _baseDamage * dmgMul);

        // UI mark
        if (svc != null)
        {
            var cfg = (svc.GetType()
                .GetField("_cfg", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.GetValue(svc)) as NemesisConfig;

            if (cfg != null && cfg.MarkPrefab)
            {
                bool shouldShow = lvl > 0;
                if (shouldShow)
                {
                    if (!_mark)
                    {
                        _mark = Instantiate(cfg.MarkPrefab, transform);
                        _mark.transform.localPosition = cfg.MarkOffset;
                    }

                    _mark.SetLevel(lvl);
                    _mark.gameObject.SetActive(true);
                }
                else if (_mark)
                {
                    _mark.gameObject.SetActive(false);
                }
            }
        }

        // HOOK: здесь можно прокинуть новые статы в твою систему HP/урона
        // например:
        // GetComponent<BaseEnemyHP>().SetMaxHp(_currentMaxHp);
        // GetComponent<BaseEnemyAttack>().SetBaseDamage(_currentDamage);
    }
}
