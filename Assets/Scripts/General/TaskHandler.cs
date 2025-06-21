using System;
using TMPro;
using UnityEngine;

public class TaskHandler : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _healHPTaskText;
    [SerializeField] private TextMeshProUGUI _chestTaskText;
    [SerializeField] private TextMeshProUGUI _doorTaskText;
    [SerializeField] private Color _completedTextColor;

    [SerializeField] private Fountain _fountain;
    [SerializeField] private Chest _chest;

    private void Awake()
    {
        _fountain.OnPlayerHealing += HealingTask;
        _chest.OnChestOpened += ChestTask;
    }

    private void ChestTask()
    {
        _chestTaskText.color = _completedTextColor;
    }

    private void HealingTask(bool state)
    {
        _healHPTaskText.color = _completedTextColor;
    }

    private void OnDestroy()
    {
        _fountain.OnPlayerHealing -= HealingTask;
        _chest.OnChestOpened -= ChestTask;
    }
}
