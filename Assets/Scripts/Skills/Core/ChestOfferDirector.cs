using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Random = UnityEngine.Random;

public enum OfferStage { Special, Defense, Dash, Passive }

public class ChestOfferDirector : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private SkillCatalog _catalog;
    [SerializeField] private PlayerSkillManager _skillManager;
    [SerializeField] private SkillSelectionSaver _selectionSaver;

    [Header("Config")]
    [SerializeField] private int _offersPerChest = 3;
    [SerializeField] private int _cycles = 2;

    [SerializeField] private OfferStage[] _order = new[]
    {
        OfferStage.Special,
        OfferStage.Defense,
        OfferStage.Dash,
        OfferStage.Passive
    };

    [Header("Runtime")]
    [SerializeField] private MagicSchool _school;

    private SkillCatalog.SchoolBundle _bundle;
    private readonly HashSet<SkillDefinition> _taken = new();
    private int _stageIndex;
    private int _cycleIndex;
    private bool _initialized;
    private void Start()
    {
        if (_selectionSaver != null && _selectionSaver.HasSchool)
            _school = _selectionSaver.School;

        TryRestoreFromSaver();

        if (!_initialized)
            InitializeAndGrantInitial();
    }

    public void SetSchool(MagicSchool school)
    {
        _school = school;
    }

    public void InitializeAndGrantInitial()
    {
        if (_initialized) return;

        if (_catalog == null || _skillManager == null)
        {
            Debug.LogError("[Director] Missing refs (Catalog/SkillManager).");
            return;
        }

        if (!_catalog.TryGetBundle(_school, out _bundle))
        {
            Debug.LogError($"[Director] No SkillCatalog bundle for school: '{_school}'");
            return;
        }

        _taken.Clear();
        SeedTakenFromManager();
        SeedStartersIntoTaken();

        _stageIndex = 0;
        _cycleIndex = 0;
        _initialized = true;

        SaveProgressToSaver();
        DumpBundle("[Director] Initialized");
    }

    private void TryRestoreFromSaver()
    {
        if (_selectionSaver == null) return;

        var p = _selectionSaver.Progress;
        if (!p.Initialized) return;
        if (!_selectionSaver.HasSchool) return;
        if (!_selectionSaver.School.Equals(_school))
        {
            return;
        }

        if (!_catalog.TryGetBundle(_school, out _bundle))
        {
            Debug.LogError($"[Director] Restore failed: bundle not found for '{_school}'");
            return;
        }

        _taken.Clear();
        
        var chosen = _selectionSaver.Chosen;
        for (int i = 0; i < chosen.Count; i++)
            if (chosen[i]) _taken.Add(chosen[i]);
        
        var takenList = _selectionSaver.TakenList;
        for (int i = 0; i < takenList.Count; i++)
            if (takenList[i]) _taken.Add(takenList[i]);

        SeedStartersIntoTaken();

        _stageIndex = Mathf.Clamp(p.StageIndex, 0, Mathf.Max(0, _order.Length - 1));
        _cycleIndex = Mathf.Max(0, p.CycleIndex);
        _initialized = true;

        DumpBundle("[Director] Restored from Saver");
    }

    public bool EnsureInitialized(MagicSchool fallbackSchool)
    {
        if (_initialized) return true;
        if (_school.Equals(default(MagicSchool))) _school = fallbackSchool;

        TryRestoreFromSaver();
        if (_initialized) return true;

        InitializeAndGrantInitial();
        return _initialized;
    }

    public bool TryGetNextOffer(out OfferStage stage, out List<SkillDefinition> offer)
    {
        stage = default;
        offer = null;

        if (!_initialized)
        {
            Debug.LogWarning("[Director] Not initialized.");
            return false;
        }

        if (_cycleIndex >= _cycles)
        {
            Debug.Log("[Director] All cycles consumed.");
            return false;
        }

        int tries = 0;
        while (_cycleIndex < _cycles && tries < _order.Length)
        {
            stage = _order[_stageIndex];
            var pool = GetPool(stage);

            var candidates = new List<SkillDefinition>(pool.Length);
            for (int i = 0; i < pool.Length; i++)
            {
                var def = pool[i];
                if (!def) continue;
                if (_taken.Contains(def)) continue;
                candidates.Add(def);
            }

            Debug.Log($"[Director] Stage={stage}, pool={pool.Length}, taken={_taken.Count}, available={candidates.Count}, cycle={_cycleIndex+1}/{_cycles}");

            if (candidates.Count > 0)
            {
                Shuffle(candidates);
                var count = Mathf.Min(_offersPerChest, candidates.Count);
                offer = candidates.GetRange(0, count);
                return true;
            }

            AdvanceStage();
            tries++;
        }

        Debug.LogWarning($"[Director] No candidates. cycle={_cycleIndex}/{_cycles}, stageIdx={_stageIndex}, orderLen={_order.Length}");
        return false;
    }

    public void AcceptChoice(SkillDefinition chosen)
    {
        if (!chosen) return;
        if (_taken.Contains(chosen)) return;

        _taken.Add(chosen);

        _skillManager.AddSkills(new List<SkillDefinition> { chosen });
        _selectionSaver.AddSkill(chosen);

        AdvanceStage();
        SaveProgressToSaver();

        Debug.Log($"[Director] Accepted: {chosen.name}. Next stage idx={_stageIndex}, cycle={_cycleIndex+1}/{_cycles}");
    }

    private void AdvanceStage()
    {
        _stageIndex++;
        if (_stageIndex >= _order.Length)
        {
            _stageIndex = 0;
            _cycleIndex++;
        }
    }

    private SkillDefinition[] GetPool(OfferStage s)
    {
        switch (s)
        {
            case OfferStage.Special: return _bundle.Specials ?? System.Array.Empty<SkillDefinition>();
            case OfferStage.Defense: return _bundle.Defenses ?? System.Array.Empty<SkillDefinition>();
            case OfferStage.Dash:    return _bundle.Dash     ?? System.Array.Empty<SkillDefinition>();
            case OfferStage.Passive: return _bundle.Passives ?? System.Array.Empty<SkillDefinition>();
        }
        return System.Array.Empty<SkillDefinition>();
    }

    private void SeedTakenFromManager()
    {
        var chosen = _skillManager.ChosenSkills;
        for (int i = 0; i < chosen.Count; i++)
            if (chosen[i]) _taken.Add(chosen[i]);
    }

    private void SeedStartersIntoTaken()
    {
        if (_bundle.Basic)       _taken.Add(_bundle.Basic);
        if (_bundle.StarterDash) _taken.Add(_bundle.StarterDash);
        if (_bundle.Innate)      _taken.Add(_bundle.Innate);
    }

    private void SaveProgressToSaver()
    {
        if (_selectionSaver == null) return;

        _selectionSaver.Progress = new OfferProgress
        {
            Initialized = _initialized,
            StageIndex = _stageIndex,
            CycleIndex = _cycleIndex,
            School = _school
        };
        
        foreach (var def in _taken)
            _selectionSaver.MarkTakenOnly(def);
    }

    private void DumpBundle(string header)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"{header} for school '{_school}':");
        sb.AppendLine($"  Starters: Basic={NameOrNull(_bundle.Basic)}, Dash={NameOrNull(_bundle.StarterDash)}, Innate={NameOrNull(_bundle.Innate)}");
        sb.AppendLine($"  Pools: Specials={Len(_bundle.Specials)}, Defenses={Len(_bundle.Defenses)}, Dash={Len(_bundle.Dash)}, Passives={Len(_bundle.Passives)}");
        sb.AppendLine($"  Order: {string.Join(" -> ", _order)} | StageIdx={_stageIndex} | Cycle={_cycleIndex}/{_cycles}");
        Debug.Log(sb.ToString());
    }

    private static string NameOrNull(SkillDefinition d) => d ? d.name : "NULL";
    private static int Len(SkillDefinition[] a) => a == null ? 0 : a.Length;

    private static void Shuffle<T>(IList<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}
