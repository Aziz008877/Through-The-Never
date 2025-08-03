using UnityEngine;
using Zenject;

public class HotbarInput : MonoBehaviour
{
    [SerializeField] private Transform     _spawnRoot;
    [SerializeField] private PlayerContext _ctx;
    [SerializeField] private ItemInventory _inv;
    [Inject] private SkillRuntimeFactory _factory;
    private void Update()
    {
        for (int i = 0; i < 6; i++)
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                Use(i);
    }

    private void Use(int idx)
    {
        var item = _inv.GetHotbarSlot(idx);
        if (item == null) return;
        if (item.Skill == null)
        {   Debug.LogWarning($"{item.DisplayName}: SkillDefinition missing"); return; }

        Debug.Log($"Scroll {item.DisplayName} → will spawn prefab {item.Skill.BehaviourPrefab.name}");
        // 1) спавним через фабрику
        var beh = _factory.Spawn(item.Skill, _ctx, null) as ActiveSkillBehaviour;
        if (beh == null)
        {
            Debug.LogError($"Failed to spawn skill for {item.DisplayName}");
            return;
        }

        beh.TryCast();                 // активируем

        // 2) очищаем хот-бар и –1 scroll
        _inv.ClearHotbarSlot(idx);
        _inv.Remove(item, 1);
    }

}
