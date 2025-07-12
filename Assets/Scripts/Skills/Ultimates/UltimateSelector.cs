using System.Collections.Generic;
using UnityEngine;
using Zenject;
public class UltimateSelector : MonoBehaviour
{
    [Header("All Ultimates")]
    [SerializeField] private UltimateDefinition[] _ultimates;
    [Inject] private PlayerSkillManager _playerSkillManager;
    private readonly Dictionary<Aspect, int> _totalStars = new();
    private void Start()
    {
        CountStars();
        ChooseAndSpawnUltimate();
    }
    
    private void CountStars()
    {
        _totalStars.Clear();
        foreach (Aspect a in System.Enum.GetValues(typeof(Aspect)))
            _totalStars[a] = 0;

        foreach (SkillDefinition def in _playerSkillManager.ChosenSkills)
        foreach (AspectStars s in def.Stars)
            _totalStars[s.Aspect] += s.Stars;
    }

    private void ChooseAndSpawnUltimate()
    {
        foreach (UltimateDefinition ult in _ultimates)
        {
            if (EnoughStars(ult.Cost))
            {
                Instantiate(ult.Behaviour, transform);
                Debug.Log($"Ultimate '{ult.DisplayName}' unlocked");
                return;
            }
        }

        Debug.LogWarning("No ultimate matches current star combination");
    }

    private bool EnoughStars(AspectStars[] cost)
    {
        foreach (AspectStars c in cost)
            if (_totalStars[c.Aspect] < c.Stars) return false;
        
        return true;
    }
}