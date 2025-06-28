using System;
using TMPro;
using UnityEngine;

public class TaskHandler : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _healHPTaskText;
    [SerializeField] private TextMeshProUGUI _chestTaskText;
    [SerializeField] private TextMeshProUGUI _npcTaskText;
    [SerializeField] private Color _completedTextColor;

    [SerializeField] private Fountain _fountain;
    [SerializeField] private Chest _chest;
    [SerializeField] private NPCHandler _npcHandler;
    private void Awake()
    {
        if (_fountain != null)
        {
            _fountain.OnPlayerHealing += HealingTask;
        }
        
        _npcHandler.OnNPCAnswered += TalkTask;
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

    private void TalkTask(string text)
    {
        _npcTaskText.color = _completedTextColor;
    }

    private void OnDestroy()
    {
        if (_fountain != null)
        {
            _fountain.OnPlayerHealing -= HealingTask;
        }
        
        _chest.OnChestOpened -= ChestTask;
        _npcHandler.OnNPCAnswered -= TalkTask;
    }
}
