using System.Collections.Generic;
using UnityEngine;
public class SkillChest : MonoBehaviour, IInteractable
{
    [SerializeField] private SkillSlot _slotType;
    [SerializeField] private SkillPoolSO _pool;
    [SerializeField] private SkillSelectionSaver _saver;
    public Transform InteractionUI { get; set; }
    public bool CanInteract { get; set; } = true;

    public void PerformAction(GameObject player)
    {
        if (!CanInteract) return;
        CanInteract = false;
        SpawnRandomPages();
        InteractionUI.gameObject.SetActive(false);
    }

    private void SpawnRandomPages()
    {
        List<SkillDefinition> pool = _pool.GetBySlot(_slotType);
        var chosen = new List<SkillDefinition>();
        while (chosen.Count < 3 && pool.Count > 0)
        {
            int index = Random.Range(0, pool.Count);
            chosen.Add(pool[index]);
            pool.RemoveAt(index);
        }
    }

    public void OnSkillPicked(SkillDefinition definition)
    {
        _saver.AddSkill(definition);
    }
}