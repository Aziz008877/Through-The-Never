using UnityEngine;

[DisallowMultipleComponent]
public sealed class NemesisTarget : MonoBehaviour
{
    [SerializeField] private string _npcId;      // стабильный уникальный ID
    [SerializeField] private bool _autoGuid = true;

    [Header("Base Stats (optional)")]
    [SerializeField] private float _baseMaxHp = 100f;
    [SerializeField] private float _baseDamage = 10f;

    public string NpcId => _npcId;

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
        ApplyNemesisModifiers();
        if (NemesisRuntime.Svc != null)
            NemesisRuntime.Svc.OnLevelChanged += OnNemesisLevelChanged;
    }

    private void OnDisable()
    {
        if (NemesisRuntime.Svc != null)
            NemesisRuntime.Svc.OnLevelChanged -= OnNemesisLevelChanged;
    }

    private void OnNemesisLevelChanged(string id, int lvl)
    {
        if (id != _npcId) return;
        ApplyNemesisModifiers();
    }

    public void ApplyNemesisModifiers()
    {
        var svc = NemesisRuntime.Svc;
        if (svc == null) return;

        float hp = Mathf.Max(1f, _baseMaxHp * svc.HpMul(_npcId));
        float dmg = Mathf.Max(0f, _baseDamage * svc.DamageMul(_npcId));
        
        // GetComponent<BaseEnemyHP>()?.SetMaxHp(hp);
        // GetComponent<BaseEnemyAttackAdapter>()?.SetBaseDamage(dmg);
    }
    
    public void OnDied()
    {
        NemesisRuntime.Svc?.Clear(_npcId);
        
    }
}